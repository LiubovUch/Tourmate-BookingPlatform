using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers
{
    [Route("ErrorPage/{statuscode}")]
    public class ErrorPageController : Controller
    {
        public IActionResult Index(int statusocde)
        {
            switch (statusocde)
            {
                case 404:
                    ViewData["Error"] = "Page Not Found";
                    break;
                default:
                    break;
            }
            return View("ErrorPage");
        }

        //public IActionResult PageNotFound() 
        //{
        //    return View();
        //}
    }
}
