namespace EpubWebLibraryServer.Areas.User.Services
{
    public class JwtAuthenticationOptions
    {
        public string Audience { get; set; }

        public string Issuer { get; set; }
        
        public string EncryptingSecret { get; set; }

        public string SigningSecret { get; set; }

        public int LifetimeInMinutes { get; set; }
    }
}