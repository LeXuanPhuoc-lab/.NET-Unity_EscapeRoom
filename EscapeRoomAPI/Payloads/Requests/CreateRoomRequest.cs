using EscapeRoomAPI.Dtos;
using EscapeRoomAPI.Enums;

namespace EscapeRoomAPI.Payloads.Requests;

public class CreateRoomRequest
{
    public string Username { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public int TotalPlayer { get; set; }
    public int EndTimeToMinute { get; set; }
    public bool IsPublic { get; set; } = true;
}

public static class CreateRoomResponseExtension
{
    public static GameSessionDto ToGameSessionDto(this CreateRoomRequest reqObj)
    {
        var random = new Random();
        var index = random.Next(0, 100);

        return new GameSessionDto
        {
            SessionName = reqObj.RoomName,
            IsEnd = false,
            IsWaiting = true,
            IsPublic = reqObj.IsPublic,
            TotalPlayer = reqObj.TotalPlayer,
            StartTime = TimeSpan.Zero,
            EndTime = TimeSpan.FromMinutes(reqObj.EndTimeToMinute),
            Hint = (index % 2 == 0) ? nameof(UnclockHint.Ascending) : nameof(UnclockHint.Descending)
        };
    }
}