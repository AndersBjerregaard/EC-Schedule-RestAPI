using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Contracts.Requests;
using WebApi.Domain;

namespace WebApi.Services.Interfaces
{
    public interface IAuthorizationService
    {
        Task<AuthenticationResult> Login(AuthenticationRequest request);
    }
}
