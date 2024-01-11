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

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
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

        [HttpGet("/getUsers"), Authorize(Roles = "Admin")]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                var users = _dataContext.Users.ToList();
                var returningList = users.Where(user => user != null).ToList();

                if (returningList.Count == 0)
                {
                    _logger.LogInformation("There are no users in the database.");
                    return Ok(returningList);
                }

                return Ok(returningList);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return NotFound();
            }
        }

        [HttpPost("/registerUser")]
        public async Task<ActionResult<UserRegResponse>> RegisterUserAsync(UserRegRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authenticationService.RegisterAsync(request.Email,
                                                                        request.Username,
                                                                        request.Password,
                                                                        request.DisplayName,
                                                                        request.PhoneNumber,
                                                                        "User"
                                                                        );

                if (!result.Success)
                {
                    AddErrors(result);
                    return BadRequest(ModelState);
                }

                return Ok(new UserRegResponse(result.Email, result.UserName));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest(new { ErrorMessage = "Cannot register!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpPost("/login")]
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

            var accessToken = _tokenService.CreateToken(managedUser, roles.First());

            return new AuthResponse(result.Email, result.UserName, accessToken, roles.First());
        }

        [HttpPatch("/changePassword"), Authorize(Roles = "Admin, User")]
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
                    return Ok($"Successful password change on {request.Email}");
                }
                else
                {
                    return BadRequest($"Unable to change password on {request.Email}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return NotFound("Error occurred!");
            }
        }

        [HttpDelete("/deleteAccount"), Authorize(Roles = "User")]
        public async Task<ActionResult> DeleteAccountAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound("Not Found!");
            }

            if (_configuration["AEmail"]!.Equals(email, StringComparison.CurrentCultureIgnoreCase))
            {
                return Unauthorized();
            }

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                var dbUser = _dataContext.Users!.FirstOrDefault(e => e.Id.Equals(user!.Id));

                _dataContext.Users.Remove(dbUser!);
                await _dataContext.SaveChangesAsync();

                if (user == null)
                {
                    return BadRequest("Something went wrong!");
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Ok($"Account with email {email} successfully deleted.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Concurrency error during account deletion: {ex.Message}", ex);
                return Ok($"Account with email {email} successfully deleted.");
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
