using Microsoft.AspNetCore.Identity;

namespace Assignment1.Areas.BookingManagement.Models
{
    public class ApplicationUser: IdentityUser
    {
        public String FirstName { get; set; }
        public string LastName { get; set; }

        public int UsernameChangeLimit { get; set; } = 10;

        public byte[]? ProfilePicture { get; set; }
    }
}
