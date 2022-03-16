using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WebApi.Contracts.Requests;
using WebApi.Domain;
using WebApi.Services.Interfaces;

namespace WebApi.Services
{
    public class DefaultAuthorizationService : IAuthorizationService
    {
        public async Task<AuthenticationResult> Login(AuthenticationRequest request)
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: request.Password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return new AuthenticationResult { Success = true };
        }
    }
}
