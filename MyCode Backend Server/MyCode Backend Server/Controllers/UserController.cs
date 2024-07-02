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
using MyCode_Backend_Server.Service.Email_Sender;
using System.Security.Claims;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UserController(
        IConfiguration configuration,
        ILogger<UserController> logger,
        IAuthService authenticationService,
        ITokenService tokenService,
        IEmailSender emailSender,
        DataContext dataContext,
        UserManager<User> userManager,
        IWebHostEnvironment environment) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<UserController> _logger = logger;
        private readonly IAuthService _authenticationService = authenticationService;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly DataContext _dataContext = dataContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IWebHostEnvironment _environment = environment;

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
                            _logger.LogError($"ModelState Error");
                        }
                    }
                    return BadRequest(ModelState);
                }

                var result = await _authenticationService.RegisterAccAsync(request.Email,
                                                                           request.Username,
                                                                           request.Password,
                                                                           request.DisplayName,
                                                                           request.PhoneNumber);

                if (!result.Success)
                {
                    AddErrors(result);
                    return BadRequest(ModelState);
                }

                if (_environment.IsDevelopment())
                {
                    var subject = "Welcome to My Code!!!";

                    var message = $"{request.DisplayName} Thank You very much to use my application, ENJOY IT!";

                    await _emailSender.SendEmailAsync(request.Email, subject, message);
                }

                return Ok(new UserRegResponse(result.Id!, result.Email!, result.UserName!, result.DisplayName!, result.PhoneNumber!, ""));
            }
            catch (Exception e)
            {
                _logger.LogError("Cannot register!");
                return BadRequest(new { ErrorMessage = "Cannot register!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.LoginAccAsync(request.Email, request.Password, request.ConfirmPassword, Request, Response);

            if (!result.Success)
            {
                AddErrors(result);
                return NotFound(ModelState);
            }

            var managedUser = await _userManager.FindByEmailAsync(result.Email!);

            if (managedUser == null)
            {
                return NotFound("User not found.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);

            if (!isPasswordValid)
            {
                return Unauthorized("PW not valid.");
            }

            await _authenticationService.ApprovedAccLogin(managedUser, Request, Response);

            return Ok("Successfully logged in");
        }

        [HttpGet("getUser"), Authorize(Roles = "Admin, Support, User")]
        public ActionResult<UserRegResponse> GetUser()
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    _logger.LogError("No 'NameIdentifier' claim found in the ClaimsPrincipal.");
                    return BadRequest("No 'NameIdentifier' claim found.");
                }

                var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);

                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred while fetching user by ID!");
                return StatusCode(500, new { ErrorMessage = "Error occurred while fetching user by ID!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpGet("getUserId"), Authorize(Roles = "Admin, Support, User")]
        public ActionResult<UserRegResponse> GetUserId()
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    _logger.LogError("No 'NameIdentifier' claim found in the ClaimsPrincipal.");
                    return BadRequest("No 'NameIdentifier' claim found.");
                }

                var user = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);

                return Ok(user!.Id);
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred while fetching user by ID!");
                return StatusCode(500, new { ErrorMessage = "Error occurred while fetching user by ID!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpPut("userUpdate"), Authorize(Roles = "User, Support")]
        public ActionResult<User> UpdateUser([FromBody] User updatedUser)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    _logger.LogError("No 'NameIdentifier' claim found in the ClaimsPrincipal.");
                    return BadRequest("No 'NameIdentifier' claim found.");
                }

                var existingUser = _dataContext.Users.FirstOrDefault(u => u.Id.ToString() == userId);

                if (existingUser == null)
                {
                    _logger.LogInformation("User not found.");
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
                _logger.LogError($"Error");
                return BadRequest(e);
            }
        }

        [HttpPatch("changePassword"), Authorize(Roles = "Admin, Support, User")]
        public async Task<ActionResult<ChangePassResponse>> ChangePasswordAsync([FromBody] ChangePassRequest request)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var existingUser = await _userManager.FindByEmailAsync(request.Email);

                if (existingUser == null)
                {
                    return NotFound($"User with email {request.Email} not found.");
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
                _logger.LogError("Error occurred while changing password!");
                return StatusCode(500, new { ErrorMessage = "Error occurred while changing password!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpDelete("userDelete"), Authorize(Roles = "User, Support")]
        public async Task<ActionResult> DeleteAccountAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                _logger.LogError("No 'NameIdentifier' claim found in the ClaimsPrincipal.");
                return BadRequest("No 'NameIdentifier' claim found.");
            }

            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return BadRequest("User not found!");
                }

                if (_environment.IsDevelopment())
                {
                    if (_configuration!["AEmail"]!.Equals(user.Email, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return Unauthorized();
                    }
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    Response.Cookies.Delete("Authorization");
                    Response.Cookies.Delete("RefreshAuthorization");
                    Response.Cookies.Delete("UI");
                    Response.Cookies.Delete("UR");
                    Response.Cookies.Delete("UD");

                    await _dataContext.SaveChangesAsync();

                    return Ok($"Account successfully deleted.");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Concurrency error during account deletion");
                return Ok($"Account with ex {ex} successfully deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error");
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
