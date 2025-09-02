using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBooker.Models;

namespace RestaurantBooker.Controllers
{

    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Your action methods...

        // GET: /Reservation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Reservation/Create
        [HttpPost]
        public IActionResult Create(Reservation reservation)
        {
            // Check guest limit first
            if (reservation.NumberOfGuests > 12)
            {
                ModelState.AddModelError("NumberOfGuests", "Number of guests exceeded");
                return View(reservation);
            }

            // Check for double booking
            bool isDoubleBooked = _context.Reservations.Any(r =>
                r.StartTime == reservation.StartTime &&
                r.EndTime == reservation.EndTime &&
                r.RoomId == reservation.RoomId);

            if (isDoubleBooked)
            {
                ModelState.AddModelError("", "There's already a reservation at this time.");
                return View(reservation);
            }

            //Proceed if everything is valid
            else if (ModelState.IsValid)
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(reservation);
        }

        public IActionResult SelectTable()
        {
            // Need some form of if statement checking to see if form is filled out/completed
            var tables = _context.Tables.ToList();
            return View(tables);
        }
    }
}
