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

        // Show all reservations
        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations.ToListAsync();
            return View(reservations);
        }

        // GET: /Reservation/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reservation reservation)
        {
            if (reservation.NumberOfGuests > 12)
            {
                ModelState.AddModelError("NumberOfGuests", "Number of guests exceeded");
                return View(reservation);
            }

            if (ModelState.IsValid)
            {
                return View("SelectTable", new SelectTableViewModel
                {
                    Reservation = reservation,
                    Tables = GetTablesForReservation(reservation)
                });
            }

            return View(reservation);
        }

        // POST: /Reservation/ConfirmReservation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmReservation(Reservation reservation)
        {
            var table = _context.Tables.Find(reservation.TableId);
            if (table == null || reservation.NumberOfGuests > table.Capacity)
            {
                ModelState.AddModelError("", "Selected table does not have enough seats.");
                return View("SelectTable", new SelectTableViewModel
                {
                    Reservation = reservation,
                    Tables = GetTablesForReservation(reservation)
                });
            }

            var startDateTime = reservation.Date.Date + reservation.Time;
            var endDateTime = startDateTime.AddHours(2);

            bool isTaken = _context.Reservations.Any(r =>
                r.TableId == reservation.TableId &&
                r.Date == reservation.Date &&
                (r.StartTime < endDateTime && r.EndTime > startDateTime)
            );

            if (isTaken)
            {
                ModelState.AddModelError("", "That table is already booked at the selected time.");
                return View("SelectTable", new SelectTableViewModel
                {
                    Reservation = reservation,
                    Tables = GetTablesForReservation(reservation)
                });
            }

            reservation.StartTime = startDateTime;
            reservation.EndTime = endDateTime;

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = reservation.Id });
        }


        // GET: /Reservation/Delete/{id}
        [HttpGet]
        public IActionResult Delete(int id)
        {

            //i need the slected table to be passed to the confirm reservation page
            // maybe a form that posts to confirm reservation with the table id
            // then the confirm reservation page can get the reservation from tempdata and the table from the form
            // then save the reservation with the table id
            // then redirect to the index page

            return RedirectToAction("Index");

            var reservation = _context.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: /Reservation/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound();
            }

            try
            {
                _context.Reservations.Remove(reservation);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to delete this reservation. It may be used by other records.");
                return View(reservation);
            }

            return RedirectToAction(nameof(Index));
        }

        // helper: get tables with availability info
        private IEnumerable<Table> GetTablesForReservation(Reservation reservation)
        {
            var startDateTime = reservation.Date.Date + reservation.Time;
            var endDateTime = startDateTime.AddHours(2);

            var tables = _context.Tables.ToList();

            foreach (var table in tables)
            {
                bool isTaken = _context.Reservations.Any(r =>
                    r.TableId == table.Id &&
                    r.Date == reservation.Date &&
                    (r.StartTime < endDateTime && r.EndTime > startDateTime)
                );

                table.IsAvailable = !isTaken && table.Capacity >= reservation.NumberOfGuests;
            }

            return tables;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Table) // include table details
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }


    }
}
