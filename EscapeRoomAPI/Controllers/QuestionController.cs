using AutoMapper;
using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Payloads;
using EscapeRoomAPI.Payloads.Requests;
using EscapeRoomAPI.Payloads.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscapeRoomAPI.Controllers;

[ApiController]
public class QuestionController : ControllerBase
{
    private readonly EscapeRoomUnityContext _context;
    private readonly IMapper _mapper;

    public QuestionController(EscapeRoomUnityContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet(APIRoutes.Questions.RetrieveQuestionNormalLevel, Name = nameof(RetrieveQuestionNormalLevelAsync))]
    public async Task<IActionResult> RetrieveQuestionNormalLevelAsync()
    {
        // Initialize random value 
        var random = new Random();

        // Count the number of normal question
        var count = _context.Questions.Count(q => q.IsHard.HasValue && !q.IsHard.Value);
        var index = random.Next(count);

        // Retrieve random normal question
        var question = await _context.Questions
            .Include(x => x.QuestionAnswers)
            .Where(q => q.IsHard.HasValue && !q.IsHard.Value)
            .Skip(index)
            .FirstOrDefaultAsync();

        return question is not null
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Data = _mapper.Map<QuestionDto>(question)
            })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }

    [HttpGet(APIRoutes.Questions.RetrieveQuestionHardLevel, Name = nameof(RetrieveQuestionHardLevelAsync))]
    public async Task<IActionResult> RetrieveQuestionHardLevelAsync()
    {
        // Initialize random value 
        var random = new Random();

        // Count the number of normal question
        var count = _context.Questions.Count(q => q.IsHard.HasValue && q.IsHard.Value);
        var index = random.Next(count);

        // Retrieve random normal question
        var question = await _context.Questions
            .Include(x => x.QuestionAnswers)
            .Where(q => q.IsHard.HasValue && q.IsHard.Value)
            .Skip(index)
            .FirstOrDefaultAsync();

        return question is not null
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Data = _mapper.Map<QuestionDto>(question)
            })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }

    [HttpPost(APIRoutes.Questions.SubmitAnswer, Name = nameof(SubmitAnswerAsync))]
    public async Task<IActionResult> SubmitAnswerAsync([FromBody] SubmitAnswerRequest reqObj)
    {
        // Check exist player 
        var player = await _context.Players.FirstOrDefaultAsync(x => x.Username.Equals(reqObj.Username));
        if (player is null) return NotFound(
            new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Không tìm thấy player {reqObj.Username}",
                IsSuccess = false
            });

        // Get playing game session
        var playerGameSession = await _context.PlayerGameSessions
            .Include(x => x.Session)
            .FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId);
        if (playerGameSession is null)
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Không tìm thấy người chơi"
            });
        }

        // Check game is end yet
        if (playerGameSession.Session.IsEnd.HasValue
        && playerGameSession.Session.IsEnd.Value)
        {
            // Here in unity script we show up leader board for all player in room
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Trò chơi đã kết thúc"
            });
        }

        // Player game answer
        var playerGameAnswerDto = reqObj.ToPlayerGameAnswerDto();
        // Assign game session
        playerGameAnswerDto.SessionId = playerGameAnswerDto.SessionId;
        // Assign who answer
        playerGameAnswerDto.PlayerId = playerGameAnswerDto.PlayerId;

        // Check is correct or not 
        // Retrieve list of question answer
        var correctAnswer = await _context.QuestionAnswers
            .Where(x => x.QuestionId == playerGameAnswerDto.QuestionId
                && x.IsTrue)
            .FirstOrDefaultAsync();
        if (correctAnswer is null)
            return Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);

        // If selected answer eq to correct answer
        if (playerGameAnswerDto.SelectAnswerId == correctAnswer.QuestionAnswerId)
        {
            // Assign is player answer is correct 
            playerGameAnswerDto.IsCorrect = true;
        }

        // Save player answer serving for leaderboard
        await _context.PlayerGameAnswers.AddAsync(_mapper.Map<PlayerGameAnswer>(playerGameAnswerDto));
        await _context.SaveChangesAsync();

        // Create the response based on the correctness of the answer
        var response = new BaseResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = playerGameAnswerDto.IsCorrect!.Value
                ? "Chúc mừng bạn, đáp án chính xác"
                : "Đáp án không chính xác",
            Data = playerGameAnswerDto.IsCorrect.Value
        };

        return Ok(response);
    }
}