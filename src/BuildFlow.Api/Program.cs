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
using BuildFlow.SharedInfrastructure.Auditing;
using BuildFlow.Api.Observability;
using Serilog.Sinks.OpenTelemetry;
using BuildFlow.SharedInfrastructure.Caching;

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
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);

        var otlpEndpoint = context.Configuration["Otel:Endpoint"];

        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            configuration.WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = otlpEndpoint;
                options.Protocol = OtlpProtocol.Grpc;
                options.ResourceAttributes = new Dictionary<string, object>
                {
                    ["service.name"] = "BuildFlow.Api"
                };
            });
        }
    });

    builder.Services.AddCaching(builder.Configuration);

    // --- Service registration (the Composition Root) ---
    builder.Services.AddIdentityApplication();
    builder.Services.AddIdentityInfrastructure(builder.Configuration);

    builder.Services.AddProjectsApplication();
    builder.Services.AddProjectsInfrastructure(builder.Configuration);

    builder.Services.AddDocumentsApplication();
    builder.Services.AddDocumentsInfrastructure(builder.Configuration);

    // تركيب التدقيق: السياق، والمستودع، والاعتراض
    builder.Services.AddAuditing(builder.Configuration);

    builder.Services.AddBuildFlowSwagger();

    // Identity from the current HTTP request. Scoped: one identity per request.
    builder.Services.AddHttpContextAccessor();
    //builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

   // Identity current-user (strong IDs) and Projects current-user (raw Guids).
    builder.Services.AddScoped<IdentityCurrentUser, CurrentUserService>();
    builder.Services.AddScoped<ProjectsCurrentUser, ProjectsCurrentUserService>();
    builder.Services.AddScoped<DocumentsCurrentUser, DocumentsCurrentUserService>();



    builder.Services.AddObservability(builder.Configuration);

    // JWT authentication + authorization, bound to the same Jwt options.
    builder.Services.AddJwtAuthentication(builder.Configuration);

    // سياسة السماح للواجهة الأمامية بالاتصال
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
       policy.WithOrigins(
                  "http://localhost:5173",
                  "https://build-flow-v3-orpin.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

    var app = builder.Build();

    // تطبيق الهجرات عند الإقلاع، فينشئ الجداول ويعدّلها حسب الهجرات غير المطبّقة
    using (var scope = app.Services.CreateScope())
    {
        var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        var projectsDb = scope.ServiceProvider.GetRequiredService<ProjectsDbContext>();
        var documentsDb = scope.ServiceProvider.GetRequiredService<DocumentsDbContext>();
        var auditDb = scope.ServiceProvider.GetRequiredService<AuditDbContext>();

        // كل سياق يطبّق هجراته، فالهجرات تنشئ الجداول وتعدّلها معاً
        await identityDb.Database.MigrateAsync();
        await projectsDb.Database.MigrateAsync();
        await documentsDb.Database.MigrateAsync();
        await auditDb.Database.MigrateAsync();

        Log.Information("Database migrations applied");
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