using AutoMapper;
using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;
using EscapeRoomAPI.Enums;
using EscapeRoomAPI.Extensions;
using EscapeRoomAPI.Payloads;
using EscapeRoomAPI.Payloads.Requests;
using EscapeRoomAPI.Payloads.Responses;
using EscapeRoomAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

namespace EscapeRoomAPI.Controllers;

[ApiController]
public class PlayerController : ControllerBase
{
    private readonly EscapeRoomUnityContext _context;
    private readonly IMapper _mapper;
    private readonly IServiceProvider _serviceProvider;

    public PlayerController(EscapeRoomUnityContext context,
        IMapper mapper,
        IServiceProvider serviceProvider)
    {
        _context = context;
        _mapper = mapper;
        _serviceProvider = serviceProvider;
    }

    [HttpPost(APIRoutes.Players.CreateRoom, Name = nameof(CreateRoomAsync))]
    public async Task<IActionResult> CreateRoomAsync([FromBody] CreateRoomRequest reqObj)
    {
        Console.WriteLine("client data");
        Console.WriteLine(reqObj.Username);
        Console.WriteLine(reqObj.RoomName);
        Console.WriteLine(reqObj.TotalPlayer);
        Console.WriteLine(reqObj.EndTimeToMinute);
        // Process register validation 
        var validationResult = await reqObj.ValidateAsync(_serviceProvider);
        if (validationResult is not null) // Invoke errors
        {
            Console.WriteLine(validationResult.Errors);
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                IsSuccess = false,
                Errors = validationResult.Errors
            });
        }

        // Check exist player 
        var player = await _context.Players.FirstOrDefaultAsync(x => x.Username.Equals(reqObj.Username));
        if (player is null)
            return NotFound(
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
                Message = "Vui lòng thoát phòng hiện tại để tìm phòng mới",
                IsSuccess = false
            });
        }

        // Get game session
        var gameSession = await _context.GameSessions
            .Include(gs => gs.PlayerGameSessions)
            .FirstOrDefaultAsync(gs =>
                gs.IsWaiting // Game session not started yet
                && gs.PlayerGameSessions.Count <
                gs.TotalPlayer); // Not full (less than total player || enough for one more)

        if (gameSession is null)
            return NotFound(
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
            IsHost = false,
            IsReady = false
        });
        // Save to DB
        var result = await _context.SaveChangesAsync() > 0;

        //tới đây thì PlayerGameSessions chỉ chứa một Player mới thêm, chứ ko phải chứ toàn bộ player trong gamesession, nên cần load thêm
        if (result)
        {
            // Reload the PlayerGameSessions collection
            await _context.Entry(gameSession)
                .Collection(gs => gs.PlayerGameSessions)
                .Query()
                .Include(pgs => pgs.Player) // Include Player if needed
                .LoadAsync();
        }

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
        if (player is null)
            return NotFound(
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
        if (playerGameSession is null)
            return BadRequest(new BaseResponse
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

        //// Check if player is host 
        //if (playerGameSession.IsHost)
        //{
        //    // Remove all relevant features (PlayerSession, PlayerAnswer, Leaderboard,...)
        //    // Get all player in game session
        //    var allPlayerGameSession = await _context.PlayerGameSessions
        //            .Include(x => x.Player)
        //                .ThenInclude(x => x.PlayerGameAnswers)
        //            .Where(x => x.SessionId == playerGameSession.SessionId)
        //            .ToListAsync();

        //    foreach (var pgs in allPlayerGameSession)
        //    {
        //        // Get all player game answers 
        //        var playerGameAnswers = pgs.Player.PlayerGameAnswers;

        //        // Remove player game session
        //        pgs.Player = null!;
        //        _context.Remove(pgs);

        //        // Clear all answer question 
        //        _context.RemoveRange(playerGameAnswers);
        //    }
        //}

        // Check if last player out game session -> Remove leaderboard
        var playersInGame = await _context.PlayerGameSessions
            .Where(x => x.SessionId == playerGameSession.SessionId)
            .CountAsync();
        if (playersInGame == 1)
        {
            // Remove all relevant features (PlayerSession, PlayerAnswer, Leaderboard,...)
            // Get all player answers
            var playerGameAnswers = await _context.PlayerGameAnswers
                .Where(x => x.SessionId == playerGameSession.SessionId)
                .ToListAsync();

            // Clear all answer  
            _context.RemoveRange(playerGameAnswers);

            // Remove leaderboard
            _context.RemoveRange(await _context.Leaderboards
                .Where(x => x.SessionId == playerGameSession.SessionId)
                .ToListAsync());

            // Get game session
            var gameSessionToDelete = await _context.GameSessions.FirstOrDefaultAsync(x =>
                x.SessionId == playerGameSession.SessionId);

            if (gameSessionToDelete is null)
                return Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);

            // Remove game session when last player out room
            // Provide access to tracking information and operation
            if (_context.GameSessions.Entry(gameSessionToDelete).State == EntityState.Detached)
            {
                // Ensure that entity is being track by context 
                _context.Attach(gameSessionToDelete); // Change entity's state to "Deleted"
            }

            // Remove leaderboard
            _context.Remove(gameSessionToDelete);
        }

        // Save DB change 
        var result = await _context.SaveChangesAsync() > 0;

        return result // Process successfully
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = $"Thoát thành công"
            })
            : Problem("Có lỗi xảy ra", null, StatusCodes.Status500InternalServerError);
    }

    [HttpPut(APIRoutes.Players.ModifyReady, Name = nameof(ModifyReadyAsync))]
    public async Task<IActionResult> ModifyReadyAsync([FromRoute] string username)
    {
        Console.WriteLine(username);
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

        Console.WriteLine(1);

        // Already in a room
        var playerGameSession = await _context.PlayerGameSessions
            .Include(x => x.Player)
            .Include(x => x.Session)
            .FirstOrDefaultAsync(x =>
                x.PlayerId == player.PlayerId);

        Console.WriteLine(2);
        if (playerGameSession is null)
            return BadRequest(
                new BaseResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = $"Player không thể ready khi chưa tham gia phòng chơi",
                    IsSuccess = false
                });

        Console.WriteLine(3);

        // Check if game already started or not 
        if (!playerGameSession.Session.IsWaiting)
        {
            return BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Người chơi đang trong trò chơi, không thể chuẩn bị"
            });
        }

        Console.WriteLine(4);


        // Modify ready status
        if (_context.Entry(playerGameSession).State == EntityState.Detached)
        {
            _context.Attach(playerGameSession);
        }

        _context.Entry(playerGameSession).Property(s => s.IsReady).CurrentValue = !playerGameSession.IsReady;
        _context.Entry(playerGameSession).Property(s => s.IsReady).IsModified = true;
        // Save change
        var result = await _context.SaveChangesAsync() > 0;

        Console.WriteLine(5);

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

    [HttpPut(APIRoutes.Players.StartRoom, Name = nameof(StartRoomAsync))]
    public async Task<IActionResult> StartRoomAsync([FromRoute] string username)
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

        // Already in a room
        var playerGameSession = await _context.PlayerGameSessions
            .Include(x => x.Session)
            .FirstOrDefaultAsync(x =>
                x.PlayerId == player.PlayerId);
        if (playerGameSession is null)
            return BadRequest(
                new BaseResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = $"Player không thể bắt đầu màn chơi khi chưa tham gia phòng chơi",
                    IsSuccess = false
                });

        // Check if player is host
        if (playerGameSession.IsHost)
        {
            // Process starting game...

            // Check if game already started or not 
            if (!playerGameSession.Session.IsWaiting)
            {
                return BadRequest(new BaseResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Bắt đầu trò chơi thất bại, trò chơi đang diễn ra"
                });
            }

            // Get all players in session, except host player
            var listPlayerSession = await _context.PlayerGameSessions
                .Include(x => x.Player)
                .Where(x => x.SessionId == playerGameSession.SessionId
                            && x.PlayerId != playerGameSession.PlayerId)
                .ToListAsync();

            // To start game, at least 2 players
            if (listPlayerSession.Count < 0)
            {
                return BadRequest(new BaseResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Yêu cầu ít nhất 1 người chơi để bắt đầu"
                });
            }

            // Check if all players in room was ready
            foreach (var ps in listPlayerSession)
            {
                if (!ps.IsReady)
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

        // Send message to all clients that subscribe 


        return result
            ? Ok(new BaseResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message =
                    $"Trò chơi đã bắt đầu, bạn còn {playerGameSession.Session.EndTime.TotalMinutes} phút để thoát khỏi nơi này",
                IsSuccess = true
            })
            : BadRequest(new BaseResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = $"Bắt đầu trò chơi thất bại, bạn không phải chủ phòng"
            });
    }

    [HttpGet(APIRoutes.Players.SubmitUnlockRoomKey, Name = nameof(SubmitUnlockRoomKeyAsync))]
    public async Task<IActionResult> SubmitUnlockRoomKeyAsync([FromRoute] string username, [FromRoute] int key,
        bool isHard)
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

        // Get all player correct answer
        var playerAnswers = await _context.PlayerGameAnswers
            .Include(x => x.Question)
            .Where(x => x.PlayerId == playerGameSession.PlayerId)
            .ToListAsync();

        // Count all digits, which are correct
        var digitsFromCorrect = playerAnswers.Select(x => x.Question)
            .Where(x => x.IsHard.Equals(isHard))
            .Select(x => x.KeyDigit).ToList();

        // Process unclock whenever total correct is excess than 4 digits
        if (digitsFromCorrect.Count >= 4)
        {
            // Compare with hint
            var hint = playerGameSession.Session.Hint;
            var unClockKey = NumberUtils.CombineDigitsIntoNumber(digitsFromCorrect, hint);

            var isCorrect = false;

            switch (hint)
            {
                case nameof(UnclockHint.Ascending):
                    if (NumberUtils.IsAscending(key) && key == unClockKey) isCorrect = true;
                    break;
                case nameof(UnclockHint.Descending):
                    if (NumberUtils.IsDescending(key) && key == unClockKey) isCorrect = true;
                    break;
            }

            if (isCorrect && !isHard)
            {
                return Ok(new BaseResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Mở khóa thành công",
                    IsSuccess = true
                });
            }
            else if (isCorrect && isHard)
            {
                // Check if all hard question is answer, and successfully open main door
                // Game is end
                playerGameSession.Session.IsEnd = true;

                await _context.SaveChangesAsync();

                return Ok(new BaseResponse
                {
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Trò chơi kết thúc",
                    IsSuccess = true
                });
            }
        }

        // Others situtation -> Wrong key pass -> Not allow to unclock the door 
        return BadRequest(new BaseResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Message = "Sai mật khẩu"
        });
    }
}