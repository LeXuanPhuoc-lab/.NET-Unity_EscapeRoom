namespace EscapeRoomAPI.Payloads;

public class APIRoutes
{
    public const string Base = "api";

    public static class Authentication
    {
        public const string SignIn = Base + "/sign-in";
        public const string Register = Base + "/register";
    }

    public static class Players
    {
        // [POST]
        public const string CreateRoom = Base + "/players/room";
        // [GET]
        public const string FindRoom = Base + "/players/{username}/room";
        // [GET]
        public const string SubmitUnlockRoomKey = Base + "/players/{username}/room/unclock/{key}";
        // [DELETE]
        public const string OutRoom = Base + "/players/{username}/room";
        // [PATCH]
        public const string StartRoom = Base + "/players/{username}/room/start";
        // [PATCH]        
        public const string ModifyReady = Base + "/players/{username}/room/ready";
    }

    public static class Questions
    {
        // [GET]
        public const string RetrieveQuestionNormalLevel = Base + "/questions/normal-level";
        // [GET]
        public const string RetrieveQuestionHardLevel = Base + "/questions/hard-level";
        // [GET]
        public const string GetAllQuestion = Base + "/questions";
        // [POST]
        public const string CreateQuestion = Base + "/questions";
        // [POST]
        public const string SubmitAnswer = Base + "/questions/submit-answer";
    }

    public static class Leaderboard
    {
        // [GET]
        public const string ShowLeaderboard = Base + "/leaderboard";
    }

    public static class GameSessions
    {
        // [GET]
        public const string JoinByCode = Base + "/game-sessions/code";
        // [GET]
        public const string JoinGameSession = Base + "/game-sessions/{sessionId}";
        // [GET]
        public const string RetrieveList = Base + "/game-sessions";
    }
}