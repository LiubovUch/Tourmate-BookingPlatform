﻿using Assignment1.Data;
using Assignment1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment1.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public ActionResult Index(int? flightId, int? hotelId, int? carRentalId)
        {
            var selectedFlight = _context.Flights.FirstOrDefault(f => f.FlightId == flightId);
            var selectedHotel = _context.Hotels.FirstOrDefault(h => h.HotelId == hotelId);
            var selectedCarRental = _context.CarRentals.FirstOrDefault(c => c.CarRentalId == carRentalId);

            var bookingViewModel = new BookingViewModel
            {
                SelectedFlight = selectedFlight,
                SelectedHotel = selectedHotel,
                SelectedCarRental = selectedCarRental
            };

            return View(bookingViewModel);
        }
    }
}
