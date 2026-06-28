using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BuildFlow.Identity.Infrastructure.Persistence;

// يُستخدم فقط وقت التصميم (توليد migrations) — لا في وقت التشغيل
internal sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlServer("Server=DESKTOP-LI2OH2L\\LUAAY;Database=BuildFlow;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new IdentityDbContext(options);
    }
}