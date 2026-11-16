
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using ValiaApiWeb.DTOs.Responses;
using ValiaApiWeb.DTOs.Requests;
using ValiaApiWeb.Interfaces;

namespace ValiaApiWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IAuthService authService,IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            AuthResponse response = await _authService.RegisterAsync(request);

            return StatusCode(StatusCodes.Status201Created,response);
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            AuthResponse response = await _authService.LoginAsync(request);

            return Ok(response);
        }

        // GET: api/Auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> Me()
        {
            // Obtém o identificador do usuário a partir do JWT
            string? userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            // Consulta o usuário autenticado
            var user = await _userService.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            // Retorna somente informações públicas da conta
            var response = new UserResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return Ok(response);
        }
    }
}

