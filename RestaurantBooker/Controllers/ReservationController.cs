using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantBooker.Models;

namespace RestaurantBooker.Controllers
{
    public class ReservationController : Controller
    {

        private readonly UserDBContext _context;

        public ReservationController(UserDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // enusre it enter List，not null
            var reservations = await _context.Reservations.ToListAsync();
            return View(reservations);
        }

        // GET: /Reservation/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                // Save reservation logic here (e.g., to a database)
                return RedirectToAction("Index", "Reservation");
            }

            return View(reservation);
        }
    }
}
