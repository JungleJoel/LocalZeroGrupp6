using System.Security.Claims;
using backend.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

public abstract class BaseController : ControllerBase
{
    protected Guid GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(claim))
            throw new UnauthorizedException("User ID not found in token.");
        return Guid.Parse(claim);
    }
}