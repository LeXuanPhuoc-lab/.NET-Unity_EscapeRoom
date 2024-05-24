using AutoMapper;
using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Extensions;
using EscapeRoomAPI.Payloads;
using EscapeRoomAPI.Payloads.Requests;
using EscapeRoomAPI.Payloads.Responses;
using EscapeRoomAPI.Services;
using EscapeRoomAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EscapeRoomAPI.Controllers;

[ApiController]
public class QuestionController : ControllerBase
{
    private readonly EscapeRoomUnityContext _context;
    private readonly IMapper _mapper;
    private readonly FirebaseCredentials _fbCredentails;
    private readonly AppSettings _appSettings;
    private readonly IFirebaseService _firebaseService;
    private readonly IServiceProvider _serviceProvider;

    public QuestionController(EscapeRoomUnityContext context,
        IMapper mapper,
        IOptionsMonitor<FirebaseCredentials> fbMonitor,
        IOptionsMonitor<AppSettings> asMonitor,
        IFirebaseService firebaseService,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _mapper = mapper;
        _fbCredentails = fbMonitor.CurrentValue;
        _appSettings = asMonitor.CurrentValue;
        _firebaseService = firebaseService;
        _serviceProvider = serviceProvider;
    }

    //  Summary:
    //      This API use in Postman only
    [HttpPost(APIRoutes.Questions.CreateQuestion, Name = nameof(CreateQuestionAsync))]
    public async Task<IActionResult> CreateQuestionAsync([FromForm] CreateQuestionRequest reqObj)
    {
        var imageUrl = string.Empty;
        // Image file validation (if any)
        if (reqObj.Image is not null)
        {
            var validationResult = await reqObj.Image.ValidateAsync(_serviceProvider);
            if (validationResult is not null)
            {
                return BadRequest(new BaseResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Có lỗi xảy ra",
                    Errors = validationResult.Errors
                });
            }
            else // Handling upload image
            {
                // Default firebase directory
                var directory = _appSettings.FirebaseDirectory.TrimEnd('/');
                // Get file extension
                var extension = Path.GetExtension(reqObj.Image.FileName).TrimStart('.');
                // Initiate filename with typeof(Guid)
                var fileName = $"{Guid.NewGuid()}.{extension}";

                // Upload to firebase
                imageUrl = await _firebaseService.UploadItemAsync(reqObj.Image,
                    _appSettings.FirebaseDirectory, fileName);
            }
        }

        // To Question Entity
        var questionEntity = reqObj.ToQuestionEntity();

        // Assign image URL (if any)
        if (!String.IsNullOrEmpty(imageUrl))
        {
            questionEntity.Image = imageUrl;
        }

        // Save to DB
        await _context.AddAsync(questionEntity);
        var result = await _context.SaveChangesAsync() > 0;

