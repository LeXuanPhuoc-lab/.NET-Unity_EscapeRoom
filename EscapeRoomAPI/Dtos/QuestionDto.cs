namespace EscapeRoomAPI.Dtos;

public class QuestionDto
{
    public int Id { get; set; }

    public string QuestionId { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public int? KeyDigit { get; set; }

    public bool? IsHard { get; set; }

    // public virtual ICollection<PlayerGameAnswer> PlayerGameAnswers { get; set; } = new List<PlayerGameAnswer>();
    
    public virtual ICollection<QuestionAnswerDto> QuestionAnswers { get; set; } = new List<QuestionAnswerDto>();
}