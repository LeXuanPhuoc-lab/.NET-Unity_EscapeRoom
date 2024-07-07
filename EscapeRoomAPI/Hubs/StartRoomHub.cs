using System.Text.Json;
using AutoMapper;
using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Entities;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EscapeRoomAPI.Hubs;

public class StartRoomHub : Hub
{
    private readonly EscapeRoomUnityContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpClientFactory _httpClientFactory;
    private static HttpClient _httpClient;

    public StartRoomHub(EscapeRoomUnityContext context,
        IMapper mapper,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _mapper = mapper;
        _httpClientFactory = httpClientFactory;
        _httpClient = _httpClientFactory.CreateClient("Unity");
    }


    public override Task OnConnectedAsync()
    {
        Clients.All.SendAsync("InvokeConnectionMessage", "Connected to server successfully");
        return base.OnConnectedAsync();
    }

    public async Task InvokeStartAsync(string username)
    {
        // Get player by username
        var player = await _context.Players.FirstOrDefaultAsync(x =>
            x.Username.Equals(username));

        if (player is null) return;

        // Get player game session
        var playerGameSession = await
            _context.PlayerGameSessions
                        .Include(x => x.Session)
                    .FirstOrDefaultAsync(x => x.PlayerId.Equals(player.PlayerId));

        if (playerGameSession is null) return;

        // Process edit game session status
        if (_context.Entry(playerGameSession.Session).State == EntityState.Detached)
        {
            _context.Attach(playerGameSession.Session);
        }

        _context.Entry(playerGameSession.Session).Property(x => x.IsWaiting).CurrentValue = false;
        _context.Entry(playerGameSession.Session).Property(x => x.IsWaiting).IsModified = true;

        var result = await _context.SaveChangesAsync() > 0;

        if (result) await Clients.All.SendAsync("OnStartingProcessed",
            true,
            playerGameSession.Session.EndTime.TotalSeconds,
            playerGameSession.SessionId);
    }

    public async Task InvokeFindOrReadyOrExistAsync(string username)
    {
        // Get player by username
        var player = await _context.Players.FirstOrDefaultAsync(x =>
            x.Username.Equals(username));

        if (player is null) return;


        // Check if user already in any game session
        var playerGameSession = await _context.PlayerGameSessions
                .Include(x => x.Session)
                .FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId);

        if (playerGameSession is null) return;

        // Total player game in session
        var totalPlayerInSession = await _context.PlayerGameSessions.CountAsync(x =>
            x.SessionId == playerGameSession.SessionId);
        // Total session players
        var sessionPlayerCap = playerGameSession.Session.TotalPlayer;
        // Total ready players
        var totalReadyPlayers = await _context.PlayerGameSessions.CountAsync(x =>
            x.SessionId == playerGameSession.SessionId && x.IsReady);
        // Game session id
        var gameSessionId = playerGameSession.SessionId;

        if (totalPlayerInSession > 0)
            await Clients.All.SendAsync("OnTriggerInWaitingRoomProcessed",
                totalPlayerInSession, sessionPlayerCap, totalReadyPlayers, gameSessionId);
    }

    public async Task InvokeLeaderBoard(string username)
    {
        // Get player by username
        var player = await _context.Players.FirstOrDefaultAsync(x =>
            x.Username.Equals(username));

        if (player is null) return;


        // Check if user already in any game session
        var playerGameSession = await _context.PlayerGameSessions
                .Include(x => x.Session)
                .FirstOrDefaultAsync(x => x.PlayerId == player.PlayerId);

        if (playerGameSession is null) return;

        var message = "Show LeaderBoard for all users";
        await Clients.All.SendAsync("ShowLeaderBoard", message);
    }

    public async Task InvokeRoomCreateAsync(string username)
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
            return;
        }

        var gameSessionDtos = _mapper.Map<List<GameSessionDto>>(gameSessions);
        await Clients.All.SendAsync("OnTriggerCreateRoom", JsonSerializer.Serialize(gameSessionDtos));
    }
}