using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
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

        public string? ImageUrl { get; set; }
    }
}
