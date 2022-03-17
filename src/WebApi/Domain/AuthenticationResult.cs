namespace WebApi.Domain
{
    public class AuthenticationResult
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string[] Errors { get; set; }

        public bool Success { get; set; }
    }
}
