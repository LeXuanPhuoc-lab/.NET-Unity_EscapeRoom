using AutoMapper;
using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Extensions;
using EscapeRoomAPI.Payloads;
using EscapeRoomAPI.Payloads.Requests;
using EscapeRoomAPI.Payloads.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscapeRoomAPI.Controllers;

[ApiController]
public class PlayerController : ControllerBase
{
    private readonly EscapeRoomUnityContext _context;
    private readonly IMapper _mapper;

    public PlayerController(EscapeRoomUnityContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost(APIRoutes.Players.CreateRoom, Name = nameof(CreateRoomAsync))]
    public async Task<IActionResult> CreateRoomAsync([FromBody] CreateRoomRequest reqObj)
    {
        // Process register validation 
        var validationResult = await reqObj.ValidateAsync();
        if (validationResult is not null) // Invoke errors
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                IsSuccess = false,
                Errors = validationResult.Errors
            });
        }

        // Check exist player 
        var player = await _context.Players.FirstOrDefaultAsync(x => x.Username.Equals(reqObj.Username));
        if (player is null) return NotFound(
            new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = $"Không tìm thấy player {reqObj.Username}",
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

        // Create game session
        var gameSessionEntity = _mapper.Map<GameSession>(reqObj.ToGameSessionDto());
        // Add user to game session
        gameSessionEntity.PlayerGameSessions.Add(new PlayerGameSession
        {
            Player = player,
            Session = gameSessionEntity,
            IsHost = true
        });

        // Save to DB
        await _context.GameSessions.AddAsync(gameSessionEntity);
        var result = await _context.SaveChangesAsync() > 0;

        return result
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Tạo phòng mới thành công",
                Data = _mapper.Map<GameSessionDto>(gameSessionEntity),
                IsSuccess = true
            })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }

    [HttpGet(APIRoutes.Players.FindRoom, Name = nameof(FindRoomAsync))]
    public async Task<IActionResult> FindRoomAsync([FromRoute] string username)
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
        
        // Check player already exist in any game session
        var isplayerJoinedGame =
            await _context.GameSessions
                .AnyAsync(gs => gs.PlayerGameSessions.Any(p => p.PlayerId.Equals(player.PlayerId)));

        if (isplayerJoinedGame) // player is already joined some game
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Vui lòng thoát phòng hiện tại để tìm phòng mới",
                IsSuccess = false
            });
        }
        
        // Get game session
        var gameSession = await _context.GameSessions
            .Include(gs => gs.PlayerGameSessions)
            .FirstOrDefaultAsync(gs =>
                gs.IsWaiting.HasValue && gs.IsWaiting.Value  // Game session not started yet
             && gs.PlayerGameSessions.Count < gs.TotalPlayer); // Not full (less than total player || enough for one more)

        if (gameSession is null) return NotFound(
            new BaseResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Hiện tại không còn phòng trống",
                IsSuccess = false
            });

        // Add user to game session as member
        gameSession.PlayerGameSessions.Add(new PlayerGameSession
        {
            Player = player,
            IsHost = false
        });
        // Save to DB
        var result = await _context.SaveChangesAsync() > 0;

        return result
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Tìm thấy phòng thành công",
                Data = _mapper.Map<GameSessionDto>(gameSession),
                IsSuccess = true
            })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }

    [HttpDelete(APIRoutes.Players.OutRoom, Name = nameof(OutRoomAsync))]
    public async Task<IActionResult> OutRoomAsync([FromRoute] string username)
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

        // Proccess out room request
        // Get player game session
        var playerGameSession = await _context.PlayerGameSessions
            .FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId);
        if (playerGameSession is null) return BadRequest(new BaseResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "Hiện tại bạn đang không phòng chơi",
            IsSuccess = false
        });

        // Remove player game session
        if (_context.PlayerGameSessions.Entry(playerGameSession).State == EntityState.Detached)
        {
            // Ensure that entity is being track by context 
            _context.Attach(playerGameSession); // Change entity's state to "Deleted"
        }
        _context.Remove(playerGameSession);

        // Check if player is host 
        if (playerGameSession.IsHost.HasValue
         && playerGameSession.IsHost.Value)
        {
            // Get game session
            var gameSessionToDelete = await _context.GameSessions.FirstOrDefaultAsync(x =>
                x.SessionId == playerGameSession.SessionId);

            if (gameSessionToDelete is null)
                return Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);

            // Remove game session
            // Provide access to tracking information and operation
            if (_context.GameSessions.Entry(gameSessionToDelete).State == EntityState.Detached)
            {
                // Ensure that entity is being track by context 
                _context.Attach(gameSessionToDelete); // Change entity's state to "Deleted"
            }
            _context.Remove(gameSessionToDelete);
        }

        // Save DB change 
        var result = await _context.SaveChangesAsync() > 0;

        if (result) // Process successfully
            Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Thoát thành công"
            });

        return Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }

    [HttpPatch(APIRoutes.Players.ModifyReady, Name = nameof(ModifyReadyAsync))]
    public async Task<IActionResult> ModifyReadyAsync([FromRoute] string username)
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

        // Already in a room
        var playerGameSession = await _context.PlayerGameSessions
            .Include(x => x.Player)
            .Include(x => x.Session)
            .FirstOrDefaultAsync(x =>
                x.PlayerId == player.PlayerId);
        if (playerGameSession is null) return BadRequest(
            new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"Player không thể ready khi chưa tham gia phòng chơi",
                IsSuccess = false
            });

        // Modify ready status
        playerGameSession.IsReady = !playerGameSession.IsReady;
        // Save change
        var result = await _context.SaveChangesAsync() > 0;

        return result
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Đổi trạng thái thành công",
                Data = _mapper.Map<PlayerGameSessionDto>(playerGameSession),
                IsSuccess = true
            })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }

    [HttpPatch(APIRoutes.Players.StartRoom, Name = nameof(StartRoomAsync))]
    public async Task<IActionResult> StartRoomAsync([FromRoute] string username)
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

        // Already in a room
        var playerGameSession = await _context.PlayerGameSessions
            .Include(x => x.Session)
            .FirstOrDefaultAsync(x =>
                x.PlayerId == player.PlayerId);
        if (playerGameSession is null) return BadRequest(
            new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"Player không thể bắt đầu màn chơi khi chưa tham gia phòng chơi",
                IsSuccess = false
            });

        // Check if player is host
        if (playerGameSession.IsHost.HasValue && playerGameSession.IsHost.Value)
        {
            // Process starting game...

            // Check if all players in room was ready
            var listPlayerSession = await _context.PlayerGameSessions
                .Include(x => x.Player)
                .Where(x => x.SessionId == playerGameSession.SessionId)
                .ToListAsync();

            foreach (var ps in listPlayerSession)
            {
                if (ps.IsReady.HasValue && !ps.IsReady.Value)
                    return BadRequest(new BaseResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Còn người chơi chưa sẵn sàng, vui lòng chờ để bắt đầu"
                    });
            }

            // When ensuring player is host && other players is ready
            // Start game 
            if (_context.Entry(playerGameSession.Session).State == EntityState.Detached)
            {
                _context.Attach(playerGameSession.Session);
            }
            _context.Entry(playerGameSession.Session).Property(s => s.IsWaiting).CurrentValue = false;
            _context.Entry(playerGameSession.Session).Property(s => s.IsWaiting).IsModified = true;
        }

        // Save change
        var result = await _context.SaveChangesAsync() > 0;

        return result
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Trò chơi đã bắt đầu, bạn còn {playerGameSession.Session.EndTime.TotalMinutes} phút để thoát khỏi nơi này",
                IsSuccess = true
            })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }
}