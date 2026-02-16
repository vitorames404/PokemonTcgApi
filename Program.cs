using Microsoft.EntityFrameworkCore;
using PokemonTcgApi.Data;
using PokemonTcgApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient for external API calls with timeout configuration
builder.Services.AddHttpClient<PokemonTcgService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30); // 30 second timeout for all requests
});
builder.Services.AddScoped<PokemonTcgService>();

var app = builder.Build();

// Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pokemon TCG API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
