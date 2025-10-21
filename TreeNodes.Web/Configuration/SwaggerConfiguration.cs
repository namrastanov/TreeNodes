using Microsoft.OpenApi.Models;

namespace TreeNodes.Web.Configuration;

/// <summary>
/// Swagger/OpenAPI configuration
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Configures Swagger/OpenAPI with JWT Bearer authentication support
    /// </summary>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TreeNodes API",
                Version = "v1",
                Description = "TreeNodes API with JWT Authentication",
                Contact = new OpenApiContact
                {
                    Name = "TreeNodes API"
                }
            });

            // Define JWT Bearer authentication scheme
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token in the format: {your token here}\n\n" +
                              "Example: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\n\n" +
                              "To get a token:\n" +
                              "1. Call POST /api.user.partner.rememberMe with your partner code\n" +
                              "2. Copy the token from the response\n" +
                              "3. Click 'Authorize' button above and paste the token"
            });

            // Require JWT Bearer authentication for all operations
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}

