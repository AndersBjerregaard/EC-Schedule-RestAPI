using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Domain;
using WebApi.Services.Interfaces;

namespace WebApi.Services
{
    public class EFDbService : IDbService<UserDomainClass>
    {
        private readonly ApplicationDbContext _dbContext;

        public EFDbService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserDomainClass> FindAsync(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDomainClass[]> GetAllAsync()
        {
            return await _dbContext.Users.ToArrayAsync();
        }
    }
}
