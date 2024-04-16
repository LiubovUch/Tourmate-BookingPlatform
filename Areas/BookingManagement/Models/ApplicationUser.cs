using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public int UsernameChangeLimit { get; set; } = 10;

    public byte[]? ProfilePicture { get; set; }

    // Set default values for preferences
    public string FrequentFlyerNumber { get; set; } = "None";
    public string? CarPreferences { get; set; } = "None";
    public string? HotelPreferences { get; set; } = "None";

}
