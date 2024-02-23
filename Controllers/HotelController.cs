using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Assignment1.Controllers
{
    public class HotelController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HotelController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index(string name, string location, string sortOrder)
        {
            var hotels = _context.Hotels.ToList();
            if (!string.IsNullOrEmpty(name))
            {
                hotels = hotels.Where(h => h.Name.Contains(name)).ToList();
            }
            if (!string.IsNullOrEmpty(location))
            {
                hotels = hotels.Where(h => h.Location.Contains(location)).ToList();
            }
            ViewData["PriceSortParm"] = string.IsNullOrEmpty(sortOrder) ? "price_asc" : ""; switch (sortOrder)
            {
                case "price_asc":
                    hotels = hotels.OrderBy(h => h.Price).ToList(); break;
                case "price_desc":
                    hotels = hotels.OrderByDescending(h => h.Price).ToList();
                    break;
                default:
                    break;
            }
            return View(hotels);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var hotels = _context.Hotels.FirstOrDefault(p => p.HotelId == id); if (hotels == null)
            {
                return NotFound();
            }
            return View(hotels);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Hotel hotel, string[] amenities)
        {
            if (ModelState.IsValid)
            {
                hotel.Amenities = amenities != null ? string.Join(", ", amenities) : null;
                _context.Hotels.Add(hotel);
                _context.SaveChanges(); return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var hotels = _context.Hotels.Find(id);
            if (hotels == null)
            {
                return NotFound();
            }
            return View(hotels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("HotelId, Name, Location, Price, Amenities, PictureUrl")] Hotel hotel, string[] Amenities)
        {
            if (id != hotel.HotelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingHotel = _context.Hotels.Find(id);
                    if (existingHotel == null)
                    {
                        return NotFound();
                    }

                    existingHotel.Name = hotel.Name;
                    existingHotel.Location = hotel.Location;
                    existingHotel.Price = hotel.Price;
                    existingHotel.PictureUrl = hotel.PictureUrl;
                    existingHotel.Amenities = Amenities != null ? string.Join(",", Amenities) : null;

                    _context.Update(existingHotel);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.HotelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(hotel);
        }




        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelId == id);
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var hotels = _context.Hotels.FirstOrDefault(p => p.HotelId == id);
            if (hotels == null)
            {
                return NotFound();
            }
            return View(hotels);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int HotelId)
        {
            var hotel = _context.Hotels.Find(HotelId); if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                _context.SaveChanges(); return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}