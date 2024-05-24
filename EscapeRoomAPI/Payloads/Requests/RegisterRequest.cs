using EscapeRoomAPI.Dtos;

namespace EscapeRoomAPI.Payloads.Requests;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? Email { get; set; } = string.Empty;

    public DateTime? RegistrationDate { get; set; } = DateTime.Now;

    public bool? IsActive { get; set; } = false;
}

public static class RegisterRequestExtension
{
    public static PlayerDto ToPlayerDto(this RegisterRequest request)
    {
        return new PlayerDto
        {
            PlayerId = Guid.NewGuid().ToString(),
            Username = request.Username,
            Password = request.Password,
            Email = request.Email,
            RegistrationDate = request.RegistrationDate,
            IsActive = request.IsActive
        };
    }
}