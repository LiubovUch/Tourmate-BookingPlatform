using Assignment1.Areas.BookingManagement.Filters;
using Assignment1.Areas.BookingManagement.Models;
using Assignment1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

namespace Assignment1.Areas.BookingManagement.Controllers
{
    [Area("BookingManagement")]
    [Route("[area]/[controller]/")]
    [ServiceFilter(typeof(LoggingFilter))]
    public class CarRentalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CarRentalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<IActionResult> Index(string carModel, string company, string sortOrder, string carType)
        {
            var carRentals = _context.CarRentals.ToList();

            var currentUser = await _userManager.GetUserAsync(User);
            var carPreferences = currentUser.CarPreferences;


            if (!string.IsNullOrEmpty(carModel))
            {
                carRentals = carRentals.Where(c => c.CarModel.Contains(carModel)).ToList();
            }
            if (!string.IsNullOrEmpty(company))
            {
                carRentals = carRentals.Where(c => c.RentalCompany.Contains(company)).ToList();
            }
            if (!string.IsNullOrEmpty(carType))
            {
                carRentals = carRentals.Where(c => c.CarType.Contains(carType)).ToList();
            }

            ViewData["PriceSortParm"] = string.IsNullOrEmpty(sortOrder) ? "price_asc" : "";
            switch (sortOrder)
            {
                case "price_asc":
                    carRentals = carRentals.OrderBy(c => c.Price).ToList();
                    break;
                case "price_desc":
                    carRentals = carRentals.OrderByDescending(c => c.Price).ToList();
                    break;
                default:
                    break;
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(carRentals);
            }

            return View(carRentals);
        }



        [HttpGet("{id:int}")]
        public IActionResult Details(int id)
        {
            var carRental = _context.CarRentals.FirstOrDefault(c => c.CarRentalId == id);
            if (carRental == null)
            {
                return NotFound();
            }
            return View(carRental);
        }

        [HttpGet("GetCarRentals")]
        public async Task<IActionResult> GetCarRentals()
        {
            var carRentals = await _context.CarRentals.ToListAsync();
            return Json(carRentals);
        }

        [HttpGet("Create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CarRental carRental)
        {
            if (ModelState.IsValid)
            {
                _context.CarRentals.Add(carRental);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(carRental);
        }

        [HttpGet("Edit/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var carRental = _context.CarRentals.Find(id);
            if (carRental == null)
            {
                return NotFound();
            }
            return View(carRental);
        }

        [HttpPost("Edit/{id:int}")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("CarRentalId,CarModel,RentalCompany,Price,AvailabilityStartDate,AvailabilityEndDate")] CarRental carRental)
        {
            if (id != carRental.CarRentalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carRental);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarRentalExists(carRental.CarRentalId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(carRental);
        }

        [HttpGet("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var carRental = _context.CarRentals.FirstOrDefault(c => c.CarRentalId == id);
            if (carRental == null)
            {
                return NotFound();
            }
            return View(carRental);
        }

        [HttpPost("DeleteConfirmed")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int carRentalId)
        {
            var carRental = _context.CarRentals.Find(carRentalId);
            if (carRental != null)
            {
                _context.CarRentals.Remove(carRental);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        private bool CarRentalExists(int id)
        {
            return _context.CarRentals.Any(c => c.CarRentalId == id);
        }

        [HttpGet("GetBestMatches")]
        public async Task<IActionResult> GetBestMatches(string carModel, string company, string carType, string sortOrder)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var carPreferences = currentUser.CarPreferences.Split(',');

            if (string.IsNullOrEmpty(carModel) && string.IsNullOrEmpty(company) && string.IsNullOrEmpty(carType) && string.IsNullOrEmpty(sortOrder))
            {

                var matchingCarRentals = _context.CarRentals
                    .Where(carRental => carPreferences.Any(pref => carRental.CarType.Contains(pref)))
                    .ToList();

                return Json(matchingCarRentals);
            }
            else
            {
                return Json(new CarRental[0]);
            }
        }

    }
}
