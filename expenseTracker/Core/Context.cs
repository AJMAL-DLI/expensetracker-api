using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DataAccess
{
    public class Context(IConfiguration configuration) : DbContext
    {
        public IConfiguration Configuration { get; } = configuration;
        public DbSet<Expense> Expense { get; set; } // Transaction table
        public DbSet<Categories> Categories { get; set; } // category table
        public DbSet<User> Users { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            object value = optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DB"));
        }
    }
}
