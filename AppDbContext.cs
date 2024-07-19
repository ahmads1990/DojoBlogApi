using DojoBlogApi.Models;
using Microsoft.EntityFrameworkCore;

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Author> Authors => Set<Author>();
}
