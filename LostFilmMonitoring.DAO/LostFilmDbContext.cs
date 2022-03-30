// <copyright file="LostFilmDbContext.cs" company="Alexander Panfilenok">
// MIT License
// Copyright (c) 2021 Alexander Panfilenok
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the 'Software'), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>

namespace LostFilmMonitoring.DAO
{
    using LostFilmMonitoring.DAO.Interfaces.DomainModels;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Database context.
    /// </summary>
    public class LostFilmDbContext : DbContext
    {
        private readonly string connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="LostFilmDbContext"/> class.
        /// </summary>
        public LostFilmDbContext()
            : base()
        {
            this.connectionString = "Data Source = lostfilmtorrentfeed.db";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LostFilmDbContext"/> class.
        /// </summary>
        /// <param name="connectionString">Database connection string.</param>
        public LostFilmDbContext(string connectionString)
            : base()
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets Feeds.
        /// </summary>
        public DbSet<Feed> Feeds { get; set; }

        /// <summary>
        /// Gets or sets Users.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets Series.
        /// </summary>
        public DbSet<Series> Series { get; set; }

        /// <summary>
        /// Gets or sets Subscriptions.
        /// </summary>
        public DbSet<Subscription> Subscriptions { get; set; }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(this.connectionString);
            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(u => u.Id);
            modelBuilder.Entity<Feed>()
                .ToTable("Feeds")
                .HasKey(f => f.UserId);
            modelBuilder.Entity<Series>()
                .ToTable("Series")
                .HasKey(s => s.Name);
            modelBuilder.Entity<Subscription>()
                .ToTable("Subscriptions")
                .HasKey(s => new { s.UserId, s.SeriesName });
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .HasConstraintName("FK_Subscriptions_Users");
            base.OnModelCreating(modelBuilder);
        }
    }
}
