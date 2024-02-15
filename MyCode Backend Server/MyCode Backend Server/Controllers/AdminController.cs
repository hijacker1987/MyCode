using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/admin")]
    public class AdminController(ITokenService tokenService, ILogger<AdminController> logger, DataContext dataContext) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<AdminController> _logger = logger;
        private readonly DataContext _dataContext = dataContext;

        [HttpGet("getUsers"), Authorize(Roles = "Admin")]
        public ActionResult<List<User>> GetAllUsers()
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];

                if (_tokenService.ValidateToken(authorizationCookie!))
                {
                    var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                    if (checkedToken == null)
                    {
                        _logger.LogError("Token expired.");
                        return BadRequest("Token expired.");
                    }
                }

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

        [HttpGet("user-by-{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserRegResponse>> GetUserById([FromRoute] Guid id)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];

                if (_tokenService.ValidateToken(authorizationCookie!))
                {
                    var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                    if (checkedToken == null)
                    {
                        _logger.LogError("Token expired.");
                        return BadRequest("Token expired.");
                    }
                }

                var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

                if (user == null)
                {
                    return NotFound(new { ErrorMessage = $"User with ID {id} not found." });
                }

                var response = new UserRegResponse(id.ToString(), user.Email!, user.UserName!, user.DisplayName!, user.PhoneNumber!);

                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return StatusCode(500, new { ErrorMessage = "Error occurred while fetching user by ID!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpGet("getCodes"), Authorize(Roles = "Admin")]
        public ActionResult<List<Code>> GetAllCodes()
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];

                if (_tokenService.ValidateToken(authorizationCookie!))
                {
                    var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                    if (checkedToken == null)
                    {
                        _logger.LogError("Token expired.");
                        return BadRequest("Token expired.");
                    }
                }

                var codes = _dataContext.CodesDb!.ToList();
                var returningList = codes.Where(code => code != null).ToList();

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

        [HttpPut("au-{id}"), Authorize(Roles = "Admin")]
        public ActionResult<User> UpdateUser([FromRoute] Guid id, [FromBody] User updatedUser)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];

                if (_tokenService.ValidateToken(authorizationCookie!))
                {
                    var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                    if (checkedToken == null)
                    {
                        _logger.LogError("Token expired.");
                        return BadRequest("Token expired.");
                    }
                }

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

        [HttpPut("acu-{id}"), Authorize(Roles = "Admin")]
        public ActionResult<Code> UpdateCode([FromRoute] Guid id, [FromBody] Code updatedCode)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];

                if (_tokenService.ValidateToken(authorizationCookie!))
                {
                    var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                    if (checkedToken == null)
                    {
                        _logger.LogError("Token expired.");
                        return BadRequest("Token expired.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingCode = _dataContext.CodesDb!.FirstOrDefault(c => c.Id == id);

                if (existingCode == null)
                {
                    _logger.LogInformation($"Code with id {id} not found.");
                    return NotFound();
                }

                existingCode.CodeTitle = updatedCode.CodeTitle;
                existingCode.MyCode = updatedCode.MyCode;
                existingCode.WhatKindOfCode = updatedCode.WhatKindOfCode;
                existingCode.IsBackend = updatedCode.IsBackend;
                existingCode.IsVisible = updatedCode.IsVisible;

                _dataContext.SaveChanges();

                return Ok(existingCode);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest();
            }
        }

        [HttpDelete("adu-{id}"), Authorize(Roles = "Admin")]
        public ActionResult DeleteUser([FromRoute] Guid id)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];

                if (_tokenService.ValidateToken(authorizationCookie!))
                {
                    var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                    if (checkedToken == null)
                    {
                        _logger.LogError("Token expired.");
                        return BadRequest("Token expired.");
                    }
                }

                _logger.LogInformation($"Deleting user with id: {id}");

                var user = _dataContext.Users!.FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogError($"User with id {id} not found.");
                    return NotFound();
                }

                _dataContext.Users!.Remove(user);
                _dataContext.SaveChanges();

                _logger.LogInformation($"User with id {id} successfully deleted.");
                return Ok($"User with id {id} successfully deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during user deletion: {e.Message}");
                _logger.LogError(e.StackTrace);
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpDelete("adc-{id}"), Authorize(Roles = "Admin")]
        public ActionResult DeleteCode([FromRoute] Guid id)
        {
            try
            {
                var authorizationCookie = Request.Cookies["Authorization"];

                if (_tokenService.ValidateToken(authorizationCookie!))
                {
                    var checkedToken = _tokenService.Refresh(authorizationCookie!, Request, Response);

                    if (checkedToken == null)
                    {
                        _logger.LogError("Token expired.");
                        return BadRequest("Token expired.");
                    }
                }

                var code = _dataContext.CodesDb!.FirstOrDefault(c => c.Id == id);

                if (code == null)
                {
                    _logger.LogInformation($"Code with id {id} not found.");
                    return NotFound();
                }

                _dataContext.CodesDb!.Remove(code);
                _dataContext.SaveChanges();

                return Ok($"Code with id {id} successfully deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest();
            }
        }
    }
}
