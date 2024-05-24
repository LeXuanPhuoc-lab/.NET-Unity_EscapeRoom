using AutoMapper;
using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Payloads;
using EscapeRoomAPI.Payloads.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscapeRoomAPI.Controllers;

[ApiController]
public class LeaderboardController : ControllerBase
{

    private readonly EscapeRoomUnityContext _context;
    private readonly IMapper _mapper;

    public LeaderboardController(EscapeRoomUnityContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet(APIRoutes.Leaderboard.ShowLeaderboard, Name = nameof(ShowLeaderboardAsync))]
    public async Task<IActionResult> ShowLeaderboardAsync([FromQuery] string username)
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
                Message = "Không tìm thấy người chơi trong phòng"
            });
        }

        // Check if leaderboard already create
        var existingLeaderboard = await _context.Leaderboards
                .Include(x => x.Player)
                .Where(x => x.SessionId == playerGameSession.SessionId) 
                .ToListAsync();

        if (existingLeaderboard.Any())
        {
            return Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Tạo leaderboard thành công",
                Data = _mapper.Map<List<LeaderboardDto>>(existingLeaderboard)
            });
        }


        // Get all player in game session
        var players = await _context.PlayerGameSessions
            .Include(x => x.Player)
                .ThenInclude(x => x.PlayerGameAnswers)
            .Where(x => x.SessionId == playerGameSession.SessionId)
            .Select(x => x.Player)
            .ToListAsync();


        var leaderBoards = new List<Leaderboard>();
        // Foreach player, count their correct answer
        foreach (var p in players)
        {
            // Total correct answers
            var listofCorrectAnswers = await _context.PlayerGameAnswers
                .Where(x => x.PlayerId == p.PlayerId
                    && x.IsCorrect)
                .ToListAsync();

            leaderBoards.Add(new Leaderboard
            {
                Player = p,
                SessionId = playerGameSession.SessionId,
                TotalRightAnswer = listofCorrectAnswers.Count,
            });
        }


        // Sorting rank - descending order
        leaderBoards.Sort((x, y) => y.TotalRightAnswer.CompareTo(x.TotalRightAnswer));

        // Save To DB
        await _context.Leaderboards.AddRangeAsync(leaderBoards);
        await _context.SaveChangesAsync();

        return leaderBoards.Any()
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Tạo leaderboard thành công",
                Data = _mapper.Map<List<LeaderboardDto>>(leaderBoards)
            })
            : Problem("Có lỗi xảy ra, không thể tạo leaderboard", null, StatusCodes.Status500InternalServerError);
    }
}