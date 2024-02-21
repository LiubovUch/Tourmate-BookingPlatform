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
        public IActionResult Index()
        {
            var hotels = _context.Hotels.ToList();
            return View(hotels);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var hotels = _context.Hotels.FirstOrDefault(p => p.HotelId == id);
            if (hotels == null)
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
        public IActionResult Create(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.Hotels.Add(hotel);
                _context.SaveChanges();
                return RedirectToAction("Index");
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
        public IActionResult Edit(int id, [Bind("HotelId, Name, Location, Price, Amenities")] Hotel hotels)
        {
            if (id != hotels.HotelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotels);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotels.HotelId))
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
            return View(hotels);
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
            var hotel = _context.Hotels.Find(HotelId);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
