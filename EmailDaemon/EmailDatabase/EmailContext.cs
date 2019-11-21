using Microsoft.EntityFrameworkCore;
using EmailDaemon.DataTypes;

namespace EmailDaemon.EmailDatabase
{
    class EmailContext : DbContext
    {
        public DbSet<Email> Emails { get; set; }
        public DbSet<Job> Jobs { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=somesqlserver;Database=Emails;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().HasKey(j => j.Id);
        }
    }
}
