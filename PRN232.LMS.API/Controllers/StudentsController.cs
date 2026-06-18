using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/students")]
[Produces("application/json", "application/xml")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service) => _service = service;

    /// <summary>Get all students with optional search, sort, paging, fields, and expand</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<StudentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(result);
    }

    /// <summary>Get student by ID with optional expand (enrollment)</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] string? expand = null)
    {
        var result = await _service.GetByIdAsync(id, expand);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Create a new student (Admin Only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] StudentCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<StudentResponse>.Fail("Validation failed.", ModelState));

        var result = await _service.CreateAsync(request);
        if (!result.Success) return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.StudentId, version = "1.0" }, result);
    }

    /// <summary>Update student by ID (Admin Only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] StudentUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<StudentResponse>.Fail("Validation failed.", ModelState));

        var result = await _service.UpdateAsync(id, request);
        if (!result.Success)
            return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);

        return Ok(result);
    }

    /// <summary>Delete student by ID (Admin Only)</summary>
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
