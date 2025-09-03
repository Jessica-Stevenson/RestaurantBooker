using Microsoft.EntityFrameworkCore;
using RestaurantBooker.Models;

public static class DbInitializer
{
    public static void Seed(IServiceProvider serviceProvider)
    {

        using (var context = new ApplicationDbContext(
           serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {

            if (!context.Tables.Any())
            {
            return;
            }

            context.Tables.AddRange(
                  new Table { Location = "Window", Capacity = 4, IsAvailable = true },
                new Table { Location = "Booth", Capacity = 2, IsAvailable = true },
                new Table { Location = "Patio", Capacity = 6, IsAvailable = true }
            );
            context.SaveChanges();
        }
    }
}