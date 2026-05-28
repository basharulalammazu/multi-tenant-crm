using BLL.DTOs.Shared;
using BLL.Services;
using DAL.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("getById/{id}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Success = false, Message = "Invalid user ID" });

                string msg = string.Empty;
                var user = userService.GetById(id, out msg);

                if (!string.IsNullOrWhiteSpace(msg))
                    return BadRequest(new { Success = false, Message = msg });

                if (user == null)
                    return NotFound(new { Success = false, Message = "User not found" });

                return Ok(new { Success = true, Data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Error retrieving user: {ex.Message}" });
            }
        }

        [HttpGet("allUser")]
        public IActionResult GetAll()
        {
            try
            {
                string msg = string.Empty;
                var users = userService.GetAll(out msg);

                if (!string.IsNullOrWhiteSpace(msg))
                    return BadRequest(new { Success = false, Message = msg });

                if (users == null || users.Count == 0)
                    return NotFound(new { Success = false, Message = "No users found" });

                return Ok(new { Success = true, Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Error retrieving all users: {ex.Message}" });
            }
        }

        [HttpGet("userByRole/{role}")]
        public IActionResult GetUserByRole(UserRole role)
        {
            try
            {
                string msg = string.Empty;
                var users = userService.GetUserByRole(role, out msg);

                if (!string.IsNullOrWhiteSpace(msg))
                    return BadRequest(new { Success = false, Message = msg });

                if (users == null || users.Count == 0)
                    return NotFound(new { Success = false, Message = "No users found for this role" });

                return Ok(new { Success = true, Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Error retrieving users by role: {ex.Message}" });
            }
        }

        [HttpGet("userByTenantId/{id}")]
        public IActionResult GetUserByTenant(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                    return BadRequest(new { Success = false, Message = "Invalid tenant ID" });

                string msg = string.Empty;
                var users = userService.GetUserByTenant(id, out msg);

                if (!string.IsNullOrWhiteSpace(msg))
                    return BadRequest(new { Success = false, Message = msg });

                if (users == null || users.Count == 0)
                    return NotFound(new { Success = false, Message = "No users found for this tenant" });

                return Ok(new { Success = true, Data = users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Error retrieving users by tenant: {ex.Message}" });
            }
        }

        [HttpPost("createUser")]
        public IActionResult RegisterUser(CreateUserDTO user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Success = false, Message = "Invalid input data", Errors = ModelState });

                if (user == null)
                    return BadRequest(new { Success = false, Message = "User data is required" });

                if (string.IsNullOrWhiteSpace(user.Email))
                    return BadRequest(new { Success = false, Message = "Email is required" });

                if (string.IsNullOrWhiteSpace(user.PasswordHash))
                    return BadRequest(new { Success = false, Message = "Password is required" });

                string msg = string.Empty;

                // Do NOT hash password here - the service will handle it
                var createdUser = userService.RegisterUser(user, out msg);

                if (!string.IsNullOrWhiteSpace(msg) && !createdUser)
                    return BadRequest(new { Success = false, Message = msg });

                if (!createdUser)
                    return BadRequest(new { Success = false, Message = "Failed to create user" });

                return Ok(new { Success = true, Message = msg, Data = createdUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Error creating user: {ex.Message}" });
            }
        }
    }
}