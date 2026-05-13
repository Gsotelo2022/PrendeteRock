using Microsoft.AspNetCore.Mvc;
using PrendeteRock.API.Services;

namespace PrendeteRock.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
                return BadRequest("Email, contraseña y nombre son requeridos");

            try
            {
                var user = await _authService.RegisterAsync(request.Email, request.Password, request.FullName);
                var token = _authService.GenerateToken(user);

                return Ok(new 
                { 
                    message = "Usuario registrado", 
                    token = token,
                    userId = user.Id, 
                    email = user.Email,
                    fullName = user.FullName,
                    isAdmin = user.IsAdmin
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _authService.LoginAsync(request.Email, request.Password);
                if (user == null)
                    return Unauthorized("Credenciales inv谩lidas");

                var token = _authService.GenerateToken(user);

                return Ok(new 
                { 
                    message = "Login exitoso", 
                    token = token,
                    userId = user.Id, 
                    email = user.Email,
                    fullName = user.FullName,
                    isAdmin = user.IsAdmin
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}