using EscapeRoomAPI.Entities;

namespace EscapeRoomAPI.Dtos
{
    public class LeaderboardDto
    {
        public int LeaderBoardId { get; set; }

        public int SessionId { get; set; }

        public string PlayerId { get; set; } = string.Empty;    

        public int TotalRightAnswer { get; set; }

        public virtual PlayerDto Player { get; set; } = null!;
    }
}
