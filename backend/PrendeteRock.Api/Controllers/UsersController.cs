using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrendeteRock.API.Services;

namespace PrendeteRock.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersWithStatsAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuarios", error = ex.Message });
            }
        }

        // GET: api/users/clients
        [HttpGet("clients")]
        public async Task<IActionResult> GetClients()
        {
            try
            {
                var clients = await _userService.GetClientsWithStatsAsync();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener clientes", error = ex.Message });
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener usuario", error = ex.Message });
            }
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Usuario eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar usuario", error = ex.Message });
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, request.FullName, request.Email, request.Phone);
                if (user == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(new { message = "Usuario actualizado", user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar usuario", error = ex.Message });
            }
        }
    }

    public class UpdateUserRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
