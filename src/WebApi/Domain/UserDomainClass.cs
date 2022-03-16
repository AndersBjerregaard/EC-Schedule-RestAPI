using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Domain
{
    public class UserDomainClass : UserBaseClass
    {
        public string Name { get; set; }
    }
}
