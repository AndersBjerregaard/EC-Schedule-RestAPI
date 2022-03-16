using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services.Interfaces
{
    public interface IDbService<T> where T : class
    {
        Task<T[]> GetAll();
    }
}
