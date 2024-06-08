﻿// <auto-generated />
using System;
using EscapeRoomAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EscapeRoomAPI.Data.Migrations
{
    [DbContext(typeof(EscapeRoomUnityContext))]
    partial class EscapeRoomUnityContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EscapeRoomAPI.Entities.GameSession", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SessionId"));

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("time");

                    b.Property<string>("Hint")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsEnd")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<bool>("IsWaiting")
                        .HasColumnType("bit");

                    b.Property<string>("SessionCode")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SessionName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("time");

                    b.Property<int>("TotalPlayer")
                        .HasColumnType("int");

                    b.HasKey("SessionId")
                        .HasName("PK__GameSess__C9F492901B46D757");

                    b.ToTable("GameSession", (string)null);
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.Leaderboard", b =>
                {
                    b.Property<int>("LeaderBoardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LeaderBoardId"));

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("SessionId")
                        .HasColumnType("int");

                    b.Property<int>("TotalRightAnswer")
                        .HasColumnType("int");

                    b.HasKey("LeaderBoardId")
                        .HasName("PK__Leaderbo__91D442149723193C");

                    b.HasIndex("PlayerId");

                    b.HasIndex("SessionId");

                    b.ToTable("Leaderboard", (string)null);
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)")
                        .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");

                    b.Property<DateTime?>("RegistrationDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("Id")
                        .HasName("PK__Player__3213E83FCAD2F2D2");

                    b.HasIndex(new[] { "PlayerId" }, "UQ__Player__4A4E74C95FB65E42")
                        .IsUnique();

                    b.HasIndex(new[] { "Username" }, "UQ__Player__536C85E4025B578D")
                        .IsUnique();

                    b.ToTable("Player", (string)null);
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.PlayerGameAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("QuestionId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("SelectAnswerId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("SessionId")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("PK__PlayerGa__3213E83F4FCA4DD2");

                    b.HasIndex("PlayerId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("SelectAnswerId");

                    b.HasIndex("SessionId");

                    b.ToTable("PlayerGameAnswer", (string)null);
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.PlayerGameSession", b =>
                {
                    b.Property<int>("SessionId")
                        .HasColumnType("int");

                    b.Property<string>("PlayerId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("IsHost")
                        .HasColumnType("bit");

                    b.Property<bool>("IsReady")
                        .HasColumnType("bit");

                    b.HasKey("SessionId", "PlayerId")
                        .HasName("PK__PlayerGa__5D5075DCB278F05E");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerGameSession", (string)null);
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsHard")
                        .HasColumnType("bit");

                    b.Property<int>("KeyDigit")
                        .HasColumnType("int");

                    b.Property<string>("QuestionDesc")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("QuestionId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)")
                        .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");

                    b.HasKey("Id")
                        .HasName("PK__Question__3213E83F75227A3C");

                    b.HasIndex(new[] { "QuestionId" }, "UQ__Question__0DC06FADEAB8E116")
                        .IsUnique();

                    b.ToTable("Question", (string)null);
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.QuestionAnswer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Answer")
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<bool>("IsTrue")
                        .HasColumnType("bit");

                    b.Property<string>("QuestionAnswerId")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)")
                        .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");

                    b.Property<string>("QuestionId")
                        .HasMaxLength(36)
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id")
                        .HasName("PK__Question__3213E83FFE50928A");

                    b.HasIndex("QuestionId");

                    b.HasIndex(new[] { "QuestionAnswerId" }, "UQ__Question__86BEDFCE7B684988")
                        .IsUnique();

                    b.ToTable("QuestionAnswer", (string)null);
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.Leaderboard", b =>
                {
                    b.HasOne("EscapeRoomAPI.Entities.Player", "Player")
                        .WithMany("Leaderboards")
                        .HasForeignKey("PlayerId")
                        .HasPrincipalKey("PlayerId")
                        .IsRequired()
                        .HasConstraintName("FK_Leaderboard_PlayerId");

                    b.HasOne("EscapeRoomAPI.Entities.GameSession", "Session")
                        .WithMany("Leaderboards")
                        .HasForeignKey("SessionId")
                        .IsRequired()
                        .HasConstraintName("FK_Leaderboard_SessionId");

                    b.Navigation("Player");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.PlayerGameAnswer", b =>
                {
                    b.HasOne("EscapeRoomAPI.Entities.Player", "Player")
                        .WithMany("PlayerGameAnswers")
                        .HasForeignKey("PlayerId")
                        .HasPrincipalKey("PlayerId")
                        .IsRequired()
                        .HasConstraintName("FK_PlayerGameAnswer_PlayerId");

                    b.HasOne("EscapeRoomAPI.Entities.Question", "Question")
                        .WithMany("PlayerGameAnswers")
                        .HasForeignKey("QuestionId")
                        .HasPrincipalKey("QuestionId")
                        .IsRequired()
                        .HasConstraintName("FK_PlayerGameAnswer_QuestionId");

                    b.HasOne("EscapeRoomAPI.Entities.QuestionAnswer", "SelectAnswer")
                        .WithMany("PlayerGameAnswers")
                        .HasForeignKey("SelectAnswerId")
                        .HasPrincipalKey("QuestionAnswerId")
                        .IsRequired()
                        .HasConstraintName("FK_PlayerGameAnswer_SelectAnswerId");

                    b.HasOne("EscapeRoomAPI.Entities.GameSession", "Session")
                        .WithMany("PlayerGameAnswers")
                        .HasForeignKey("SessionId")
                        .IsRequired()
                        .HasConstraintName("FK_PlayerGameAnswer_SessionId");

                    b.Navigation("Player");

                    b.Navigation("Question");

                    b.Navigation("SelectAnswer");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.PlayerGameSession", b =>
                {
                    b.HasOne("EscapeRoomAPI.Entities.Player", "Player")
                        .WithMany("PlayerGameSessions")
                        .HasForeignKey("PlayerId")
                        .HasPrincipalKey("PlayerId")
                        .IsRequired()
                        .HasConstraintName("FK_PlayerGameSession_PlayerId");

                    b.HasOne("EscapeRoomAPI.Entities.GameSession", "Session")
                        .WithMany("PlayerGameSessions")
                        .HasForeignKey("SessionId")
                        .IsRequired()
                        .HasConstraintName("FK_PlayerGameSession_SessionId");

                    b.Navigation("Player");

                    b.Navigation("Session");
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.QuestionAnswer", b =>
                {
                    b.HasOne("EscapeRoomAPI.Entities.Question", "Question")
                        .WithMany("QuestionAnswers")
                        .HasForeignKey("QuestionId")
                        .HasPrincipalKey("QuestionId")
                        .HasConstraintName("FK_QuestionAnswer_QuestionId");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.GameSession", b =>
                {
                    b.Navigation("Leaderboards");

                    b.Navigation("PlayerGameAnswers");

                    b.Navigation("PlayerGameSessions");
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.Player", b =>
                {
                    b.Navigation("Leaderboards");

                    b.Navigation("PlayerGameAnswers");

                    b.Navigation("PlayerGameSessions");
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.Question", b =>
                {
                    b.Navigation("PlayerGameAnswers");

                    b.Navigation("QuestionAnswers");
                });

            modelBuilder.Entity("EscapeRoomAPI.Entities.QuestionAnswer", b =>
                {
                    b.Navigation("PlayerGameAnswers");
                });
#pragma warning restore 612, 618
        }
    }
}
