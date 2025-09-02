using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBooker.Models;

namespace RestaurantBooker.Controllers
{

    public class ReservationController : Controller
    {


//Vison
        private readonly UserDBContext _context;

        public ReservationController(UserDBContext context)
=======
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
//Tegan
        {
            _context = context;
        }

//Vison
        public async Task<IActionResult> Index()
        {
            // enusre it enter List，not null
            var reservations = await _context.Reservations.ToListAsync();
            return View(reservations);
        }
=======
        // Your action methods...
//Tegan

        // GET: /Reservation/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // GET: /Reservation/Edit/5
        [HttpGet]
        public IActionResult Edit(int Id) {
            return View(Edit);
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
//Vison
                // Save reservation logic here (e.g., to a database)
                return RedirectToAction("Index", "Reservation");
=======
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return RedirectToAction("Index");
//Tegan
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
