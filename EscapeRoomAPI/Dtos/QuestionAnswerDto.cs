namespace EscapeRoomAPI.Dtos;

public class QuestionAnswerDto
{
    public int Id { get; set; }

    public string QuestionAnswerId { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public string QuestionId { get; set; } = string.Empty;
    
    public bool IsTrue { get; set; }

    // public virtual ICollection<PlayerGameAnswer> PlayerGameAnswers { get; set; } = new List<PlayerGameAnswer>();

}