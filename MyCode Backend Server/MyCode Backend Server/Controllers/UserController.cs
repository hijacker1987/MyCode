using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DataContext _dataContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authenticationService;

        public UserController(ILogger<UserController> logger, DataContext dataContext, UserManager<IdentityUser> userManager, IConfiguration configuration, IAuthService authenticationService)
        {
            _logger = logger;
            _dataContext = dataContext;
            _userManager = userManager;
            _configuration = configuration;
            _authenticationService = authenticationService;
        }

        [HttpGet("/getUsers")]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            try
            {
                var returningList = new List<IdentityUser>();
                var users = _dataContext.Users.ToList();

                if (users == null || !users.Any())
                {
                    _logger.LogInformation("There is no user in the database.");
                    return Ok(users);
                }

                foreach (var user in users)
                {
                    if (user != null)
                    {
                        returningList.Add(user);
                    }
                }

                return Ok(returningList);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return NotFound();
            }
        }

        [HttpPost("/register")]
        public async Task<ActionResult<UserRegResponse>> Register(UserRegRequest request)
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

                _dataContext!.Users!.Add(newUser);
                await _dataContext.SaveChangesAsync();

                return CreatedAtAction(nameof(Register), new UserRegResponse(result.Email, result.UserName));
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        [HttpPost("/login")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationService.LoginAsync(request.Email, request.Password);

            if (!result.Success)
            {
                AddErrors(result);
                return BadRequest(ModelState);
            }

            var roleId = _dataContext!.UserRoles.First(r => r.UserId == result.Id).RoleId;
            var role = _dataContext.Roles.First(r => r.Id == roleId).Name;

            Response.Cookies.Append("Authentication", result.Token);
            return Ok(new AuthResponse(result.Email, result.UserName, result.Token, role!));
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