        return result
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Tạo câu hỏi thành công",
                Data = _mapper.Map<QuestionDto>(questionEntity)
            })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }

    // Summary:
    //      This API use in Postman only
    [HttpGet(APIRoutes.Questions.GetAllQuestion, Name = nameof(GetAllQuestionAsync))]
    public async Task<IActionResult> GetAllQuestionAsync()
    {
        var questions = await _context.Questions
            .Include(x => x.QuestionAnswers).ToListAsync();

        if (questions.Any())
        {
            return Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Data = _mapper.Map<List<QuestionDto>>(questions)
            });
        }

        return NotFound(new BaseResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Không tìm thấy câu hỏi nào"
        });
    }

    [HttpGet(APIRoutes.Questions.RetrieveQuestionNormalLevel, Name = nameof(RetrieveQuestionNormalLevelAsync))]
    public async Task<IActionResult> RetrieveQuestionNormalLevelAsync(string username)
    {
        // Check exist player 
        var player = await _context.Players.FirstOrDefaultAsync(x => x.Username.Equals(username));
        if (player is null) return NotFound(
            new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Không tìm thấy player {username}",
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
        if (playerGameSession.Session.IsEnd)
        {
            // Here in unity script we show up leader board for all player in room
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Trò chơi đã kết thúc"
            });
        }

        // Get all player answers question in the game session
        var questionAnswerIds = await _context.PlayerGameAnswers
                .Where(x => x.SessionId == playerGameSession.SessionId
                    && x.PlayerId == playerGameSession.PlayerId)
                .Select(x => x.QuestionId)
                .ToListAsync();

        // If player already answer correct 4 question -> not allow more
        if(questionAnswerIds.Count == 4)
        {
            // Here in unity script we show up leader board for all player in room
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Bạn đã trả lời thành công 4 chướng ngại vật, hãy nhập mật khẩu để mở khóa cửa"
            });
        }

        // Initialize random value 
        var random = new Random();

        // Count the number of normal question
        var count = _context.Questions
            .Where(q => !q.IsHard
                && !questionAnswerIds.Contains(q.QuestionId))
            .Count(q => !q.IsHard);
        var index = random.Next(count);

        // Retrieve random normal question
        var question = await _context.Questions
            .Include(x => x.QuestionAnswers)
            .Where(q => !q.IsHard
                && !questionAnswerIds.Contains(q.QuestionId)) // Get a question not exist in already answer questions
            .Skip(index)
            .FirstOrDefaultAsync();

        return question is not null
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Data = _mapper.Map<QuestionDto>(question)
            })
            : NotFound(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Không tìm thấy câu hỏi nào"
            });
    }

    [HttpGet(APIRoutes.Questions.RetrieveQuestionHardLevel, Name = nameof(RetrieveQuestionHardLevelAsync))]
    public async Task<IActionResult> RetrieveQuestionHardLevelAsync(string username)
    {
        // Check exist player 
        var player = await _context.Players.FirstOrDefaultAsync(x => x.Username.Equals(username));
        if (player is null) return NotFound(
            new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Không tìm thấy player {username}",
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
        if (playerGameSession.Session.IsEnd)
        {
            // Here in unity script we show up leader board for all player in room
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Trò chơi đã kết thúc"
            });
        }

        // Get all player answers question in the game session
        var questionAnswerIds = await _context.PlayerGameAnswers
                .Where(x => x.SessionId == playerGameSession.SessionId
                    && x.PlayerId == playerGameSession.PlayerId)
                .Select(x => x.QuestionId)
                .ToListAsync();

        // Initialize random value 
        var random = new Random();

        // Count the number of hard question
        var count = _context.Questions
            .Where(q => q.IsHard
                && !questionAnswerIds.Contains(q.QuestionId))
            .Count(q => q.IsHard);
        var index = random.Next(count);

        // Retrieve random hard question
        var question = await _context.Questions
            .Include(x => x.QuestionAnswers)
            .Where(q => q.IsHard
                && !questionAnswerIds.Contains(q.QuestionId)) // Get a question not exist in already answer questions
            .Skip(index)
            .FirstOrDefaultAsync();

        return question is not null
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Data = _mapper.Map<QuestionDto>(question)
            })
            : NotFound(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Không tìm thấy câu hỏi nào"
            });
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
        if (playerGameSession.Session.IsEnd)
        {
            // Here in unity script we show up leader board for all player in room
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Trò chơi đã kết thúc"
            });
        }

        // Check if player already answer question
        var playerAnswer = await _context.PlayerGameAnswers
            .Where(x => x.PlayerId == playerGameSession.PlayerId
                && x.QuestionId == reqObj.QuestionId)
            .FirstOrDefaultAsync();

        if (playerAnswer is not null)
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Có lỗi xảy ra, người chơi đã trả lời câu hỏi này"
            });

        // Player game answer
        var playerGameAnswerDto = reqObj.ToPlayerGameAnswerDto();
        // Assign game session
        playerGameAnswerDto.SessionId = playerGameSession.SessionId;
        // Assign who answer
        playerGameAnswerDto.PlayerId = playerGameSession.PlayerId;

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

            // Save player answer (just correct only)
            await _context.PlayerGameAnswers.AddAsync(_mapper.Map<PlayerGameAnswer>(playerGameAnswerDto));
            await _context.SaveChangesAsync();
        }

        // Get question 
        var question = await _context.Questions.FirstOrDefaultAsync(x => x.QuestionId == playerGameAnswerDto.QuestionId);

        // Create the response based on the correctness of the answer
        var response = new BaseResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = playerGameAnswerDto.IsCorrect!.Value
                ? "Chúc mừng bạn, đáp án chính xác"
                : "Đáp án không chính xác",
            Data = playerGameAnswerDto.IsCorrect.Value
                ? new { KeyDigit = question!.KeyDigit }
                : null!
        };

        return Ok(response);
    }
}