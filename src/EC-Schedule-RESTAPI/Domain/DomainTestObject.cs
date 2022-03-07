using System;
using System.ComponentModel.DataAnnotations;

namespace EC_Schedule_RESTAPI.Domain
{
    public class DomainTestObject
    {
        [Key]
        public Guid DomainTestObjectId { get; set; }

        public string Info { get; set; }
    }
}
