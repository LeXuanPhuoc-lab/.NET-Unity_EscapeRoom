namespace EscapeRoomAPI.Dtos;

public class GameSessionDto
{
    public int SessionId { get; set; }
    
    public string? SessionName { get; set; } = string.Empty;
    
    public string SessionCode { get; set; } = string.Empty;
    
    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int TotalPlayer { get; set; }

    public bool IsWaiting { get; set; }

    public bool IsEnd { get; set; }

    public bool IsPublic { get; set; }

    public string Hint { get; set; } = string.Empty;

    public virtual ICollection<PlayerGameSessionDto> PlayerGameSessions { get; set; } =
        new List<PlayerGameSessionDto>();
}