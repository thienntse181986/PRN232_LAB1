using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses")]
[Produces("application/json", "application/xml")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service) => _service = service;

    /// <summary>Get all courses with optional search, sort, paging, and expand (semester, enrollment)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CourseResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        => Ok(await _service.GetAllAsync(query));

    /// <summary>Get course by ID with optional expand (semester, enrollment)</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] string? expand = null)
    {
        var result = await _service.GetByIdAsync(id, expand);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Get students enrolled in a specific course (Nested Resource)</summary>
    [HttpGet("{courseId:int}/students")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentResponse>>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentsByCourse([FromRoute] int courseId)
    {
        var result = await _service.GetStudentsByCourseIdAsync(courseId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Create a new course (Admin Only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CourseCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<CourseResponse>.Fail("Validation failed.", ModelState));
        var result = await _service.CreateAsync(request);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.CourseId, version = "1.0" }, result);
    }

    /// <summary>Update course by ID (Admin Only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<CourseResponse>.Fail("Validation failed.", ModelState));
        var result = await _service.UpdateAsync(id, request);
        if (!result.Success)
            return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return Ok(result);
    }

    /// <summary>Delete course by ID (Admin Only)</summary>
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
