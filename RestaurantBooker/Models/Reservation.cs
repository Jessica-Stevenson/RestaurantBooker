using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantBooker.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, Phone]
        public string Phone { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeSpan Time { get; set; }

        [Required, Range(1, 20)]
        public int NumberOfGuests { get; set; }

        public string SpecialRequests { get; set; }
    }
}
