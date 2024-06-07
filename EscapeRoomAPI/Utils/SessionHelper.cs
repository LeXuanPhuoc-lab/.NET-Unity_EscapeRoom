namespace EscapeRoomAPI.Utils
{
    public class SessionHelper
    {
        private static Random _random = new Random();

        public static string GenerateUniqueSessionCode(ICollection<string> existingCodes,
            int codeLength = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string newCode = string.Empty;

            do
            {
                newCode = new string(Enumerable.Repeat(chars, codeLength)
                                               .Select(s => s[_random.Next(s.Length)]).ToArray());
            } while (existingCodes.Contains(newCode));

            return newCode;
        }
    }
}
