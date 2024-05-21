namespace EscapeRoomAPI.Payloads;

public static class APIRoutes
{
    public const string Base = "api";


    public static class Authentication
    {
        public const string SignIn = "/sign-in";
        public const string Register = "/register";
    }

    public static class Players
    {
        // [POST]
        public const string CreateRoom = "/players/{username:string}/room";
        // [GET]
        public const string FindRoom = "/players/{username:string}/room";
        // [DELETE]
        public const string OutRoom = "/players/{username:string}/room";
        // [PATCH]
        public const string StartRoom = "/players/{username:string}/room/start";
        // [PATCH]        
        public const string ModifyReady = "/players/{username:string}/room";
    }

    public static class Questions
    {
        // [GET]
        public const string RetrieveQuestionNormalLevel = "/questions/normal-level";
        // [GET]
        public const string RetrieveQuestionHardLevel = "/questions/hard-level";
        // [POST]
        public const string SubmitAnswer = "/questions/submit-answer";
    }

    public static class Leaderboard
    {
        // [GET]
        public const string ShowLeaderboard = "/leaderboard";
    }
}