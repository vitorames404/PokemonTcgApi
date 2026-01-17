using Microsoft.EntityFrameworkCore;
using PokemonTcgApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient for external API calls
// Whenever you use an External API, use IHttpClientFactory to create HttpClient instances
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
