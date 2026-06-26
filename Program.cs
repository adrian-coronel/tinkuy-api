using Microsoft.EntityFrameworkCore;
using Npgsql;
using StockLinkApi.Data;
using StockLinkApi.Endpoints;
using StockLinkApi.Hubs;
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

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddSignalR();

// AllowCredentials + SetIsOriginAllowed requerido por SignalR (no compatible con AllowAnyOrigin)
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p
        .SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

builder.Services.ConfigureHttpJsonOptions(opt =>
    opt.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

var app = builder.Build();

app.UseCors();

app.MapHub<NotificationHub>("/hubs/notifications");

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
NotificationEndpoints.Map(app);

app.Run();
