# ==============================================================================
# Multi-Stage Dockerfile for OptiTravelAPI (.NET 10)
# ==============================================================================

# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for caching dependency restore
COPY ["src/OptiTravelAPI.Web/OptiTravelAPI.Web.csproj", "src/OptiTravelAPI.Web/"]
COPY ["src/OptiTravelAPI.ConsolidationSupplierLayer/OptiTravelAPI.ConsolidationSupplierLayer.csproj", "src/OptiTravelAPI.ConsolidationSupplierLayer/"]
COPY ["src/OptiTravelAPI.ConsolidationLayer/OptiTravelAPI.ConsolidationLayer.csproj", "src/OptiTravelAPI.ConsolidationLayer/"]
COPY ["src/OptiTravelAPI.SupplierLayer/OptiTravelAPI.SupplierLayer.csproj", "src/OptiTravelAPI.SupplierLayer/"]
COPY ["src/OptiTravelAPI.Suppliers/OptiTravelAPI.Suppliers.csproj", "src/OptiTravelAPI.Suppliers/"]
COPY ["src/OptiTravelAPI.Shared/OptiTravelAPI.Shared.csproj", "src/OptiTravelAPI.Shared/"]

RUN dotnet restore "src/OptiTravelAPI.Web/OptiTravelAPI.Web.csproj"

# Copy entire source tree
COPY . .

WORKDIR "/src/src/OptiTravelAPI.Web"
RUN dotnet publish "OptiTravelAPI.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Minimal Production Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Run as non-root user provided by official .NET images
USER $APP_UID

COPY --from=build /app/publish .

# Dynamic port binding: PORT environment variable is respected at runtime
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "OptiTravelAPI.Web.dll"]
