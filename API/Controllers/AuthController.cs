using BLL.DTOs;
using BLL.DTOs.Auth;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace CRM_SaaS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService service;

        public AuthController(AuthService service)
        {
            this.service = service;
        }


        // Login
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO login)
        {
            try
            {
                string msg;

                var tokenResponse = service.Login(login, out msg);

                return Ok(new {Success = true, Message = "Login successful.", Data = tokenResponse});
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new {Success = false, Message = ex.Message});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {Success = false, Message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new {Success = false, Message = ex.Message});
            }
        }

        // ─────────────────────────────────────────────────────────
        // Refresh Token
        // ─────────────────────────────────────────────────────────

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public IActionResult RefreshToken(RefreshTokenDTO request)
        {
            try
            {
                string msg;

                var tokenResponse = service.RefreshToken(request.RefreshToken, out msg);
                return Ok(new {Success = true, Message = "Token refreshed successfully.", Data = tokenResponse});
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new {Success = false, Message = ex.Message});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {Success = false, Message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new{Success = false, Message = ex.Message});
            }
        }

        // Register Tenant
        /*
        [HttpPost("register-tenant")]
        [AllowAnonymous]
        public IActionResult RegisterTenant(RegisterTenantDTO model)
        {
            try
            {
                string msg;

                var tokenResponse = service.RegisterTenant(model, out msg);

                return Ok(new
                {
                    Success = true,
                    Message = "Tenant registered successfully.",
                    Data = tokenResponse
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
        */

        // ─────────────────────────────────────────────────────────
        // Logout
        // ─────────────────────────────────────────────────────────

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Invalid token."
                    });
                }

                bool parsed = Guid.TryParse(userIdClaim, out Guid userId);

                if (!parsed)
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Invalid user ID."
                    });
                }

                bool response = service.Logout(userId);

                if (!response)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Logout failed."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Logout successful."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // ─────────────────────────────────────────────────────────
        // Current User
        // ─────────────────────────────────────────────────────────

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");

                if (string.IsNullOrWhiteSpace(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Invalid token."
                    });
                }

                bool parsed = Guid.TryParse(userIdClaim, out Guid userId);

                if (!parsed)
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Invalid user ID."
                    });
                }

                var user = service.GetCurrentUser(userId);

                if (user == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}