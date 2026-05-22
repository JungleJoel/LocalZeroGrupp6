using backend.Interfaces;
using backend.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<AccountProfileDto>> GetProfile()
        {
            var profile = await _accountService.GetProfileAsync(GetUserId());
            return Ok(profile);
        }

        [HttpPut("name")]
        public async Task<IActionResult> UpdateName([FromBody] UpdateNameDto dto)
        {
            await _accountService.UpdateNameAsync(GetUserId(), dto);
            return NoContent();
        }

        [HttpPut("email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto dto)
        {
            await _accountService.UpdateEmailAsync(GetUserId(), dto);
            return NoContent();
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            await _accountService.UpdatePasswordAsync(GetUserId(), dto);
            return NoContent();
        }

        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarDto dto)
        {
            await _accountService.UpdateAvatarAsync(GetUserId(), dto);
            return NoContent();
        }
    }
}