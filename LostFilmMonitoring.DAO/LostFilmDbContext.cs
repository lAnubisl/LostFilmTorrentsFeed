using LostFilmMonitoring.DAO.DomainModels;
using Microsoft.EntityFrameworkCore;

namespace LostFilmMonitoring.DAO
{
    public class LostFilmDbContext : DbContext
    {
        private readonly string _connectionString;

        public LostFilmDbContext() : base()
        {
            _connectionString = "Data Source = lostfilmtorrentfeed.db";
        }

        public LostFilmDbContext(string connectionString) : base()
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(u => u.Id);
            modelBuilder.Entity<Feed>()
                .ToTable("Feeds")
                .HasKey(f => f.Id);
            modelBuilder.Entity<Series>()
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
            modelBuilder.Entity<Setting>()
                .HasKey(s => s.Name);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Feed> Feeds { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Series> Serials { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}
