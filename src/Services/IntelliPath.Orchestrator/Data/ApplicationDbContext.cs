using IntelliPath.Orchestrator.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntelliPath.Orchestrator.Data;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditableEntitySaveChangesInterceptor saveChangesInterceptor) : DbContext(options)
{
    public DbSet<ChatMessage> Messages { get; set; }

    public DbSet<Conversation> Conversations { get; set; }

    public DbSet<MemoryTag> MemoryTags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(saveChangesInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MemoryTag>().HasData(
            new MemoryTag { Id = Guid.Parse("4A425F94-7D0B-48A9-9E99-7CEB42CF6F1D"), Name = "Personal Info" },
            new MemoryTag { Id = Guid.Parse("73095DA3-2A2E-4310-9501-FBCCE3BAD261"), Name = "Work Info" },
            new MemoryTag { Id = Guid.Parse("11FA6AA8-6781-4FA9-8CA3-EC5F1DE998C4"), Name = "Hobbies" },
            new MemoryTag { Id = Guid.Parse("1D3B28A9-7EAD-485C-9ED8-807E5C6D4DDB"), Name = "Skills" },
            new MemoryTag { Id = Guid.Parse("54A563DF-7731-47E6-8808-72BCA4BAE316"), Name = "Education" },
            new MemoryTag { Id = Guid.Parse("E808F592-8C8F-461D-9BE0-6F9D0F1D4F6F"), Name = "Certifications" },
            new MemoryTag { Id = Guid.Parse("305E4FEB-3C13-499E-B4D8-483BE516725A"), Name = "Projects" }
        );
    }
}