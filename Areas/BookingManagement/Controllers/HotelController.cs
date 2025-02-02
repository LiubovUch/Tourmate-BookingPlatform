﻿using Assignment1.Areas.BookingManagement.Filters;
using Assignment1.Areas.BookingManagement.Models;
using Assignment1.Data;
using Assignment1.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assignment1.Areas.BookingManagement.Controllers
{
    [Area("BookingManagement")]
    [Route("[area]/[controller]")]
    [ServiceFilter(typeof(LoggingFilter))]
    public class HotelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; 

        public HotelController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string name, string location, string sortOrder)
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

            var currentUser = await _userManager.GetUserAsync(User);
            var hotelPreferences = currentUser.HotelPreferences; 

            ViewData["PriceSortParm"] = string.IsNullOrEmpty(sortOrder) ? "price_asc" : "";
            switch (sortOrder)
            {
                case "price_asc":
                    hotels = hotels.OrderBy(h => h.Price).ToList();
                    break;
                case "price_desc":
                    hotels = hotels.OrderByDescending(h => h.Price).ToList();
                    break;
                default:
                    break;
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(hotels);
            }

            return View(hotels);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult Details(int id)
        {
            var hotel = _context.Hotels.FirstOrDefault(p => p.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Hotel hotel, string[] amenities)
        {
            if (ModelState.IsValid)
            {
                hotel.Amenities = amenities != null ? string.Join(", ", amenities) : null;
                _context.Hotels.Add(hotel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            var hotel = _context.Hotels.Find(id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [HttpPost("Edit/{id:int}")]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var hotel = _context.Hotels.FirstOrDefault(p => p.HotelId == id);
            if (hotel == null)
            {
                return NotFound();
            }
            return View(hotel);
        }

        [HttpPost("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int hotelId)
        {
            var hotel = _context.Hotels.Find(hotelId);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        private bool HotelExists(int id)
        {
            return _context.Hotels.Any(e => e.HotelId == id);
        }
        [HttpGet("GetBestMatches")]
        public async Task<IActionResult> GetBestMatches()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var hotelPreferences = currentUser.HotelPreferences.Split(','); 

            var matchingHotels = _context.Hotels
                .Where(hotel => hotelPreferences.Any(pref => hotel.Amenities.Contains(pref)))
                .ToList();

            return Json(matchingHotels);
        }

        [HttpGet("GetHotels")]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await _context.Hotels.ToListAsync();
            return Json(hotels);
        }

    }
}
