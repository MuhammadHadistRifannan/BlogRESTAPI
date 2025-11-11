using Microsoft.EntityFrameworkCore;

namespace PersonalBlog;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options: options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Postingan>()
    .HasMany(p => p.tags)
    .WithMany(t => t.posts).UsingEntity(j => j.ToTable("PostinganTags"));
    }
    public DbSet<Postingan> posts { get; set; }
    public DbSet<Tag> tags{ get; set; }
}
