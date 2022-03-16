namespace WebApi.Domain
{
    public class AuthenticationResult
    {
        public string[] Errors { get; set; }

        public bool Success { get; set; }
    }
}
