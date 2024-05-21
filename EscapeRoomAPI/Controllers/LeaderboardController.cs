using AutoMapper;
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

        // Get all player in game session
        var players = await _context.PlayerGameSessions
            .Where(x => x.SessionId == playerGameSession.SessionId)
            .Select(x => x.Player)
            .ToListAsync();


        // Foreach player, count their correct answer
        foreach (var p in players)
        {
            // Total correct answers
            var listofCorrectAnswers = await _context.PlayerGameAnswers
                .Where(x => x.PlayerId == p.PlayerId 
                    && x.IsCorrect.HasValue
                    && x.IsCorrect.Value)
                .ToListAsync();
            // var totalCorrect = listAns
        }

        return Ok();
    }
}