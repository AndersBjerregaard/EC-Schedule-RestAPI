using EC_Schedule_RESTAPI.Domain;
using EC_Schedule_RESTAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EC_Schedule_RESTAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
        }

        public DbSet<DomainTestObject> TestObjects { get; set; }
    }
}