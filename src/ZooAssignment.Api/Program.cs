using ZooAssignment.Api.Extensions;
using ZooAssignment.BusinessLayer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

// Add Logging
builder.Logging.AddZooLogging();

// Add Zoo Services (Facade Pattern)
builder.Services.AddZooServices(builder.Configuration);

var app = builder.Build();

// Run database migrations on startup
await app.RunDatabaseMigrationsAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Use Swagger/OpenAPI (Swashbuckle)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZooAssignment API V1");
        c.RoutePrefix = string.Empty; // serve UI at app root in Development
    });
}

app.UseHttpsRedirection();

// Zoo API Endpoints
app.MapGet("/api/zoo/getDailyCost", async (IZooService zooService, ILogger<Program> logger) =>
{
    logger.LogInformation("GET /api/zoo/getDailyCost - Request started");
    try
    {
        var totalCost = await zooService.GetTotalDailyCostAsync();
        logger.LogInformation("GET /api/zoo/getDailyCost - Request completed successfully. Total monthly cost: {totalCost}", totalCost);
        return Results.Ok(new { totalDailyCost = totalCost });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "GET /api/zoo/getDailyCost - Error occurred");
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("GetDailyCost")
.Produces<dynamic>(StatusCodes.Status200OK);

app.Run();
