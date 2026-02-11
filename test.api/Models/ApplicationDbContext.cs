using Microsoft.EntityFrameworkCore;

namespace test.api.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}
