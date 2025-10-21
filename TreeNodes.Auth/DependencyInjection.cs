using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TreeNodes.Auth.Interfaces;
using TreeNodes.Auth.Options;
using TreeNodes.Auth.Services;

namespace TreeNodes.Auth;

/// <summary>
/// Dependency injection configuration for TreeNodes.Auth
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds authentication services to the service collection
    /// </summary>
    public static IServiceCollection AddTreeNodesAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure options
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));

        // Register services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        // Get auth options for JWT configuration
        var authOptions = configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>();
        
        if (authOptions == null)
            throw new InvalidOperationException("Auth configuration is missing.");

        if (string.IsNullOrEmpty(authOptions.JwtSecret))
            throw new InvalidOperationException("JWT secret is not configured.");

        var key = Encoding.UTF8.GetBytes(authOptions.JwtSecret);

        // Configure JWT authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Set to true in production
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = authOptions.JwtIssuer,
                ValidateAudience = true,
                ValidAudience = authOptions.JwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Configure events to throw exceptions instead of returning 401
            // This allows the GlobalExceptionHandlingMiddleware to catch and log auth failures
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    // Throw exception so it can be caught by global exception handler
                    throw new Exceptions.InvalidTokenException(
                        "Authentication failed: " + context.Exception.Message,
                        context.Exception);
                },
                OnChallenge = context =>
                {
                    // Prevent default 401 response
                    context.HandleResponse();
                    
                    // Throw exception so it can be caught by global exception handler
                    var errorMessage = string.IsNullOrEmpty(context.Error) 
                        ? "Unauthorized access - valid JWT token required" 
                        : $"Unauthorized: {context.Error}";
                    
                    throw new Exceptions.AuthenticationException(errorMessage);
                },
                OnForbidden = context =>
                {
                    // Throw exception for forbidden access
                    throw new Exceptions.AuthenticationException("Access forbidden - insufficient permissions");
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
}

