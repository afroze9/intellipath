using IntelliPath.Orchestrator.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntelliPath.Orchestrator.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ChatMessage> Messages { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
}