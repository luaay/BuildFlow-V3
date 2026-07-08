using System.Reflection;
using BuildFlow.Projects.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BuildFlow.Projects.Infrastructure.Persistence;

public sealed class ProjectsDbContext : DbContext
{
    public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // تطبيق كل إعدادات الكيانات في هذه التجميعة تلقائياً
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}