﻿using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Assignment1.Controllers
{
    public class CarRentalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarRentalController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index(string carModel, string company, string sortOrder)
        {
            // Retrieve all car rentals from the database
            var carRentals = _context.CarRentals.ToList();

            // Filter car rentals based on search criteria
            if (!string.IsNullOrEmpty(carModel))
            {
                carRentals = carRentals.Where(c => c.CarModel.Contains(carModel)).ToList();
            }
            if (!string.IsNullOrEmpty(company))
            {
                carRentals = carRentals.Where(c => c.RentalCompany.Contains(company)).ToList();
            }

            // Sorting logic
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

            return View(carRentals);
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var carRental = _context.CarRentals.FirstOrDefault(c => c.CarRentalId == id);
            if (carRental == null)
            {
                return NotFound();
            }
            return View(carRental);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
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

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var carRental = _context.CarRentals.Find(id);
            if (carRental == null)
            {
                return NotFound();
            }
            return View(carRental);
        }

        [HttpPost]
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

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var carRental = _context.CarRentals.FirstOrDefault(c => c.CarRentalId == id);
            if (carRental == null)
            {
                return NotFound();
            }
            return View(carRental);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
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
    }
}

