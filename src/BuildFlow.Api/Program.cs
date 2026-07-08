using BuildFlow.Identity.Application;
using BuildFlow.Identity.Infrastructure;
// using BuildFlow.Identity.Application.Abstractions;
using BuildFlow.Api.Authentication;
using Serilog;
using BuildFlow.Api.Authentication;
using BuildFlow.Api.Endpoints;
using BuildFlow.Api.Documentation;
using BuildFlow.Projects.Application;
using BuildFlow.Projects.Infrastructure;

using IdentityCurrentUser = BuildFlow.Identity.Application.Abstractions.ICurrentUserService;
using ProjectsCurrentUser = BuildFlow.Projects.Application.Abstractions.ICurrentUserService;

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

    builder.Services.AddBuildFlowSwagger();

    // Identity from the current HTTP request. Scoped: one identity per request.
    builder.Services.AddHttpContextAccessor();
    //builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

   // Identity current-user (strong IDs) and Projects current-user (raw Guids).
    builder.Services.AddScoped<IdentityCurrentUser, CurrentUserService>();
    builder.Services.AddScoped<ProjectsCurrentUser, ProjectsCurrentUserService>();

    // JWT authentication + authorization, bound to the same Jwt options.
    builder.Services.AddJwtAuthentication(builder.Configuration);

    var app = builder.Build();

    // --- HTTP pipeline ---
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Logs one structured line per HTTP request (method, path, status, timing).
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    
    // Order matters: authentication first (who are you?),
    // then authorization (are you allowed?).
    app.UseAuthentication();
    app.UseAuthorization();
    
    // Vertical-slice endpoints will be mapped here in later batches.
    // Map all vertical-slice endpoints.
    app.MapBuildFlowEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "BuildFlow API host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}