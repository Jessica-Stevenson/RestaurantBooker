using Microsoft.AspNetCore.Mvc;

namespace RestaurantBooker.Controllers
{
    public class MenuController : Controller
    {
        // This action will handle the main menu page (e.g., /Menu/Index)
        public IActionResult Index()
        {
            return View();
        }

        // This action will handle the starters and mains page
        public IActionResult StartersMains()
        {
            return View();
        }

        // This action will handle the desserts and beverages page
        public IActionResult DessertsBeverages()
        {
            return View();
        }
    }
}