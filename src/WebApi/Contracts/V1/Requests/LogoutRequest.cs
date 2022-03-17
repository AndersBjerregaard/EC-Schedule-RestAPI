using System;

namespace WebApi.Contracts.V1.Requests
{
    public class LogoutRequest
    {
        public string AccessToken { get; set; }

        public Guid RefreshToken { get; set; }
    }
}
