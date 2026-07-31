using BuildFlow.Identity.Application;
using BuildFlow.Identity.Infrastructure;
// using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Api.Authentication;
using Serilog;

using BuildFlow.Api.Endpoints;
using BuildFlow.Api.Documentation;
using BuildFlow.Projects.Application;
using BuildFlow.Projects.Infrastructure;
using Microsoft.Extensions.Hosting;

using BuildFlow.Identity.Infrastructure.Persistence;
using BuildFlow.Projects.Infrastructure.Persistence;
using BuildFlow.Documents.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using DocumentsCurrentUser = BuildFlow.Documents.Application.Abstractions.ICurrentUserService;

using IdentityCurrentUser = BuildFlow.Identity.Application.Abstractions.ICurrentUserService;
using ProjectsCurrentUser = BuildFlow.Projects.Application.Abstractions.ICurrentUserService;
using BuildFlow.Documents.Application;
using BuildFlow.Documents.Infrastructure;

// Bootstrap logger: a temporary logger so that even failures during
// host startup get logged before the full configuration is read.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting BuildFlow API host");

    var builder = WebApplication.CreateBuilder(args);

    // Replace the default logging with Serilog, reading the detailed
    // configuration (levels, sinks) from appsettings at runtime.
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));

    // --- Service registration (the Composition Root) ---
    builder.Services.AddIdentityApplication();
    builder.Services.AddIdentityInfrastructure(builder.Configuration);

    builder.Services.AddProjectsApplication();
    builder.Services.AddProjectsInfrastructure(builder.Configuration);

    builder.Services.AddDocumentsApplication();
    builder.Services.AddDocumentsInfrastructure(builder.Configuration);

    builder.Services.AddBuildFlowSwagger();

    // Identity from the current HTTP request. Scoped: one identity per request.
    builder.Services.AddHttpContextAccessor();
    //builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

   // Identity current-user (strong IDs) and Projects current-user (raw Guids).
    builder.Services.AddScoped<IdentityCurrentUser, CurrentUserService>();
    builder.Services.AddScoped<ProjectsCurrentUser, ProjectsCurrentUserService>();
    builder.Services.AddScoped<DocumentsCurrentUser, DocumentsCurrentUserService>();

    // JWT authentication + authorization, bound to the same Jwt options.
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // سياسة السماح للواجهة الأمامية بالاتصال
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // أصل الواجهة في التطوير
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

    var app = builder.Build();

// إنشاء مخطّط قاعدة البيانات عند الإقلاع
// إنشاء مخطّط قاعدة البيانات عند الإقلاع، إن لم يكن موجوداً
// إنشاء مخطّط قاعدة البيانات عند الإقلاع، إن لم يكن موجوداً
// إنشاء مخطّط قاعدة البيانات عند الإقلاع، إن كانت فارغة
using (var scope = app.Services.CreateScope())
{
    var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    var projectsDb = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();
    var documentsDb = scope.ServiceProvider.GetRequiredService<DocumentsDbContext>();

    var identityCreator = identityDb.GetService<IRelationalDatabaseCreator>();
    var projectsCreator = projectsDb.GetService<IRelationalDatabaseCreator>();
    var documentsCreator = documentsDb.GetService<IRelationalDatabaseCreator>();

    // أنشئ القاعدة إن لم تكن موجودة أصلاً
    if (!await identityCreator.ExistsAsync())
    {
        await identityCreator.CreateAsync();
    }

    // أنشئ الجداول إن كانت القاعدة فارغة
    if (!await identityCreator.HasTablesAsync())
    {
        await identityCreator.CreateTablesAsync();
        await projectsCreator.CreateTablesAsync();
        await documentsCreator.CreateTablesAsync();
    }
}
    // --- HTTP pipeline ---
    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}

        // التوثيق مفعّل في كل البيئات، فهذا مشروع محفظة يُجرَّب حيّاً
        app.UseSwagger();
        app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseCors("AllowFrontend");

    // المصادقة أولاً، فتُقرأ الهوية من الرمز
    app.UseAuthentication();

    // ثم إثراء السياق بالهوية، فتتوفّر لكل ما بعده
    app.UseMiddleware<BuildFlow.Api.Logging.RequestContextEnrichmentMiddleware>();

    // ثم تسجيل الطلبات، فيحمل كل سطر طلب هوية صاحبه
    app.UseSerilogRequestLogging();

    app.UseAuthorization();
    
    // Vertical-slice endpoints will be mapped here in later batches.
    // Map all vertical-slice endpoints.
    app.MapBuildFlowEndpoints();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException
    && ex.GetType().Name != "HostAbortedException")
{
    Log.Fatal(ex, "BuildFlow API host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }