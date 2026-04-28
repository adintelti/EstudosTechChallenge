using FCG.Users.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FCG.Users.API.Controllers;

[ApiController]
[Route("users/{userId}/games")]
[Authorize]
public class UserGamesController : ControllerBase
{
    private readonly UserGameService _service;

    public UserGamesController(UserGameService service)
    {
        _service = service;
    }

    [HttpPost("{gameId}")]
    public async Task<IActionResult> AcquireGame(
        Guid userId,
        Guid gameId)
    {
        if (!HasLibraryAccess(userId))
            return Forbid();

        await _service.AcquireGameAsync(userId, gameId);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetLibrary(Guid userId)
    {
        if (!HasLibraryAccess(userId))
            return Forbid();

        var response = await _service.GetLibraryAsync(userId);

        return Ok(response);
    }

    private bool HasLibraryAccess(Guid userId)
    {
        var loggedUserId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var role =
            User.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "Admin")
            return true;

        return loggedUserId == userId.ToString();
    }
}