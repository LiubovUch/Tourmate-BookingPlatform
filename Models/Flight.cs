using System.ComponentModel.DataAnnotations;

namespace Assignment1.Models
{
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }

        [Required]
        public string Airline { get; set; }

        [Required]
        public string DepartureLocation { get; set; }

        [Required]
        public string ArrivalLocation { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DepartureTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ArrivalTime { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
