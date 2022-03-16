using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Domain;

namespace WebApi.Services.Interfaces
{
    public interface IUserDbService : IDbService<UserDomainClass>
    {
        
    }
}
