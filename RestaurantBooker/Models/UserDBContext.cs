using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RestaurantBooker.Models
{
    public class UserDBContext : IdentityDbContext<User>
    {
        public UserDBContext(DbContextOptions<UserDBContext> options): base(options)
        {

//Vison
        }

        public DbSet<Reservation> Reservations { get; set; }
=======
        public DbSet<Reservation> Reservations { get; set; }

//Tegan
    }
}
