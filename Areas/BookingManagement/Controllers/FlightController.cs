using Assignment1.Areas.BookingManagement.Filters;
using Assignment1.Areas.BookingManagement.Models;
using Assignment1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;

namespace Assignment1.Areas.BookingManagement.Controllers
{
    [Area("BookingManagement")]
    [Route("[area]/[controller]")]
    [ServiceFilter(typeof(LoggingFilter))]
    public class FlightController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlightController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index(string airline, string departureLocation, string arrivalLocation, DateTime? departureTime)
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

            var filteredFlights = await flights.ToListAsync(); // Make sure to await ToListAsync

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(filteredFlights);
            }

            return View(filteredFlights);
        }
        [HttpGet("GetFlightData")]
        public async Task<IActionResult> GetFlightData(int id)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == id);

            if (flight == null)
            {
                return NotFound();
            }

            return Json(flight);
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Flight flight)
        {
            if (ModelState.IsValid)
            {
                _context.Flights.Add(flight);
                _context.SaveChanges();
                Log.Information("New flight created: {FlightId}", flight.FlightId);
                return RedirectToAction(nameof(Index));
            }
            return View(flight);
        }

        [HttpGet("Edit/{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var flight = _context.Flights.Find(id);
            if (flight == null)
            {
                return NotFound();
            }
            return PartialView("_Edit", flight);
        }
        [HttpPost("Edit/{id:int}")]
        [Authorize(Roles = "Admin")]
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
                    return PartialView("_FlightDetails", flight); // Return partial view with updated flight details
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
            }
            return PartialView("_Edit", flight);
        }
        [HttpPost("Update")]
        public async Task<IActionResult> Update(Flight flight)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the flight from the database
                var existingFlight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == flight.FlightId);

                if (existingFlight == null)
                {
                    return NotFound();
                }

                // Update the flight details
                existingFlight.Airline = flight.Airline;
                existingFlight.DepartureLocation = flight.DepartureLocation;
                existingFlight.ArrivalLocation = flight.ArrivalLocation;
                existingFlight.DepartureTime = flight.DepartureTime;
                existingFlight.ArrivalTime = flight.ArrivalTime;
                existingFlight.Price = flight.Price;

                // Save changes to the database
                _context.Entry(existingFlight).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // Optionally, you can return a JSON response indicating success
                return Json(new { success = true });
            }
            else
            {
                // If model state is not valid, return validation errors
                return BadRequest(ModelState);
            }
        }

        [HttpGet("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [HttpGet("GetFlights")]
        public async Task<IActionResult> GetFlights()
        {
            var flights = await _context.Flights.ToListAsync();
            return Json(flights);
        }


    }
}
