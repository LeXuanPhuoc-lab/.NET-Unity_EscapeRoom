using System.Text.Json.Serialization;

namespace EscapeRoomAPI.Dtos;

public class PlayerGameSessionDto
{
    public int SessionId { get; set; }

    public string PlayerId { get; set; } = string.Empty;

    public bool? IsHost { get; set; }

    public bool? IsReady { get; set; }

    [JsonIgnore]
    public virtual PlayerDto Player { get; set; } = null!;

    [JsonIgnore]
    public virtual GameSessionDto Session { get; set; } = null!;
}