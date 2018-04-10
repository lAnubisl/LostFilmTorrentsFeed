using LostFilmMonitoring.DAO.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace LostFilmMonitoring.DAO
{
    public class LostFilmDbContext : DbContext
    {
        private readonly string _connectionString;

        public LostFilmDbContext(string connectionString) : base()
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(u => u.Id);
            modelBuilder.Entity<Serial>()
                .ToTable("Serials")
                .HasKey(s => s.Name);
            modelBuilder.Entity<Subscription>()
                .ToTable("Subscriptions")
                .HasKey(s => new { s.UserId, s.Serial });
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK_Subscriptions_Users");

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Serial> Serials { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}