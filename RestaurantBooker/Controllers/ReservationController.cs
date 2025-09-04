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

        public async Task<IActionResult> Index()
        {
            // enusre it enter List，not null
            var reservations = await _context.Reservations.ToListAsync();
            return View(reservations);
        }
      
        // GET: /Reservation/Create
        [HttpGet]
        public IActionResult Create()
        {

            return View();

        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var reservation = _context.Reservations.Find(id); // 根据主键查找
            if (reservation == null)
            {
                return NotFound(); // 如果不存在，返回 404
            }
            return View(reservation); // 把 category 传到 Delete 视图
        }


        //// POST: /Reservation/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Create(Reservation reservation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Save reservation logic here (e.g., to a database)
        //        _context.Reservations.Add(reservation);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index", "Reservation");
        //    }

        //    return View(reservation);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
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

                TempData["ReservationData"] = System.Text.Json.JsonSerializer.Serialize(reservation);

                return RedirectToAction("SelectTable", "Reservation");
            }

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmReservation(int tableId)
        {
            if (TempData["ReservationData"] == null)
            {
                return RedirectToAction("Create");
            }

            var reservationJson = TempData["ReservationData"].ToString();
            var reservation = System.Text.Json.JsonSerializer.Deserialize<Reservation>(reservationJson);

            reservation.TableId = tableId;

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult SelectTable()
        {
            // Need some form of if statement checking to see if form is filled out/completed
            var tables = _context.Tables.Where(t => t.IsAvailable).ToList();
            return View(tables);

        }


        public IActionResult Confirmtable()
        {
            //i need the slected table to be passed to the confirm reservation page
            // maybe a form that posts to confirm reservation with the table id
            // then the confirm reservation page can get the reservation from tempdata and the table from the form
            // then save the reservation with the table id
            // then redirect to the index page
            return RedirectToAction("Index");

        }


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
                // 如果因为外键约束无法删除，可以给用户提示
                ModelState.AddModelError("", "Unable to delete this reservation. It may be used by other records.");
                return View(reservation);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
