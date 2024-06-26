﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Models.MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication;
using MyCode_Backend_Server.Service.Authentication.Token;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/admin")]
    public class AdminController(ITokenService tokenService, IAuthService authService, ILogger<AdminController> logger, DataContext dataContext) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AdminController> _logger = logger;
        private readonly DataContext _dataContext = dataContext;

        [HttpGet("getUsers"), Authorize(Roles = "Admin, Support")]
        public async Task<ActionResult<List<UserWithRole>>> GetAllUsersAsync()
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var users = _dataContext.Users.ToList();
                var usersList = users.Where(user => user != null).ToList();

                if (usersList.Count == 0)
                {
                    _logger.LogInformation("There are no users in the database.");
                    return Ok(usersList);
                }

                var returnList = new List<UserWithRole>();

                foreach ( var user in usersList )
                {
                    var result = new UserWithRole(user, await _authService.GetRoleStatusByUserAsync(user));
                    returnList.Add(result);
                }

                return Ok(returnList);
            }
            catch (Exception e)
            {
                _logger.LogError("Error");
                return NotFound(e);
            }
        }

        [HttpGet("user-by-{id}"), Authorize(Roles = "Admin, Support")]
        public async Task<ActionResult<UserRegResponse>> GetUserById([FromRoute] Guid id)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));

                if (user == null)
                {
                    return NotFound(new { ErrorMessage = $"User with ID {id} not found." });
                }

                var role = await _authService.GetRoleStatusByUserAsync(user);

                var response = new UserRegResponse(id.ToString(), user.Email!, user.UserName!, user.DisplayName!, user.PhoneNumber!, role);

                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError("Error");
                return StatusCode(500, new { ErrorMessage = "Error occurred while fetching user by ID!", ExceptionDetails = e.ToString() });
            }
        }

        [HttpGet("getCodes"), Authorize(Roles = "Admin, Support")]
        public ActionResult<List<Code>> GetAllCodes()
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

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
                _logger.LogError("Error");
                return NotFound(e);
            }
        }

        [HttpPut("aupdate-{id}"), Authorize(Roles = "Admin")]
        public ActionResult<User> UpdateUser([FromRoute] Guid id, [FromBody] User updatedUser)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = _dataContext.Users.FirstOrDefault(u => u.Id == id);

                if (existingUser == null)
                {
                    _logger.LogInformation("User not found.");
                    return NotFound();
                }

                existingUser.DisplayName = updatedUser.DisplayName;
                if (updatedUser.UserName != null && !updatedUser.UserName.Contains(' '))
                {
                    existingUser.UserName = updatedUser.UserName;
                }
                else
                {
                    return BadRequest();
                }
                existingUser.NormalizedUserName = updatedUser.UserName!.ToUpper();
                if (updatedUser.Email != null && updatedUser.Email.Contains('@'))
                {
                    existingUser.Email = updatedUser.Email;
                }
                else
                {
                    return BadRequest();
                }
                existingUser.NormalizedEmail = updatedUser.Email!.ToUpper();
                existingUser.PhoneNumber = updatedUser.PhoneNumber;

                _dataContext.SaveChanges();

                return Ok(existingUser);
            }
            catch (Exception e)
            {
                _logger.LogError("Error");
                return BadRequest(e);
            }
        }

        [HttpPut("acupdate-{id}"), Authorize(Roles = "Admin")]
        public ActionResult<Code> UpdateCode([FromRoute] Guid id, [FromBody] Code updatedCode)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingCode = _dataContext.CodesDb!.FirstOrDefault(c => c.Id == id);

                if (existingCode == null)
                {
                    _logger.LogInformation("Code not found.");
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
                _logger.LogError("Error");
                return BadRequest(e);
            }
        }

        [HttpPost("asupdate"), Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoleStatusResponse>> UpdateStatus([FromBody] RoleStatusRequest request)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _dataContext.Users!.FirstOrDefaultAsync(u => u.Email == request.Status);

                if (existingUser == null)
                {
                    _logger.LogInformation("User not found.");
                    return BadRequest(request);
                }

                if (existingUser.UserName == HttpContext.User.Identity?.Name)
                {
                    return NotFound();
                }

                var result = new RoleStatusResponse(await _authService.SetRoleStatusAsync(existingUser));

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError("Error");
                return BadRequest(e);
            }
        }

        [HttpDelete("aduser-{id}"), Authorize(Roles = "Admin")]
        public ActionResult DeleteUser([FromRoute] Guid id)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                _logger.LogInformation("Deleting user");

                var user = _dataContext.Users!.FirstOrDefault(u => u.Id == id);

                if (user == null)
                {
                    _logger.LogError("User not found.");
                    return NotFound();
                }

                _dataContext.Users!.Remove(user);
                _dataContext.SaveChanges();

                _logger.LogInformation("User successfully deleted.");
                return Ok($"User with id {id} successfully deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError("Error during user deletion");
                return StatusCode(500, $"Internal Server Error: {e.Message}");
            }
        }

        [HttpDelete("adcode-{id}"), Authorize(Roles = "Admin")]
        public ActionResult DeleteCode([FromRoute] Guid id)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var code = _dataContext.CodesDb!.FirstOrDefault(c => c.Id == id);

                if (code == null)
                {
                    _logger.LogInformation("Code not found.");
                    return NotFound();
                }

                _dataContext.CodesDb!.Remove(code);
                _dataContext.SaveChanges();

                return Ok($"Code with id {id} successfully deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError("Error");
                return BadRequest(e);
            }
        }
    }
}
