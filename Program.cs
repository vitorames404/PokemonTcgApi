using Microsoft.EntityFrameworkCore;
using PokemonTcgApi.Data;
using PokemonTcgApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Register database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register HttpClient for external API calls with timeout configuration
builder.Services.AddHttpClient<PokemonTcgService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var rapidApiKey = configuration["PokemonTcgApi:RapidApiKey"];
    var rapidApiHost = configuration["PokemonTcgApi:RapidApiHost"];

    client.Timeout = TimeSpan.FromSeconds(30);

    if (!string.IsNullOrEmpty(rapidApiKey))
        client.DefaultRequestHeaders.Add("X-RapidAPI-Key", rapidApiKey);
    if (!string.IsNullOrEmpty(rapidApiHost))
        client.DefaultRequestHeaders.Add("X-RapidAPI-Host", rapidApiHost);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder => builder.WithOrigins("http://localhost:4200")
                         .AllowAnyMethod()
                         .AllowAnyHeader());
});

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

app.UseCors("AllowAngular");

app.MapControllers();

app.Run();
