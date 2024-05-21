using EscapeRoomAPI.Dtos;

namespace EscapeRoomAPI.Payloads.Requests;

public class SubmitAnswerRequest
{
    public string Username { get; set; } = string.Empty;
    public string QuestionId { get; set; } = string.Empty;
    public string SelectAnswerId { get; set; } = string.Empty;
}

public static class SubmitAnswerRequestExtension
{
    public static PlayerGameAnswerDto ToPlayerGameAnswerDto(this SubmitAnswerRequest reqObj)
    {
        return new PlayerGameAnswerDto
        {
            QuestionId = reqObj.QuestionId,
            SelectAnswerId = reqObj.SelectAnswerId,
            IsCorrect = false
        };
    }
}