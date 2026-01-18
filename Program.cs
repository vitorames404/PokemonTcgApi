using Microsoft.EntityFrameworkCore;
using PokemonTcgApi.Data;
using PokemonTcgApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Register database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient for external API calls
// Whenever you use an External API, use IHttpClientFactory to create HttpClient instances
builder.Services.AddHttpClient();
builder.Services.AddScoped<PokemonTcgService>();



var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
