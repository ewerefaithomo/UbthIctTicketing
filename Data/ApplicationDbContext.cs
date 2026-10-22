using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UbthIctTicketing.Models;

namespace UbthIctTicketing.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Incident> Incidents { get; set; }
    }
}