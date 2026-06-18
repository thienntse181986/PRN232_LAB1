using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/semesters")]
[Produces("application/json", "application/xml")]
[Authorize]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _service;

    public SemestersController(ISemesterService service) => _service = service;

    /// <summary>Get all semesters with optional search, sort, paging, and expand (courses)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<SemesterResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        => Ok(await _service.GetAllAsync(query));

    /// <summary>Get semester by ID with optional expand (courses)</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] string? expand = null)
    {
        var result = await _service.GetByIdAsync(id, expand);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Create a new semester (Admin Only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] SemesterCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<SemesterResponse>.Fail("Validation failed.", ModelState));
        var result = await _service.CreateAsync(request);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.SemesterId, version = "1.0" }, result);
    }

    /// <summary>Update semester by ID (Admin Only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SemesterUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<SemesterResponse>.Fail("Validation failed.", ModelState));
        var result = await _service.UpdateAsync(id, request);
        if (!result.Success)
            return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return Ok(result);
    }

    /// <summary>Delete semester by ID (Admin Only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}
