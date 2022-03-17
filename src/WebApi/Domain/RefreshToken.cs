using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }

        public string JwtId { get; set; }

        public DateTime CreationDate { get; set; }

        // A refresh token is much more long-lived than a regular token. Sometimes 6 months or more
        // Making the expiry date of a refresh token configurable might not be a bad idea
        public DateTime ExpiryDate { get; set; }

        // If it's used you don't wanna issue another JWT with the same refresh token for security purposes
        public bool Used { get; set; }

        // A good case for invalidating all refresh tokens for a user, could be when the user changes password, etc.
        public bool Invalidated { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserDomainClass User { get; set; }
    }
}
