using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Index() 

        {
            return View();
        }
    }
}
