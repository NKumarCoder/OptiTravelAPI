using System.Reflection;
using Microsoft.OpenApi;
using OptiTravelAPI.Web.Configuration;

namespace OptiTravelAPI.Web.Extensions;

public static class SwaggerExtensions
{
    /// <summary>
    /// Configures Swagger and OpenAPI specifications.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        var swaggerOptions = configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>() ?? new SwaggerOptions();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = swaggerOptions.Title,
                Version = swaggerOptions.Version,
                Description = swaggerOptions.Description,
                Contact = new OpenApiContact
                {
                    Name = "OptiTravelAPI Engineering Team",
                    Email = "support@optitravelapi.internal"
                }
            });

            // Include XML comments if generated
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            var sharedXmlPath = Path.Combine(AppContext.BaseDirectory, "OptiTravelAPI.Shared.xml");
            if (File.Exists(sharedXmlPath))
            {
                c.IncludeXmlComments(sharedXmlPath);
            }

            // Configure Bearer Authentication in Swagger UI
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "To get access token, login with username: user, password: userword.\nEnter your Bearer token in the format: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", null, null),
                    new List<string>()
                }
            });
        });

        return services;
    }

    /// <summary>
    /// Enables Swagger UI middleware if enabled in configuration or running in Development environment.
    /// </summary>
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var swaggerOptions = configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>() ?? new SwaggerOptions();

        if (environment.IsDevelopment() || swaggerOptions.Enabled)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{swaggerOptions.Title} {swaggerOptions.Version}");
                c.RoutePrefix = "swagger";
                c.DocumentTitle = swaggerOptions.Title;
                c.DisplayRequestDuration();
            });
        }

        return app;
    }
}
