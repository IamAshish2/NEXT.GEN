
using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Models;
namespace MinimalAPIConcepts.Context
{
    public class ApplicationDbContext:DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
    }
}
