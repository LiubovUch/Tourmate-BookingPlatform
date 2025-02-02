﻿using System.ComponentModel.DataAnnotations;

namespace Assignment1.Areas.BookingManagement.Models
{
    public class Hotel
    {
        [Key]
        public int HotelId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Location { get; set; }

        public decimal Price { get; set; }

        public string? Amenities { get; set; }
        public string? PictureUrl { get; set; }
    }
}
