using LibraryApplication.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApplication.Api.Data
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) {}

        public DbSet<BookEntity> Books => Set<BookEntity>();
        public DbSet<AuthorEntity> Authors => Set<AuthorEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookEntity>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}