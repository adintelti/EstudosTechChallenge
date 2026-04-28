using FCG.Users.Domain.Entities;

namespace FCG.Users.Infrastructure.Persistence.Seed;

public static class AdminSeed
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var adminExists = context.Users.Any(x =>
            x.Email == "admin@fcg.com");

        if (adminExists)
            return;

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Name = "Administrador",
            Email = "admin@fcg.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin"
        };

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}