using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TreeNodes.Auth.Interfaces;
using TreeNodes.Auth.Options;

namespace TreeNodes.Auth.Services;

/// <summary>
/// Implementation of JWT token service
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly AuthOptions _authOptions;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(IOptions<AuthOptions> authOptions)
    {
        _authOptions = authOptions.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string GenerateToken(string partnerCode)
    {
        var key = Encoding.UTF8.GetBytes(_authOptions.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, partnerCode),
                new Claim("partner_code", partnerCode)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.TokenExpirationMinutes),
            Issuer = _authOptions.JwtIssuer,
            Audience = _authOptions.JwtAudience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }

    public string? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        try
        {
            var key = Encoding.UTF8.GetBytes(_authOptions.JwtSecret);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _authOptions.JwtIssuer,
                ValidateAudience = true,
                ValidAudience = _authOptions.JwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = _tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            
            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                var partnerCodeClaim = principal.FindFirst("partner_code");
                return partnerCodeClaim?.Value;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}

