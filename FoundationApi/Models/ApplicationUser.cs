using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
  // phoneNumber is already in IdentityUser, but let's add:
  public string? PhotoUrl { get; set; } // Optional photo
                                        // If you want a custom field for phone, you could override or create a new property
                                        // public string? PhoneNumber { get; set; } // IdentityUser includes it, but you can keep your own if you prefer

  // Example of storing a code for email confirmation manually (if not using the built-in token system):
  // public string? EmailConfirmationCode { get; set; }
}
