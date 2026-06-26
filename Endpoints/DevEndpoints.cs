using StockLinkApi.Data;
using StockLinkApi.Models;

namespace StockLinkApi.Endpoints;

public static class DevEndpoints
{
    public static void Map(WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;

        app.MapPost("/api/dev/users", async (AppDbContext db) =>
        {
            var user = new User { Name = "Test User", Phone = "999000111" };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(new { userId = user.Id });
        });
    }
}
