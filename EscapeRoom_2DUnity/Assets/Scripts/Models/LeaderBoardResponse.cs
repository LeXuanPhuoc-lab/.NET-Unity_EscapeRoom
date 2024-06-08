using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class LeaderBoardResponse
    {


        public int LeaderBoardId { get; set; }
        public int SessionId { get; set; }
        public string PlayerId { get; set; }

        public int TotalRightAnswer { get; set; }

        public PlayerResponse Player {  get; set; }     
    
    }
}
