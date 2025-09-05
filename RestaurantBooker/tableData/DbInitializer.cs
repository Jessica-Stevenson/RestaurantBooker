using Microsoft.EntityFrameworkCore;
using RestaurantBooker.Models;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        var tables = new List<Table>
    {
        new Table { Location = "Window", Capacity = 2, IsAvailable = true },
        new Table { Location = "Window", Capacity = 4, IsAvailable = true },
        new Table { Location = "Window", Capacity = 6, IsAvailable = true },

        new Table { Location = "Booth", Capacity = 2, IsAvailable = true },
        new Table { Location = "Booth", Capacity = 4, IsAvailable = true },
        new Table { Location = "Booth", Capacity = 6, IsAvailable = true },

        new Table { Location = "Patio", Capacity = 2, IsAvailable = true },
        new Table { Location = "Patio", Capacity = 4, IsAvailable = true },
        new Table { Location = "Patio", Capacity = 6, IsAvailable = true },

        new Table { Location = "Center Hall", Capacity = 2, IsAvailable = true },
        new Table { Location = "Center Hall", Capacity = 4, IsAvailable = true },
        new Table { Location = "Center Hall", Capacity = 8, IsAvailable = true }
    };

        foreach (var table in tables)
        {
            if (!context.Tables.Any(t =>
                 t.Location == table.Location &&
                 t.Capacity == table.Capacity))
            {
                context.Tables.Add(table);
            }
        }

        context.SaveChanges();
    }

}
