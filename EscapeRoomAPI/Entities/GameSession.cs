﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EscapeRoomAPI.Entities;

public partial class GameSession
{
    public int SessionId { get; set; }

    public string SessionName { get; set; }

    public string SessionCode { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public int TotalPlayer { get; set; }

    public bool IsWaiting { get; set; }

    public bool IsEnd { get; set; }

    public bool IsPublic { get; set; }

    public string Hint { get; set; }

    public virtual ICollection<Leaderboard> Leaderboards { get; set; } = new List<Leaderboard>();

    public virtual ICollection<PlayerGameAnswer> PlayerGameAnswers { get; set; } = new List<PlayerGameAnswer>();

    public virtual ICollection<PlayerGameSession> PlayerGameSessions { get; set; } = new List<PlayerGameSession>();
}