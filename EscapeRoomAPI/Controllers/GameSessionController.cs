using AutoMapper;
using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Payloads;
using EscapeRoomAPI.Payloads.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EscapeRoomAPI.Payloads.APIRoutes;

namespace EscapeRoomAPI.Controllers;

[ApiController]
public class GameSessionController : ControllerBase
{
    private readonly EscapeRoomUnityContext _context;
    private readonly IMapper _mapper;

    public GameSessionController(EscapeRoomUnityContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    //  Summary:
    //      This function use to access private session only    
    [HttpGet(APIRoutes.GameSessions.JoinByCode, Name = nameof(JoinByCodeAsync))]
    public async Task<IActionResult> JoinByCodeAsync([FromQuery] string sessionCode,
        [FromQuery] string username)
    {
        // Check exist player 
        var player = await _context.Players.FirstOrDefaultAsync(x => x.Username.Equals(username));
        if (player is null)
            return NotFound(
                new BaseResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Không tìm thấy player {username}",
                    IsSuccess = false
                });

        // Check player already exist in any game session
        var isplayerJoinedGame =
            await _context.GameSessions
                .AnyAsync(gs => gs.PlayerGameSessions.Any(p => p.PlayerId.Equals(player.PlayerId)));

        if (isplayerJoinedGame) // player is already joined some game
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Bạn không được phép tạo phòng mới, vui lòng thoát khỏi phòng cũ",
                IsSuccess = false
            });
        }

        // Check exist game session by code
        var gameSession = await _context.GameSessions
            // Include player game-sessions serve for loading waiting room
            .Include(x => x.PlayerGameSessions)
            // Retrieve player info 
            .ThenInclude(x => x.Player)
            // With condition: same session code
            .FirstOrDefaultAsync(x => x.SessionCode.Equals(sessionCode));

        // Not exist game session
        if (gameSession is null)
        {
            return NotFound(new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Không tìm thấy phòng có mã {sessionCode}"
            });
        }

        // Add player to game session
        gameSession.PlayerGameSessions.Add(new PlayerGameSession()
        {
            PlayerId = player.PlayerId,
            SessionId = gameSession.SessionId,
            IsReady = false,
            IsHost = false,
        });
        // Save to DB
        _context.Entry(gameSession).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // Retrieve game session successfully
        return Ok(new BaseResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Message = "Tìm thấy phòng thành công",
            Data = _mapper.Map<GameSessionDto>(gameSession)
        });
    }

    //  Summary:
    //      This function handling click join game from list of session
    [HttpGet(APIRoutes.GameSessions.JoinGameSession, Name = nameof(JoinGameSessionAsync))]
    public async Task<IActionResult> JoinGameSessionAsync([FromRoute] int sessionId,
        [FromQuery] string username)
    {
        // Check exist player 
        var player = await _context.Players.FirstOrDefaultAsync(x => x.Username.Equals(username));
        if (player is null)
            return NotFound(
                new BaseResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Không tìm thấy player {username}",
                    IsSuccess = false
                });

        // Check player already exist in any game session
        var isplayerJoinedGame =
            await _context.GameSessions
                .AnyAsync(gs => gs.PlayerGameSessions.Any(p => p.PlayerId.Equals(player.PlayerId)));

        if (isplayerJoinedGame) // player is already joined some game
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Bạn không được phép tạo phòng mới, vui lòng thoát khỏi phòng cũ",
                IsSuccess = false
            });
        }

        // Check exist game session by code
        var gameSession = await _context.GameSessions
            // Include player game-sessions serve for loading waiting room
            .Include(x => x.PlayerGameSessions)
            // Retrieve player info 
            .ThenInclude(x => x.Player)
            // With condition: same session code
            .FirstOrDefaultAsync(x => x.SessionId == sessionId);

        // Not exist game session
        if (gameSession is null)
        {
            return NotFound(new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Tham gia phòng chơi thất bại"
            });
        }
        else if (gameSession.IsPublic == false)
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Bạn không thể tham gia phòng chơi này"
            });
        }

        // Add player to game session
        gameSession.PlayerGameSessions.Add(new PlayerGameSession()
        {
            PlayerId = player.PlayerId,
            SessionId = gameSession.SessionId,
            IsReady = false,
            IsHost = false,
        });

        // Save to DB
        _context.Entry(gameSession).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        // Retrieve game session successfully
        return Ok(new BaseResponse
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Tham gia phòng chơi thành công",
            Data = _mapper.Map<GameSessionDto>(gameSession)
        });
    }

    //  Summary:
    //      This function use to get all existing game session (public only)
    [HttpGet(APIRoutes.GameSessions.RetrieveList, Name = nameof(RetrieveListAsync))]
    public async Task<IActionResult> RetrieveListAsync()
    {
        var gameSessions = await _context.GameSessions
            // Include game session to count total player already joined room
            .Include(x => x.PlayerGameSessions)
            //.ThenInclude(x => x.Player) -> It's unnecessary to get player
            // With conditions: game is not end yet, still waiting for other players and is public
            .Where(x => !x.IsEnd && x.IsWaiting && x.IsPublic == true).ToListAsync();

        // Not found any game session
        if (!gameSessions.Any())
        {
            return NotFound(new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Không tìm thấy phòng chơi"
            });
        }

        // Retrieve collection of game sessions successfully
        return Ok(new BaseResponse
        {
            IsSuccess = true,
            StatusCode = StatusCodes.Status200OK,
            Message = "Tìm thấy phòng thành công",
            Data = _mapper.Map<List<GameSessionDto>>(gameSessions)
        });
    }
}