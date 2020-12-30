using Microsoft.EntityFrameworkCore;

namespace EpubWebLibraryServer.Areas.Library.Data
{
    public class EpubCoverDbContext : DbContext
    {
        public DbSet<EpubCover> EpubCovers { get; set; }

        public EpubCoverDbContext(DbContextOptions<EpubCoverDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpubCover>()
                .HasKey(e => e.EpubId);
        }
    }
}