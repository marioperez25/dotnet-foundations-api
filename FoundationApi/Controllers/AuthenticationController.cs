using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly IConfiguration _config;

  public AuthController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IConfiguration config)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _config = config;
  }

  // Example register model
  public class RegisterRequest
  {
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Password { get; set; }
    public string? PhotoUrl { get; set; }  // optional
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest model)
  {
    if (!ModelState.IsValid) return BadRequest(ModelState);

    // Create the user entity
    var user = new ApplicationUser
    {
      UserName = model.Username,
      Email = model.Email,
      PhoneNumber = model.PhoneNumber,
      PhotoUrl = model.PhotoUrl // optional
    };

    // Create the user in the database
    var result = await _userManager.CreateAsync(user, model.Password);
    if (!result.Succeeded)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(error.Code, error.Description);
      }
      return BadRequest(ModelState);
    }

    // Generate email confirmation token
    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

    // In a real app, you’d send an email with a link containing this token.
    // E.g., "https://yourdomain.com/api/auth/confirm-email?userId=XXX&token=YYY"
    // For a quick test, we could just return it to the client:
    return Ok(new { Message = "User created. Please confirm email.", EmailConfirmationToken = token });
  }

  [HttpGet("confirm-email")]
  public async Task<IActionResult> ConfirmEmail(string email, string token)
  {
    if (email == null || token == null)
    {
      return BadRequest("Invalid confirmation data");
    }
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null) return NotFound("User not found for that email");

    var result = await _userManager.ConfirmEmailAsync(user, token);
    if (result.Succeeded)
    {
      return Ok("Email confirmed successfully!");
    }
    else
    {
      return BadRequest("Error confirming your email.");
    }
  }

  public class LoginRequest
  {
    public required string Email { get; set; }
    public required string Password { get; set; }
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest model)
  {
    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null) return Unauthorized("Invalid email or password.");

    // Optionally, check if email is confirmed:
    if (!await _userManager.IsEmailConfirmedAsync(user))
    {
      return BadRequest("Email not confirmed. Please confirm your email first.");
    }

    // Check the password
    var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
    if (!passwordCheck.Succeeded) return Unauthorized("Invalid email or password.");

    // Generate JWT
    var tokenString = GenerateJwtToken(user);

    return Ok(new { token = tokenString });
  }

  private string GenerateJwtToken(ApplicationUser user)
  {
    var jwtKey = _config["JwtSettings:Key"];
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // Add whatever claims you want, e.g. role claims, or custom user info
    var claims = new[]
    {
          new Claim(JwtRegisteredClaimNames.Sub, user.Id),
          new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
          new Claim("username", user.UserName)
      };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  [HttpPost("forgot-password")]
  public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
  {
    // model just needs the Email
    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null)
      return Ok("If that email exists, a reset email has been sent.");
    // For security: don't reveal if user doesn't exist

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

    // Send email with token or a link with the token in the query
    // e.g. "https://yourfrontend/reset?userId=xyz&token=abc"

    return Ok(new { Message = "Password reset link sent. Check your email.", ResetToken = token });
  }

  public class ForgotPasswordRequest
  {
    public required string Email { get; set; }
  }

  [HttpPost("reset-password")]
  public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
  {
    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null) return BadRequest("Invalid request.");

    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
    if (!result.Succeeded)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(error.Code, error.Description);
      }
      return BadRequest(ModelState);
    }
    return Ok("Password has been reset successfully.");
  }

  public class ResetPasswordRequest
  {
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
  }

  [Authorize]
  [HttpPut("update-user")]
  public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest model)
  {
    // The token should contain the user's ID or email. Let's assume the user ID is stored in "sub" claim:
    var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    if (userId == null) return Unauthorized();

    var user = await _userManager.FindByIdAsync(userId);
    if (user == null) return NotFound();

    // Update fields
    user.UserName = model.Username ?? user.UserName;
    user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
    user.PhotoUrl = model.PhotoUrl ?? user.PhotoUrl;

    // Update user in DB
    var result = await _userManager.UpdateAsync(user);
    if (!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }
    return Ok("User updated successfully.");
  }

  public class UpdateUserRequest
  {
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PhotoUrl { get; set; }
  }

}
