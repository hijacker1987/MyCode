using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCode_Backend_Server.Contracts.Registers;
using MyCode_Backend_Server.Data;
using MyCode_Backend_Server.Models;
using System.Security.Claims;

namespace MyCode_Backend_Server.Controllers
{
    [ApiController]
    [Route("api/codes")]
    public class CodeController(ILogger<CodeController> logger, DataContext dataContext) : ControllerBase
    {
        private readonly ILogger<CodeController> _logger = logger;
        private readonly DataContext _dataContext = dataContext;

        [HttpGet("by-user")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<List<Code>> GetAllCodesByUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("Unable to retrieve UserId from the ClaimsPrincipal.");
                    return BadRequest("Unable to retrieve UserId.");
                }

                var codes = _dataContext.CodesDb!
                                        .Where(c => c.UserId == Guid.Parse(userId))
                                        .ToList();

                return Ok(codes);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest();
            }
        }

        [HttpGet("by-visibility")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<List<Code>> GetAllCodesByVisibility()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("Unable to retrieve UserId from the ClaimsPrincipal.");
                    return BadRequest("Unable to retrieve UserId.");
                }

                var codes = _dataContext.CodesDb!
                                        .Where(c => c.UserId == Guid.Parse(userId) || c.IsVisible)
                                        .ToList();

                return Ok(codes);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest();
            }
        }

        [HttpGet("by-id:{id}")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<Code> GetCodeById(Guid id)
        {
            try
            {
                var code = _dataContext.CodesDb!.FirstOrDefault(c => c.Id == id);

                if (code == null)
                {
                    _logger.LogInformation($"Code with id {id} not found.");
                    return NotFound();
                }

                return Ok(code);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error: {e.Message}", e);
                return NotFound();
            }
        }

        [HttpPost("codeRegister")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<CodeRegResponse> CreateCode([FromBody] CodeRegRequest codeRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("Unable to retrieve UserId from the ClaimsPrincipal.");
                    return BadRequest("Unable to retrieve UserId.");
                }
                var userIdGuid = Guid.Parse(userId);

                var code = new Code(
                    codeRequest.CodeTitle,
                    codeRequest.MyCode,
                    codeRequest.WhatKindOfCode,
                    codeRequest.IsBackend,
                    codeRequest.IsVisible)
                    {
                        UserId = userIdGuid
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
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, User")]
        public ActionResult<CodeRegResponse> UpdateCode(Guid id, [FromBody] CodeRegRequest updatedCode)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
                {
                    _logger.LogError("Unable to retrieve UserId from the ClaimsPrincipal.");
                    return BadRequest("Unable to retrieve UserId.");
                }

                var existingCode = _dataContext.CodesDb!
                    .FirstOrDefault(c => c.Id == id && c.UserId == Guid.Parse(userIdClaim.Value));

                if (existingCode == null)
                {
                    _logger.LogInformation($"Code with id {id} not found or does not belong to the authenticated user.");
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
                _logger.LogError($"Error: {e.Message}", e);
                return BadRequest();
            }
        }

        [HttpPut("U-{id}")]
        [Authorize(Roles = "User")]
        public ActionResult<Code> UpdateCodeByUser(Guid id, [FromBody] Code updatedCode)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("Unable to retrieve UserId from the ClaimsPrincipal.");
                    return BadRequest("Unable to retrieve UserId.");
                }

                var existingCode = _dataContext.CodesDb!
                    .FirstOrDefault(c => c.Id == id && c.UserId == Guid.Parse(userId));

                if (existingCode == null)
                {
                    _logger.LogInformation($"Code with id {id} not found or does not belong to the authenticated user.");
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

        [HttpDelete("U-{id}")]
        [Authorize(Roles = "User")]
        public ActionResult DeleteCodeByUser(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("Unable to retrieve UserId from the ClaimsPrincipal.");
                    return BadRequest("Unable to retrieve UserId.");
                }

                var code = _dataContext.CodesDb!
                                       .FirstOrDefault(c => c.Id == id && c.UserId == Guid.Parse(userId));

                if (code == null)
                {
                    _logger.LogInformation($"Code with id {id} not found for the authenticated user.");
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
