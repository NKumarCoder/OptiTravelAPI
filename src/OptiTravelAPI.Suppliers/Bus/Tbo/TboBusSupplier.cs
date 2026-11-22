using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OptiTravelAPI.Shared.Constants;
using OptiTravelAPI.Shared.Exceptions;
using OptiTravelAPI.Shared.Models;
using OptiTravelAPI.Shared.Models.Bus;
using OptiTravelAPI.SupplierLayer.Bus;

namespace OptiTravelAPI.Suppliers.Bus.Tbo;

/// <summary>
/// Concrete TBO Bus Supplier implementation.
/// Handles HTTP communication, token management, robust error preservation, and logging.
/// </summary>
public class TboBusSupplier : IBusSupplier
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<SupplierConfiguration> _supplierConfiguration;
    private readonly ILogger<TboBusSupplier> _logger;

    private static readonly SemaphoreSlim AuthLock = new(1, 1);
    private static string? _cachedAccessToken;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };

    public string SupplierName => SupplierNames.Tbo;
    public string ServiceType => ServiceTypes.Bus;
    public bool IsEnabled => GetConfig()?.Enabled ?? true;

    public TboBusSupplier(
        IHttpClientFactory httpClientFactory,
        IOptions<SupplierConfiguration> supplierConfiguration,
        ILogger<TboBusSupplier> logger)
    {
        _httpClientFactory = httpClientFactory;
        _supplierConfiguration = supplierConfiguration;
        _logger = logger;
    }

    private SupplierItemConfig? GetConfig() =>
        _supplierConfiguration.Value.GetSupplierConfig(ServiceTypes.Bus, SupplierNames.Tbo);

    private HttpClient CreateClient() =>
        _httpClientFactory.CreateClient("TboBusClient");

    public async Task<string?> EnsureAuthenticatedAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_cachedAccessToken))
        {
            return _cachedAccessToken;
        }

        await AuthLock.WaitAsync(cancellationToken);
        try
        {
            if (!string.IsNullOrEmpty(_cachedAccessToken))
            {
                return _cachedAccessToken;
            }

            var config = GetConfig();
            if (string.IsNullOrWhiteSpace(config?.Username) || string.IsNullOrWhiteSpace(config?.Password))
            {
                _logger.LogWarning("TBO credentials not configured in environment or appsettings.");
                return null;
            }

            _logger.LogInformation("Authenticating with TBO Bus Supplier for user '{Username}'...", config.Username);

            var loginRequest = new BusLoginRequest
            {
                Username = config.Username,
                Password = config.Password
            };

            var loginResponse = await ExecuteRequestAsync<BusLoginRequest, BusLoginResponse>(
                HttpMethod.Post,
                "/api/bus/login",
                loginRequest,
                requiresAuth: false,
                cancellationToken: cancellationToken);

            if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
            {
                _cachedAccessToken = loginResponse.AccessToken;
                _logger.LogInformation("TBO Bus Supplier authentication successful.");
                return _cachedAccessToken;
            }

            _logger.LogWarning("TBO login did not return an access token: {ErrorDesc}", loginResponse?.Error?.Desc);
            return null;
        }
        finally
        {
            AuthLock.Release();
        }
    }

    public async Task<BusLoginResponse> LoginAsync(BusLoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await ExecuteRequestAsync<BusLoginRequest, BusLoginResponse>(
            HttpMethod.Post,
            "/api/bus/login",
            request,
            requiresAuth: false,
            cancellationToken: cancellationToken);

        if (!string.IsNullOrEmpty(response?.AccessToken))
        {
            await AuthLock.WaitAsync(cancellationToken);
            try
            {
                _cachedAccessToken = response.AccessToken;
            }
            finally
            {
                AuthLock.Release();
            }
        }

        return response ?? new BusLoginResponse();
    }

    public Task<BusSearchResponse> SearchAsync(BusSearchRequest request, CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<BusSearchRequest, BusSearchResponse>(
            HttpMethod.Post,
            "/api/bus/search",
            request,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<BusSeatLayoutResponse> SelectSeatsAsync(BusSeatLayoutRequest request, CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<BusSeatLayoutRequest, BusSeatLayoutResponse>(
            HttpMethod.Post,
            "/api/bus/select",
            request,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<BusBlockResponse> BlockSeatsAsync(BusBlockRequest request, CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<BusBlockRequest, BusBlockResponse>(
            HttpMethod.Post,
            "/api/bus/block",
            request,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<BusBookResponse> BookSeatsAsync(BusBookRequest request, CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<BusBookRequest, BusBookResponse>(
            HttpMethod.Post,
            "/api/bus/book",
            request,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<BusTicketResponse> GetTicketAsync(BusTicketRequest request, CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<BusTicketRequest, BusTicketResponse>(
            HttpMethod.Post,
            "/api/bus/ticket",
            request,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<BusTrackingResponse> GetTrackingAsync(BusTrackingRequest request, CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<BusTrackingRequest, BusTrackingResponse>(
            HttpMethod.Post,
            "/api/bus/tracking",
            request,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<BusGetTicketDetailsResponse> GetTicketDetailsAsync(string referanceNumber, CancellationToken cancellationToken = default)
    {
        var encodedRef = Uri.EscapeDataString(referanceNumber);
        return ExecuteRequestAsync<object, BusGetTicketDetailsResponse>(
            HttpMethod.Get,
            $"/api/bus/GetTicketDetails?referanceNumber={encodedRef}",
            payload: null,
            requiresAuth: true,
            cancellationToken: cancellationToken);
    }

    public Task<BusPreCancelResponse> PreCancelAsync(BusPreCancelRequest request, CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<BusPreCancelRequest, BusPreCancelResponse>(
            HttpMethod.Post,
            "/api/bus/precancel",
            request,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<BusCancelResponse> CancelAsync(BusCancelRequest request, CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<BusCancelRequest, BusCancelResponse>(
            HttpMethod.Post,
            "/api/bus/cancel",
            request,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<BusBalanceResponse> GetBalanceAsync(CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<object, BusBalanceResponse>(
            HttpMethod.Get,
            "/api/bus/balance",
            payload: null,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    public Task<CdnCitiesResponse> GetCdnCitiesAsync(CancellationToken cancellationToken = default) =>
        ExecuteRequestAsync<object, CdnCitiesResponse>(
            HttpMethod.Get,
            "/api/cdn/cities",
            payload: null,
            requiresAuth: true,
            cancellationToken: cancellationToken);

    private async Task<TResponse> ExecuteRequestAsync<TRequest, TResponse>(
        HttpMethod method,
        string endpoint,
        TRequest? payload,
        bool requiresAuth,
        CancellationToken cancellationToken)
    {
        var config = GetConfig();
        if (string.IsNullOrWhiteSpace(config?.BaseUrl))
        {
            _logger.LogError("TBO Bus Supplier BaseUrl is not configured in appsettings or environment.");
            throw new SupplierException(
                SupplierNames.Tbo,
                ServiceTypes.Bus,
                "TBO upstream supplier BaseUrl is not configured.",
                upstreamStatusCode: (int)HttpStatusCode.BadGateway,
                errorCode: "SUPPLIER_NOT_CONFIGURED");
        }

        if (!Uri.TryCreate(config.BaseUrl, UriKind.Absolute, out var baseUri))
        {
            _logger.LogError("TBO Bus Supplier BaseUrl '{BaseUrl}' is not a valid absolute URI.", config.BaseUrl);
            throw new SupplierException(
                SupplierNames.Tbo,
                ServiceTypes.Bus,
                $"TBO upstream supplier BaseUrl '{config.BaseUrl}' is invalid.",
                upstreamStatusCode: (int)HttpStatusCode.BadGateway,
                errorCode: "INVALID_SUPPLIER_CONFIG");
        }

        // Construct clean absolute URI
        var relativePath = endpoint.StartsWith('/') ? endpoint[1..] : endpoint;
        var fullUri = new Uri(baseUri, relativePath);

        var client = CreateClient();
        using var requestMsg = new HttpRequestMessage(method, fullUri);

        if (requiresAuth)
        {
            var token = await EnsureAuthenticatedAsync(cancellationToken);
            if (!string.IsNullOrEmpty(token))
            {
                requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                requestMsg.Headers.TryAddWithoutValidation("accessToken", token);
            }
        }

        if (payload != null && method != HttpMethod.Get)
        {
            requestMsg.Content = JsonContent.Create(payload, options: JsonOptions);
        }

        var timeoutDuration = config.TimeoutSeconds > 0
            ? TimeSpan.FromSeconds(config.TimeoutSeconds)
            : TimeSpan.FromSeconds(60);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeoutDuration);

        _logger.LogInformation("Sending {Method} request to TBO Bus: {Endpoint}", method, fullUri.PathAndQuery);

        var stopwatch = Stopwatch.StartNew();
        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(requestMsg, cts.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            stopwatch.Stop();
            _logger.LogInformation("TBO Bus {Method} request to {Endpoint} was cancelled by caller after {ElapsedMs}ms",
                method, fullUri.PathAndQuery, stopwatch.ElapsedMilliseconds);
            throw; // Propagate client cancellation directly
        }
        catch (OperationCanceledException ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "TBO Bus Supplier request timed out after {ElapsedMs}ms on {Endpoint}",
                stopwatch.ElapsedMilliseconds, fullUri.PathAndQuery);
            throw new SupplierException(
                SupplierNames.Tbo,
                ServiceTypes.Bus,
                $"Request timed out after {timeoutDuration.TotalSeconds}s while contacting upstream supplier.",
                ex,
                upstreamStatusCode: (int)HttpStatusCode.GatewayTimeout,
                errorCode: "TIMEOUT");
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "TBO Bus Supplier network/transport failure after {ElapsedMs}ms on {Endpoint}: {Message}",
                stopwatch.ElapsedMilliseconds, fullUri.PathAndQuery, ex.Message);
            throw new SupplierException(
                SupplierNames.Tbo,
                ServiceTypes.Bus,
                $"Network connection error while contacting upstream supplier: {ex.Message}",
                ex,
                upstreamStatusCode: (int?)ex.StatusCode ?? (int)HttpStatusCode.BadGateway,
                errorCode: "NETWORK_ERROR");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Unexpected error communicating with TBO Bus Supplier on {Endpoint}: {Message}",
                fullUri.PathAndQuery, ex.Message);
            throw new SupplierException(
                SupplierNames.Tbo,
                ServiceTypes.Bus,
                $"Unexpected error communicating with supplier: {ex.Message}",
                ex,
                upstreamStatusCode: (int)HttpStatusCode.BadGateway,
                errorCode: "SUPPLIER_ERROR");
        }

        stopwatch.Stop();
        _logger.LogInformation("TBO Bus {Method} {Endpoint} responded with HTTP {StatusCode} in {ElapsedMs}ms",
            method, fullUri.PathAndQuery, (int)response.StatusCode, stopwatch.ElapsedMilliseconds);

        var content = await response.Content.ReadAsStringAsync(cts.Token);

        // If supplier returned 401 Unauthorized, invalidate cached token
        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            _logger.LogWarning("TBO returned {StatusCode} for {Endpoint}. Invalidating cached token.", response.StatusCode, fullUri.PathAndQuery);
            await AuthLock.WaitAsync(cancellationToken);
            try
            {
                _cachedAccessToken = null;
            }
            finally
            {
                AuthLock.Release();
            }
        }

        // Handle non-success HTTP status codes
        if (!response.IsSuccessStatusCode)
        {
            string? supplierErrorDesc = null;
            string? supplierErrorCode = null;

            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("error", out var errorProp))
                    {
                        if (errorProp.TryGetProperty("desc", out var descProp))
                            supplierErrorDesc = descProp.GetString();
                        if (errorProp.TryGetProperty("code", out var codeProp))
                            supplierErrorCode = codeProp.GetString();
                    }
                }
                catch
                {
                    // Body is not JSON (e.g. HTML gateway error)
                }
            }

            var message = !string.IsNullOrWhiteSpace(supplierErrorDesc)
                ? $"Supplier error: {supplierErrorDesc}"
                : $"Supplier returned status {(int)response.StatusCode} ({response.ReasonPhrase}).";

            var errorCode = (int)response.StatusCode switch
            {
                400 => "BAD_REQUEST",
                401 => "UNAUTHORIZED",
                403 => "FORBIDDEN",
                404 => "NOT_FOUND",
                >= 500 => "UPSTREAM_SERVER_ERROR",
                _ => "SUPPLIER_ERROR"
            };

            throw new SupplierException(
                SupplierNames.Tbo,
                ServiceTypes.Bus,
                message,
                upstreamStatusCode: (int)response.StatusCode,
                errorCode: supplierErrorCode ?? errorCode,
                rawResponseBody: content);
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Activator.CreateInstance<TResponse>();
        }

        try
        {
            var deserialized = JsonSerializer.Deserialize<TResponse>(content, JsonOptions);
            return deserialized ?? Activator.CreateInstance<TResponse>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse TBO Bus Supplier response JSON from {Endpoint}", fullUri.PathAndQuery);
            throw new SupplierException(
                SupplierNames.Tbo,
                ServiceTypes.Bus,
                "Invalid or unparseable JSON received from supplier.",
                ex,
                upstreamStatusCode: (int)response.StatusCode,
                errorCode: "INVALID_RESPONSE",
                rawResponseBody: content);
        }
    }
}
