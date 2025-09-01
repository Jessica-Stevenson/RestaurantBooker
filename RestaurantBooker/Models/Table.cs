using RestaurantBooker.Models;

public class Table
{
    public int Id { get; set; }
    public string Location { get; set; }
    public int Capacity { get; set; }

    public ICollection<Reservation> Reservations { get; set; }
}