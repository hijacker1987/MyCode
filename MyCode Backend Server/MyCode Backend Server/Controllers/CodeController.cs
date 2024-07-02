using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using MyCode_Backend_Server.Service.Authentication.Token;
using System.Security.Claims;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("/codes")]
    public class CodeController(ITokenService tokenService, ILogger<CodeController> logger, DataContext dataContext, UserManager<User> userManager) : ControllerBase
    {
        private readonly ITokenService _tokenService = tokenService;
        private readonly ILogger<CodeController> _logger = logger;
        private readonly DataContext _dataContext = dataContext;
        private readonly UserManager<User> _userManager = userManager;

        [HttpGet("by-user"), Authorize(Roles = "Admin, Support, User")]
        public ActionResult<List<Code>> GetAllCodesByUser()
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

                var codes = _dataContext.CodesDb!
                                        .Where(c => c.UserId.ToString() == userId)
                                        .ToList();

                return Ok(codes);
            }
            catch (Exception e)
            {
                _logger.LogError("Error: {e}", e.Message);
                return BadRequest();
            }
        }

        [HttpGet("by-visibility"), Authorize(Roles = "Admin, Support, User")]
        public ActionResult<List<Code>> GetAllCodesByVisibility()
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

                var codes = _dataContext.CodesDb!
                                        .Where(c => c.IsVisible && c.UserId.ToString() != userId)
                                        .Select(c => new CodeWithAdditionalData
                                        {
                                            Id = c.Id,
                                            CodeTitle = c.CodeTitle,
                                            MyCode = c.MyCode,
                                            WhatKindOfCode = c.WhatKindOfCode,
                                            IsBackend = c.IsBackend,
                                            IsVisible = c.IsVisible,
                                            DisplayName = _dataContext.Users.FirstOrDefault(u => u.Id == c.UserId)!.DisplayName
                                        })
                                        .ToList();

                return Ok(codes);
            }
            catch (Exception e)
            {
                _logger.LogError("Error: {e}", e.Message);
                return BadRequest();
            }
        }

        [HttpGet("code-{id}"), Authorize(Roles = "Admin, Support, User")]
        public ActionResult<Code> GetCodeById([FromRoute] Guid id)
        {
            try
            {
                var tokenValidationResult = TokenAndCookieHelper.ValidateAndRefreshToken(_tokenService, Request, Response, _logger);
                if (tokenValidationResult != null) return tokenValidationResult;

                var code = _dataContext.CodesDb!.FirstOrDefault(c => c.Id == id);

                if (code == null)
                {
                    _logger.LogInformation("Code with id {id} not found.", id);
                    return NotFound();
                }

                return Ok(code);
            }
            catch (Exception e)
            {
                _logger.LogError("Error: {e}", e.Message);
                return NotFound();
            }
        }

        [HttpPost("register"), Authorize(Roles = "Admin, Support, User")]
        public ActionResult<CodeRegResponse> CreateCode([FromBody] CodeRegRequest codeRequest)
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

                var user = _dataContext.Users!.FirstOrDefault(u => u.Id.ToString() == userId);

                if (user == null)
                {
                    _logger.LogInformation("User with id {id} not found.", userId);
                    return NotFound();
                }

                if (user.EmailConfirmed != true && user.TwoFactorEnabled)
                {
                    _logger.LogInformation("User with id {id} not verified.", userId);
                    return BadRequest();
                }

                var code = new Code(
                    codeRequest.CodeTitle,
                    codeRequest.MyCode,
                    codeRequest.WhatKindOfCode,
                    codeRequest.IsBackend,
                    codeRequest.IsVisible)
                    {
                        UserId = new Guid(userId)
                    };

                _dataContext.CodesDb!.Add(code);
                _dataContext.SaveChanges();

                return CreatedAtAction(nameof(GetCodeById), new { id = code.Id }, new
                {
                    code.Id,
                    code.CodeTitle,
                    code.MyCode,
                    code.WhatKindOfCode,
                    code.IsBackend,
                    code.IsVisible
                });
            }
            catch (Exception e)
            {
                _logger.LogError("Error: {e}", e.Message);
                return BadRequest();
            }
        }

        [HttpPut("cupdate-{id}"), Authorize(Roles = "Admin, Support, User")]
        public ActionResult<CodeRegResponse> UpdateCode([FromRoute] Guid id, [FromBody] CodeRegRequest updatedCode)
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

                var user = _dataContext.Users!.FirstOrDefault(u => u.Id.ToString() == userId);

                if (user == null)
                {
                    _logger.LogInformation("User with id {id} not found.", userId);
                    return NotFound();
                }

                if (user.EmailConfirmed != true && user.TwoFactorEnabled)
                {
                    _logger.LogInformation("User with id {id} not verified.", userId);
                    return BadRequest();
                }

                var existingCode = _dataContext.CodesDb!.FirstOrDefault(c => c.Id == id && c.UserId.ToString() == userId);

                if (existingCode == null)
                {
                    _logger.LogInformation("Code with id {id} not found or does not belong to the authenticated user.", id);
                    return NotFound();
                }

                if (updatedCode == null)
                {
                    return BadRequest("Invalid code data");
                }

                existingCode.CodeTitle = updatedCode.CodeTitle;
                existingCode.MyCode = updatedCode.MyCode;
                existingCode.WhatKindOfCode = updatedCode.WhatKindOfCode;
                existingCode.IsBackend = updatedCode.IsBackend;
                existingCode.IsVisible = updatedCode.IsVisible;

                _dataContext.SaveChanges();

                var response = new
                {
                    existingCode.CodeTitle,
                    existingCode.MyCode,
                    existingCode.WhatKindOfCode,
                    existingCode.IsBackend,
                    existingCode.IsVisible
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError("Error: {e}", e.Message);
                return BadRequest();
            }
        }

        [HttpDelete("cdelete-{id}"), Authorize(Roles = "User, Support")]
        public ActionResult DeleteCodeByUser([FromRoute] Guid id)
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

                var code = _dataContext.CodesDb!.FirstOrDefault(c => c.Id == id && c.UserId.ToString() == userId);

                if (code == null)
                {
                    _logger.LogInformation("Code with id {id} not found for the authenticated user.", id);
                    return NotFound();
                }

                _dataContext.CodesDb!.Remove(code);
                _dataContext.SaveChanges();

                return Ok($"Code with id {id} successfully deleted.");
            }
            catch (Exception e)
            {
                _logger.LogError("Error: {e}", e.Message);
                return BadRequest();
            }
        }
    }
}
