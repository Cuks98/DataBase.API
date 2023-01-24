using DataBase.API.Models;
using DataBaseAPI.Models;

namespace DataBaseAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Trainer> Trainers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().Property(u => u.LeftOnRegistration).HasComputedColumnSql("DATEDIFF(DAY, GETDATE(), RegisteredTo)");
            base.OnModelCreating(builder);
        }

    }
}
