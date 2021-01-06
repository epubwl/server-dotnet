using Microsoft.EntityFrameworkCore;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Data
{
    public class EpubFileDbContext : DbContext
    {
        public EpubFileDbContext(DbContextOptions<EpubFileDbContext> options)
            : base(options)
        {
        }

        public DbSet<EpubFile> EpubFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpubFile>()
                .HasKey(e => e.EpubId);
        }
    }
}