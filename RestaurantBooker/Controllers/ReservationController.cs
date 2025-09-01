using Microsoft.AspNetCore.Mvc;
using RestaurantBooker.Models;

namespace RestaurantBooker.Controllers
{
    public class ReservationController : Controller
    {
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
                return RedirectToAction("Index", "Home");
            }

            return View(reservation);
        }
    }
}
