// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Assignment1.Areas.BookingManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Assignment1.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }
        [TempData]
        public string UserChangeLimitStatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Username")]
            public string Username { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile Picture")]
            public byte[] ProfilePicture { get; set; }

            [Display(Name = "Frequent Flyer Number")]
            public string FrequentFlyerNumber { get; set; }

            [Display(Name = "Hotel Preferences")]
            public string HotelPreferences { get; set; }

            [Display(Name = "Car Preferences")]
            public string CarPreferences { get; set; }
        }


        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var profilePicture = user.ProfilePicture;
            var frequentFlyerNumber = user.FrequentFlyerNumber;
            var hotelPreferences = user.HotelPreferences; 
            var carPreferences = user.CarPreferences; 

            Username = userName;
            Input = new InputModel
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Username = userName,
                ProfilePicture = profilePicture,
                FrequentFlyerNumber = frequentFlyerNumber,
                HotelPreferences = hotelPreferences, 
                CarPreferences = carPreferences, 
            };


        }


        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var frequentFlyerNumber = user.FrequentFlyerNumber;
            var carPreferences = user.CarPreferences;
            var hotelPreferences = user.HotelPreferences; ;
            Input = new InputModel
            {
                FrequentFlyerNumber = frequentFlyerNumber,

            };
            Input.FrequentFlyerNumber = frequentFlyerNumber;
            ViewData["CheckedAmenities"] = new List<string>(); 
            ViewData["CheckedCarPreferences"] = new List<string>(); 
            ViewData["CheckedHotelPreferences"] = new List<string>(); 


            if (!string.IsNullOrEmpty(carPreferences))
            {
                var preferences = carPreferences.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var preference in preferences)
                {
                    ((List<string>)ViewData["CheckedCarPreferences"]).Add(preference.Trim());
                }
            }

            if (!string.IsNullOrEmpty(hotelPreferences))
            {
                var preferences = hotelPreferences.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var preference in preferences)
                {
                    ((List<string>)ViewData["CheckedHotelPreferences"]).Add(preference.Trim());
                }
            }
            UserChangeLimitStatusMessage = $"You can change your name {user.UsernameChangeLimit} more times";
            await LoadAsync(user);
            return Page();
        }



        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (user.UsernameChangeLimit > 0)
            {
                if (Input.Username != user.UserName)
                {
                    if (string.IsNullOrEmpty(Input.Username))
                    {
                        StatusMessage = "Error: Username cannot be null or empty.";
                        return RedirectToPage();
                    }

                    var userNameExists = await _userManager.FindByNameAsync(Input.Username);
                    if (userNameExists != null)
                    {
                        StatusMessage = "Error: Username is already taken. Please choose a different username.";
                        return RedirectToPage();
                    }

                    var setUserName = await _userManager.SetUserNameAsync(user, Input.Username);
                    if (!setUserName.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set user name";
                        return RedirectToPage();
                    }
                    else
                    {
                        user.UserName = Input.Username;
                        user.UsernameChangeLimit -= 1;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }

            var firstName = user.FirstName;
            if (Input.FirstName != firstName)
            {
                user.FirstName = Input.FirstName;
            }

            var lastName = user.LastName;
            if (Input.LastName != lastName)
            {
                user.LastName = Input.LastName;
            }
            user.CarPreferences = Input.CarPreferences;
            user.HotelPreferences = Input.HotelPreferences;
            var hotelPreferences = Request.Form["Input.HotelPreferences"].Select(p => p).ToArray();
            if (hotelPreferences.Length > 0)
            {
                user.HotelPreferences = string.Join(",", hotelPreferences);
            }
            else
            {
                user.HotelPreferences = null; 
            }

            var carPreferences = Request.Form["Input.CarPreferences"].Select(p => p).ToArray();
            if (carPreferences.Length > 0)
            {
                user.CarPreferences = string.Join(",", carPreferences);
            }
            else
            {
                user.CarPreferences = null; 
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to update user profile.";
                return RedirectToPage();
            }
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    user.ProfilePicture = dataStream.ToArray();
                }
                await _userManager.UpdateAsync(user);
            }

            updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to update user profile picture.";
                return RedirectToPage();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }


    }
}
