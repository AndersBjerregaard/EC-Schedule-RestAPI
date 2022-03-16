using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain
{
    public class UserDomainClass
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
