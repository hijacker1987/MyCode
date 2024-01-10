using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Contracts.Services;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authenticationService;
        private readonly DataContext _dataContext;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(
            ILogger<UserController> logger,
            IConfiguration configuration,
            IAuthService authenticationService,
            DataContext dataContext,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _configuration = configuration;
            _authenticationService = authenticationService;
            _dataContext = dataContext;
            _userManager = userManager;
        }

        [HttpGet("/getUsers"), Authorize(Roles = "Admin")]
        public ActionResult<List<IdentityUser>> GetAllUsers()
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

                var result = await _authenticationService.RegisterAsync(request.Email, request.Username, request.Password, request.PhoneNumber, "User");

                if (!result.Success)
                {
                    AddErrors(result);
                    return BadRequest(ModelState);
                }

                var newUser = new User();

                _dataContext.Users.Add(newUser);
                await _dataContext.SaveChangesAsync();

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
                return BadRequest(result);
            }

            var roleId = _dataContext.UserRoles.First(r => r.UserId == result.Id).RoleId;
            var role = _dataContext.Roles.First(r => r.Id == roleId).Name;

            Response.Cookies.Append("Authentication", result.Token);
            return Ok(new AuthResponse(result.Email, result.UserName, result.Token, role!));
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
                return NotFound("Error occured!");
            }
        }


        [HttpDelete("/deleteAccount"), Authorize(Roles = "User")]
        public async Task<ActionResult> DeleteAccountAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound("Not Found!");
            }

            if (_configuration["AEmail"]!.ToLower() == email!.ToLower())
            {
                return Unauthorized();
            }

            try
            {
                var identityUser = await _userManager.FindByEmailAsync(email);
                var dbUser = _dataContext.Users!.FirstOrDefault(e => e.Id == identityUser!.Id);

                _dataContext.Users.Remove(dbUser!);
                await _dataContext.SaveChangesAsync();

                if (identityUser == null)
                {
                    return BadRequest("Something went wrong!");
                }

                var result = await _userManager.DeleteAsync(identityUser);

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
