using System.Collections.Generic;

namespace RestaurantBooker.Models
{
    public class SelectTableViewModel
    {
        public Reservation Reservation { get; set; }
        public IEnumerable<Table> Tables { get; set; }
    }
}
