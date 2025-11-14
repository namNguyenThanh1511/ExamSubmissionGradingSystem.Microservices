using Microsoft.EntityFrameworkCore;
using Submission.Repositories.Entities;
namespace Submission.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // Define DbSets for your entities here
        public DbSet<StudentSubmission> Submissions { get; set; }
    }
}
