using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Domain
{
    public class UserBaseClass
    {
        [Key, Editable(false)]
        public Guid Id { get; set; }

        [Editable(false)]
        public string Salt { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
