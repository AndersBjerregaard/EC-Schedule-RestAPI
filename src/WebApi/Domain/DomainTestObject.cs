using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain
{
    public class DomainTestObject
    {
        [Key]
        public Guid DomainTestObjectId { get; set; }

        public string Info { get; set; }
    }
}
