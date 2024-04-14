using Assignment1.Areas.BookingManagement.Models;
using Assignment1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace Assignment1.Areas.BookingManagement.Controllers
{
    [Area("BookingManagement")]
    [Route("[area]/[controller]")]
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string airline, string departureLocation, string arrivalLocation, DateTime? departureTime)
        {
            var flights = _context.Flights.AsQueryable();

            if (!string.IsNullOrEmpty(airline))
            {
                flights = flights.Where(f => f.Airline.Contains(airline));
            }
            if (!string.IsNullOrEmpty(departureLocation))
            {
                flights = flights.Where(f => f.DepartureLocation.Contains(departureLocation));
            }
            if (!string.IsNullOrEmpty(arrivalLocation))
            {
                flights = flights.Where(f => f.ArrivalLocation.Contains(arrivalLocation));
            }
            if (departureTime.HasValue)
            {
                flights = flights.Where(f => f.DepartureTime.Date == departureTime.Value.Date);
            }

            var filteredFlights = flights.ToList();
            return View(filteredFlights);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult Details(int id)
        {
            var flight = _context.Flights.FirstOrDefault(f => f.FlightId == id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        [HttpGet("Edit/{id:int}")]
        public IActionResult Edit(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("FlightId,Airline,DepartureLocation,ArrivalLocation,DepartureTime,ArrivalTime,Price")] Flight flight)
        {
            if (id != flight.FlightId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.FlightId))
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
            return View(flight);
        }

        [HttpGet("Delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            var flight = _context.Flights.FirstOrDefault(f => f.FlightId == id);
            if (flight == null)
            {
                return NotFound();
            }
            return View(flight);
        }

        [HttpPost("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int flightId)
        {
            var flight = _context.Flights.Find(flightId);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        private bool FlightExists(int id)
        {
            return _context.Flights.Any(f => f.FlightId == id);
        }
    }
}
