using System.Security.Claims;
using backend.Data;
using backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _database;

    public AccountController(ApplicationDbContext database)
    {
        _database = database;
    }

    private Guid GetUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /Account/me
    [HttpGet("me")]
    public async Task<ActionResult<AccountProfileDto>> GetProfile()
    {
        var user = await _database.Users.FindAsync(GetUserId());
        if (user is null) return NotFound();

        return Ok(new AccountProfileDto(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.AvatarImageUrl,
            user.CreatedAt
        ));
    }

    // PUT /Account/name
    [HttpPut("name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateNameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            return BadRequest("First name and last name are required.");

        var user = await _database.Users.FindAsync(GetUserId());
        if (user is null) return NotFound();

        user.FirstName = dto.FirstName.Trim();
        user.LastName = dto.LastName.Trim();

        await _database.SaveChangesAsync();
        return NoContent();
    }

    // PUT /Account/email
    [HttpPut("email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NewEmail))
            return BadRequest("Email is required.");

        var user = await _database.Users.FindAsync(GetUserId());
        if (user is null) return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest("Current password is incorrect.");

        var emailTaken = await _database.Users.AnyAsync(u => u.Email == dto.NewEmail && u.Id != user.Id);
        if (emailTaken) return Conflict("Email is already in use.");

        user.Email = dto.NewEmail.Trim().ToLower();

        await _database.SaveChangesAsync();
        return NoContent();
    }

    // PUT /Account/password
    [HttpPut("password")]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmNewPassword)
            return BadRequest("Passwords do not match.");

        if (dto.NewPassword.Length < 8)
            return BadRequest("Password must be at least 8 characters.");

        var user = await _database.Users.FindAsync(GetUserId());
        if (user is null) return NotFound();

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return BadRequest("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

        await _database.SaveChangesAsync();
        return NoContent();
    }

    // PUT /Account/avatar
    [HttpPut("avatar")]
    public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.AvatarImageUrl))
            return BadRequest("Avatar URL is required.");

        if (!Uri.TryCreate(dto.AvatarImageUrl, UriKind.Absolute, out _))
            return BadRequest("Invalid URL format.");

        var user = await _database.Users.FindAsync(GetUserId());
        if (user is null) return NotFound();

        user.AvatarImageUrl = dto.AvatarImageUrl;

        await _database.SaveChangesAsync();
        return NoContent();
    }
}