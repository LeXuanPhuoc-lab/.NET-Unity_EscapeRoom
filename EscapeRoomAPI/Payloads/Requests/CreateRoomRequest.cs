using EscapeRoomAPI.Dtos;

namespace EscapeRoomAPI.Payloads.Requests;

public class CreateRoomRequest
{
    public string Username { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public int TotalPlayer { get; set; }
    public int EndTimeToMinute { get; set; }
}

public static class CreateRoomResponseExtension
{
    public static GameSessionDto ToGameSessionDto(this CreateRoomRequest reqObj)
    {
        return new GameSessionDto
        {
            SessionName = reqObj.RoomName,
            IsEnd = false,
            IsWaiting = true,
            TotalPlayer = reqObj.TotalPlayer,
            StartTime = TimeSpan.Zero,
            EndTime = TimeSpan.FromMinutes(reqObj.EndTimeToMinute)
        };
    }
}