using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
{
    public class CarRental
    {
        [Key]
        public int CarRentalId { get; set; }

        [Required]
        public string? CarModel { get; set; }

        [Required]
        public string? RentalCompany { get; set; }

        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        public DateTime AvailabilityStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime AvailabilityEndDate { get; set; }
    }
}
