using Microsoft.EntityFrameworkCore;
using Npgsql;
using StockLinkApi.Data;
using StockLinkApi.Endpoints;
using StockLinkApi.Services;

var builder = WebApplication.CreateBuilder(args);

var dataSource = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("Default"))
    .EnableDynamicJson()
    .Build();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(dataSource));

if (builder.Configuration.GetValue<bool>("UseMockVision"))
    builder.Services.AddSingleton<IGeminiVisionService, MockVisionService>();
else
    builder.Services.AddHttpClient<IGeminiVisionService, GeminiVisionService>();

builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.ConfigureHttpJsonOptions(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await StockLinkApi.Data.DbSeeder.SeedAsync(db);
}

ProductSearchEndpoints.Map(app);
ReservationEndpoints.Map(app);
StoreEndpoints.Map(app);
VisionEndpoints.Map(app);
DevEndpoints.Map(app);

app.Run();
