namespace EscapeRoomAPI.Dtos;

public class GameSessionDto
{
    public string? SessionName { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int TotalPlayer { get; set; }

    public bool? IsWaiting { get; set; } 

    public bool? IsEnd { get; set; } 
    public virtual ICollection<PlayerDto> Players { get; set; } = new List<PlayerDto>();
}
