using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Enums;

namespace EscapeRoomAPI.Payloads.Requests;

public class CreateQuestionRequest
{
    public string QuestionDesc { get; set; } = string.Empty;
    public IFormFile? Image { get; set; } = null!;
    public string[] Answers { get; set; } = null!;
    public string TrueAnswer { get; set; } = string.Empty;
    public bool IsHard { get; set; }
    public int KeyDigit { get; set; }
}

public static class CreateQuestionRequestExtension
{
    public static Question ToQuestionEntity(this CreateQuestionRequest reqObj)
    {
        // Initiate list of answers
        List<QuestionAnswer> answers = new();

        foreach(var answer in reqObj.Answers)
        {
            // Answer DTO
            var answerDto = new QuestionAnswer()
            {
                QuestionAnswerId = Guid.NewGuid().ToString(),
                Answer = answer,
                IsTrue = false
            };

            // Check if answer is true
            if (answer.Equals(reqObj.TrueAnswer))
            {
                answerDto.IsTrue = true;
            }

            // Add to list of answers 
            answers.Add(answerDto);
        }

        return new Question()
        {
            QuestionDesc = reqObj.QuestionDesc,
            QuestionAnswers = answers,
            KeyDigit = reqObj.KeyDigit,
            IsHard = reqObj.IsHard
        };
    }
}