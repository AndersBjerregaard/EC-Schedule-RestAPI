using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebApi.Configuration;
using WebApi.Contracts.V1.Requests;
using WebApi.Data;
using WebApi.Domain;
using WebApi.Services.Interfaces;

namespace WebApi.Services
{
    public class DefaultAuthorizationService : IAuthorizationService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtSettings _jwtSettings;

        public DefaultAuthorizationService(ApplicationDbContext dbContext, TokenValidationParameters tokenValidationParameters, JwtSettings jwtSettings)
        {
            _dbContext = dbContext;
            _tokenValidationParameters = tokenValidationParameters;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthenticationResult> LoginAsync(AuthenticationRequest request)
        {
            // Check if a user with that username exists
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);
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

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<LogoutResult> LogoutAsync(LogoutRequest request)
        {
            ClaimsPrincipal claimsPrincipal;
            try
            {
                claimsPrincipal = GetPrincipalFromToken(request.AccessToken);
            }
            catch (SecurityTokenExpiredException)
            {
                return new LogoutResult { Errors = new[] { "This token has expired" } };
            }

            // Checking if token is valid
            if (claimsPrincipal is null)
                return new LogoutResult { Errors = new[] { "Invalid Token" } };

            // Checking if token is expired. This is seen as optional
            var expiryDateUnix = long.Parse(claimsPrincipal.Claims.Single(x =>
               x.Type == JwtRegisteredClaimNames.Exp).Value);

            // Apparently a standardized way of storing DateTimes, is seconds since the first of january 1970
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc < DateTime.UtcNow)
                return new LogoutResult { Errors = new[] { "This token has expired" } };

            // The Id of the token
            var jti = claimsPrincipal.Claims.Single(x =>
               x.Type == JwtRegisteredClaimNames.Jti).Value;

            // Validating if the token Id is the same as the refresh token's
            var storedRefreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x =>
               x.Token == request.RefreshToken.ToString());

            if (storedRefreshToken is null)
                return new LogoutResult { Errors = new[] { "This refresh token does not exist" } };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new LogoutResult { Errors = new[] { "This refresh token has expired" } };

            if (storedRefreshToken.Invalidated)
                return new LogoutResult { Errors = new[] { "This refresh token has been invalidated" } };

            if (storedRefreshToken.Used)
                return new LogoutResult { Errors = new[] { "This refresh token has been used" } };

            if (storedRefreshToken.JwtId != jti)
                return new LogoutResult { Errors = new[] { "This refresh token does not match this JWT" } };

            storedRefreshToken.Used = true;
            storedRefreshToken.Invalidated = true;
            _dbContext.RefreshTokens.Update(storedRefreshToken);
            await _dbContext.SaveChangesAsync();

            return new LogoutResult { Success = true };
        }

        public async Task<AuthenticationResult> RegisterAsync(AuthenticationRequest request)
        {
            // Check if a user with the same username exists
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName);
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

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            // See the GetPrincipalFromToken method for an explanation of this nonsense
            _tokenValidationParameters.ValidateLifetime = false;
            var claimsPrincipal = GetPrincipalFromToken(token);
            _tokenValidationParameters.ValidateLifetime = true;

            // Checking if token is valid
            if (claimsPrincipal is null)
                return new AuthenticationResult { Errors = new[] { "Invalid Token" } };

            // Checking if token is expired. This is seen as optional
            var expiryDateUnix = long.Parse(claimsPrincipal.Claims.Single(x =>
               x.Type == JwtRegisteredClaimNames.Exp).Value);

            // Apparently a standardized way of storing DateTimes, is seconds since the first of january 1970
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };

            // The Id of the token
            var jti = claimsPrincipal.Claims.Single(x =>
               x.Type == JwtRegisteredClaimNames.Jti).Value;

            // Validating if the token Id is the same as the refresh token's
            var storedRefreshToken = await _dbContext.RefreshTokens.SingleOrDefaultAsync(x =>
               x.Token == refreshToken);

            if (storedRefreshToken is null)
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };

            if (storedRefreshToken.Invalidated)
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };

            if (storedRefreshToken.Used)
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };

            if (storedRefreshToken.JwtId != jti)
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };

            storedRefreshToken.Used = true;
            _dbContext.RefreshTokens.Update(storedRefreshToken);
            await _dbContext.SaveChangesAsync();

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(claimsPrincipal.Claims.Single(x =>
               x.Type == "id").Value));

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        /// <returns></returns>
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // If the request is to refresh, you should not validate the lifetime on the expired access token. This is because the refresh tokens are being issued within the first 5 minutes of access token expiry, which is furthermore because the clockskew is set to 5 minutes by default (and if set to 0, they will be invalid each time)
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(UserDomainClass user)
        {
            // Here is when token stuff comes into the picture
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            // This one is quite a mouthful
            // Claims are essentially a bunch of properties in our token, that tells us a bunch of stuff about the specific user holding said token.
            // Things that could be nice to know could be: Id, Email, Permissions...
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // What this is, is a unique Id for this specific JWT. The reason for using this is token invalidation.
                    new Claim("id", user.Id.ToString()), // A custom claim to define a users Id
                    new Claim(JwtRegisteredClaimNames.Iss, "https://localhost:5001")
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Making the expiry date of the refresh token configurable might not be a bad idea
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString()
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token.ToString()
            };
        }

    }
}
