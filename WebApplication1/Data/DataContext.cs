using Microsoft.EntityFrameworkCore;
using WebAPIAdverts.Models;

namespace WebAPIAdverts.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Announcement> Announcements { get; set; }

        public DataContext(DbContextOptions<DataContext> options)
        : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Announcement>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Announcement>()
                .HasIndex(a => a.Id)
                .IsUnique();

            modelBuilder.Entity<Announcement>()
                .HasIndex(a => a.Number);

            modelBuilder.Entity<Announcement>()
                .HasIndex(a => a.Rating);

            base.OnModelCreating(modelBuilder);
        }
    }
}
