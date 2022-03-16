using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using WebApi.Contracts.Requests;
using WebApi.Data;
using WebApi.Domain;
using WebApi.Services.Interfaces;

namespace WebApi.Services
{
    public class DefaultAuthorizationService : IAuthorizationService
    {
        private readonly ApplicationDbContext _dbContext;

        public DefaultAuthorizationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AuthenticationResult> LoginAsync(AuthenticationRequest request)
        {
            // Check if a user with that username exists
            var user = await _dbContext.Users.FirstAsync(x => x.UserName == request.UserName);
            if (user is null)
                return new AuthenticationResult { Success = false, Errors = new string[1] { "Invalid credentials" } };

            // Hashing the password and comparing it to the hashed password in the database
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: request.Password,
                salt: Convert.FromBase64String(user.Salt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            if (hashed != user.Password)
                return new AuthenticationResult { Success = false, Errors = new string[1] { "Invalid credentials" } };

            // TODO: Create some authentication token in form of a jwt and return it to the client
            return new AuthenticationResult { Success = true };
        }

        public async Task<AuthenticationResult> RegisterAsync(AuthenticationRequest request)
        {
            // Check if a user with the same username exists
            var user = await _dbContext.Users.FirstAsync(x => x.UserName == request.UserName);
            if (!(user is null))
                return new AuthenticationResult { Success = false, Errors = new string[1] { "A user with that username already exists." } };

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

            // Initialize the user object
            UserDomainClass newUser = new UserDomainClass { Id = Guid.NewGuid(), UserName = request.UserName, Password = hashed, Salt = Convert.ToBase64String(salt) };

            // Save the registered user and the randomly generated salt in the database
            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();

            return new AuthenticationResult { Success = true };
        }
    }
}
