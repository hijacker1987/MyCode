using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;
using System.Security.Claims;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UserController(
        ILogger<UserController> logger,
        IConfiguration configuration,
        IAuthService authenticationService,
        ITokenService tokenService,
        DataContext dataContext,
        UserManager<User> userManager) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IAuthService _authenticationService = authenticationService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly DataContext _dataContext = dataContext;
        private readonly UserManager<User> _userManager = userManager;

        [HttpGet("getUser"), Authorize(Roles = "Admin, User")]
        public ActionResult<UserRegResponse> GetUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    _logger.LogError("No 'NameIdentifier' claim found in the ClaimsPrincipal.");
                    return BadRequest("No 'NameIdentifier' claim found.");
                }

                var userId = userIdClaim.Value;

                if (!Guid.TryParse(userId, out var userIdGuid))
                {
                    _logger.LogError($"Unable to parse as a Guid.");
                    return BadRequest($"Unable to parse 'NameIdentifier' claim value as a Guid.");
                }

                var user = _dataContext.Users.FirstOrDefault(u => u.Id == userIdGuid);

                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return StatusCode(500, new { ErrorMessage = "Error occurred while fetching user by ID!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserRegResponse>> RegisterUserAsync(UserRegRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            _logger.LogError($"ModelState Error: {error.ErrorMessage}");
                        }
                    }
                    return BadRequest(ModelState);
                }

                var result = await _authenticationService.RegisterAsync(request.Email,
                                                                        request.Username,
                                                                        request.Password,
                                                                        request.DisplayName,
                                                                        request.PhoneNumber);

                if (!result.Success)
                {
                    AddErrors(result);
                    return BadRequest(ModelState);
                }

                return Ok(new UserRegResponse(result.Id!, result.Email!, result.UserName!, result.DisplayName!, result.PhoneNumber!));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest(new { ErrorMessage = "Cannot register!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> LoginAsync([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.LoginAsync(request.Email, request.Password);

            if (!result.Success)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }

            var managedUser = await _userManager.FindByEmailAsync(request.Email);

            if (managedUser == null)
            {
                return BadRequest("User not found.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);

            if (!isPasswordValid)
            {
                return BadRequest("PW not valid.");
            }

            var roles = await _userManager.GetRolesAsync(managedUser);

            await _userManager.AddToRolesAsync(managedUser, roles);

            managedUser.LastTimeLogin = DateTime.UtcNow.AddHours(1);

            var refreshToken = _tokenService.GenerateRefreshToken();

            managedUser.RefreshToken = refreshToken;
            managedUser.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(30);

            await _userManager.UpdateAsync(managedUser);
            await _dataContext.SaveChangesAsync();

            var accessToken = _tokenService.CreateToken(managedUser, roles);

            return new AuthResponse(result.Email!, result.UserName!, accessToken, refreshToken);
        }

        [HttpPut("u-{id}"), Authorize(Roles = "User")]
        public ActionResult<User> UpdateUser([FromRoute] Guid id, [FromBody] User updatedUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = _dataContext.Users.FirstOrDefault(u => u.Id == id);

                if (existingUser == null)
                {
                    _logger.LogInformation($"User with id {id} not found.");
                    return NotFound();
                }

                existingUser.DisplayName = updatedUser.DisplayName;
                existingUser.UserName = updatedUser.UserName;
                existingUser.NormalizedUserName = updatedUser.UserName!.ToUpper();
                existingUser.Email = updatedUser.Email;
                existingUser.NormalizedEmail = updatedUser.Email!.ToUpper();
                existingUser.PhoneNumber = updatedUser.PhoneNumber;

                _dataContext.SaveChanges();

                return Ok(existingUser);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest();
            }
        }

        [HttpPatch("changePassword"), Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<ChangePassResponse>> ChangePasswordAsync([FromBody] ChangePassRequest request)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);

                if (existingUser == null)
                {
                    return BadRequest(existingUser);
                }

                var result = await _userManager.ChangePasswordAsync(existingUser, request.CurrentPassword, request.NewPassword);

                if (result.Succeeded)
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Message = $"Successful password change on {request.Email}" });
                }
                else
                {
                    return BadRequest(new { ErrorMessage = "Unable to change password.", result.Errors });
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return NotFound("Error occurred!");
            }
        }

        [HttpDelete("delete-{id}"), Authorize(Roles = "User")]
        public async Task<ActionResult> DeleteAccountAsync([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound("Not Found!");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    return BadRequest("User not found!");
                }

                if (_configuration["AEmail"]!.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase))
                {
                    return Unauthorized();
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Ok($"Account with ID {id} successfully deleted.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Concurrency error during account deletion: {ex.Message}", ex);
                return Ok($"Account with ID {id} successfully deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest(e.Message);
            }
        }

        private void AddErrors(AuthResult result)
        {
            foreach (var resultErrorMessage in result.ErrorMessages)
            {
                ModelState.AddModelError(resultErrorMessage.Key, resultErrorMessage.Value);
            }
        }
    }
}
