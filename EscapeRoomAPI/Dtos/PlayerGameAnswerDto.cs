using System.Text.Json.Serialization;

namespace EscapeRoomAPI.Dtos;

public class PlayerGameAnswerDto
{
    public int Id { get; set; }

    public int SessionId { get; set; }

    public string PlayerId { get; set; } = string.Empty;

    public string QuestionId { get; set; } = string.Empty;

    public string SelectAnswerId { get; set; } = string.Empty;

    public bool? IsCorrect { get; set; }

    public virtual PlayerDto Player { get; set; } = null!;

    [JsonIgnore]
    public virtual QuestionDto Question { get; set; } = null!;

    [JsonIgnore]
    public virtual QuestionAnswerDto SelectAnswer { get; set; } = null!;

    public virtual GameSessionDto Session { get; set; } = null!;
}