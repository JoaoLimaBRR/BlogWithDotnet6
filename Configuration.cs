namespace Blog
{
    public static class Configuration
    {
        //TOKEN - JWT - JSON WEB TOKEN
        public static string JwtKey { get; set; } = "IzqjKiXvbU+S1cerdURAnQ==";
        public static string ApiKeyName = "api_key";
        public static string ApiKey = "api_keyV4jGUd0MBEOGnt6zbI/eKA==";
        public static SmtpConfiguration smtpConfiguration { get; set; }

        public class SmtpConfiguration
        {
            public string Host { get; set; }
            public int Port { get; set; } = 25;
            public string UserName { get; set; }
            public string Password{ get; set; }
        }
    }
}
