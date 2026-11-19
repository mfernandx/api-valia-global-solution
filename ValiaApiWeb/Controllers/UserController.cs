using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.Interfaces;
using ValiaApiWeb.Services.Interfaces;

namespace ValiaApiWeb.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // ATUALIZAR E-MAIL
    [HttpPatch("me/email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateUserEmailRequest request, CancellationToken cancellationToken)
    {
        Guid userId = GetAuthenticatedUserId();
        await _userService.UpdateEmailAsync(userId, request.Email);
        return NoContent();
    }

    // ATUALIZAR TELEFONE
    [HttpPatch("me/phone")]
    public async Task<IActionResult> UpdatePhone([FromBody] UpdateUserPhoneRequest request, CancellationToken cancellationToken)
    {
        Guid userId = GetAuthenticatedUserId();
        await _userService.UpdatePhoneNumberAsync(userId, request.PhoneNumber);
        return NoContent();
    }

    // DESATIVAR CONTA
    [HttpPatch("me/deactivate")]
    public async Task<IActionResult> Deactivate(CancellationToken cancellationToken)
    {
        Guid userId = GetAuthenticatedUserId();
        await _userService.DeactivateAsync(userId);
        return NoContent();
    }

    // IDENTIFICAR USUÁRIO AUTENTICADO
    private Guid GetAuthenticatedUserId()
    {
        string? userId = User.FindFirstValue("sub");

        if (!Guid.TryParse(userId, out Guid parsedUserId))
            throw new UnauthorizedAccessException("Usuário não autenticado.");

        return parsedUserId;
    }
}

