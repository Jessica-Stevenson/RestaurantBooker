using RestaurantBooker.Models;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        if (!context.Tables.Any())
        {
            context.Tables.AddRange(
    new Table { Location = "Window", Capacity = 4, IsAvailable = true },
    new Table { Location = "Booth", Capacity = 2, IsAvailable = true },
    new Table { Location = "Patio", Capacity = 6, IsAvailable = true }
);
            context.SaveChanges();
        }
    }
}