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

        // GET: /Reservation/Update/{id}
        [HttpGet]
        public IActionResult Update(int id)
        {
            var reservation = _context.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound();
            }

            // Pass list of tables to view
            ViewBag.Tables = _context.Tables
                .Select(t => new
                {
                    Id = t.Id,
                    Display = $"Table {t.Id} (Seats {t.Capacity})"
                })
                .ToList();

            return View(reservation);
        }


        // POST: /Reservation/Update/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, Reservation updatedReservation)
        {
            if (id != updatedReservation.Id)
            {
                return BadRequest();
            }

            // Always repopulate tables in case of returning the view
            ViewBag.Tables = _context.Tables
                .Select(t => new
                {
                    Id = t.Id,
                    Display = $"Table {t.Id} (Seats {t.Capacity})"
                })
                .ToList();

            if (updatedReservation.NumberOfGuests > 12)
            {
                ModelState.AddModelError("NumberOfGuests", "Number of guests exceeded");
                return View(updatedReservation);
            }

            if (!ModelState.IsValid)
            {
                return View(updatedReservation);
            }

            // Recalculate times
            var startDateTime = updatedReservation.Date.Date + updatedReservation.Time;
            var endDateTime = startDateTime.AddHours(2);

            // Validate table capacity
            var table = _context.Tables.Find(updatedReservation.TableId);
            if (table == null || updatedReservation.NumberOfGuests > table.Capacity)
            {
                ModelState.AddModelError("", "Selected table does not have enough seats.");
                return View(updatedReservation);
            }

            // Check overlap with other reservations
            bool isTaken = _context.Reservations.Any(r =>
                r.Id != updatedReservation.Id && // exclude current one
                r.TableId == updatedReservation.TableId &&
                r.Date == updatedReservation.Date &&
                (r.StartTime < endDateTime && r.EndTime > startDateTime)
            );

            if (isTaken)
            {
                ModelState.AddModelError("", "That table is already booked at the selected time.");
                return View(updatedReservation);
            }

            // Save
            updatedReservation.StartTime = startDateTime;
            updatedReservation.EndTime = endDateTime;

            _context.Update(updatedReservation);
            _context.SaveChanges();

            //Redirect after success
            return RedirectToAction("Details", new { id = updatedReservation.Id });
        }



    }
}
