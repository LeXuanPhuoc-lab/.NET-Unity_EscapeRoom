namespace EscapeRoomAPI.Dtos;

public class PlayerDto
{
    public int Id { get; set; }

    public string PlayerId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public bool? IsActive { get; set; }

    // public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    // public virtual ICollection<PlayerGameAnswer> PlayerGameAnswers { get; set; } = new List<PlayerGameAnswer>();

    // public virtual ICollection<GameSession> Sessions { get; set; } = new List<GameSession>();
}
