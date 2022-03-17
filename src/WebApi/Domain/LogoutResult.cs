namespace WebApi.Domain
{
    public class LogoutResult
    {
        public bool Success { get; set; }

        public string[] Errors { get; set; }
    }
}
