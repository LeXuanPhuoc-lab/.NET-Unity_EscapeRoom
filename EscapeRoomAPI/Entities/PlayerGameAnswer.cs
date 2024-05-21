﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace EscapeRoomAPI.Entities;

public partial class PlayerGameAnswer
{
    public int Id { get; set; }

    public int SessionId { get; set; }

    public string PlayerId { get; set; }

    public string QuestionId { get; set; }

    public string SelectAnswerId { get; set; }

    public bool? IsCorrect { get; set; }

    public virtual Player Player { get; set; }

    public virtual Question Question { get; set; }

    public virtual QuestionAnswer SelectAnswer { get; set; }

    public virtual GameSession Session { get; set; }
}