using Microsoft.EntityFrameworkCore;
using Serilog;
using TreeNodes.Application;
using TreeNodes.Auth;
using TreeNodes.Infrastructure;
using TreeNodes.Infrastructure.Database;
using TreeNodes.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Debug()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// Services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddTreeNodesAuth(builder.Configuration);
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Disable automatic 400 response for model validation errors
        // Let exceptions bubble up to GlobalExceptionHandlingMiddleware
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddApplication();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Migrate database at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// IMPORTANT: Global exception handling middleware MUST come before authentication middleware
// This ensures authentication failures are caught, logged to the journal, and properly formatted
app.UseMiddleware<TreeNodes.Web.Middlewares.GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
