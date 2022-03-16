﻿using Microsoft.EntityFrameworkCore;
using WebApi.Domain;

namespace WebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {

        }

        public DbSet<DomainTestObject> TestObjects { get; set; }

        public DbSet<UserDomainClass> Users { get; set; }
    }
}
