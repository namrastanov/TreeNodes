namespace TreeNodes.Application.Common.DTOs;

/// <summary>
/// Request DTO for partner authentication.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Partner authentication code.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}

