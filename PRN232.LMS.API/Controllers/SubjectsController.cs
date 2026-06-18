using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/subjects")]
[Produces("application/json", "application/xml")]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _service;

    public SubjectsController(ISubjectService service) => _service = service;

    /// <summary>Get all subjects with optional search, sort, paging</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<SubjectResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
        => Ok(await _service.GetAllAsync(query));

    /// <summary>Get subject by ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Create a new subject (Admin Only)</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] SubjectCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<SubjectResponse>.Fail("Validation failed.", ModelState));
        var result = await _service.CreateAsync(request);
        if (!result.Success) return BadRequest(result);
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.SubjectId, version = "1.0" }, result);
    }

    /// <summary>Update subject by ID (Admin Only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SubjectUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<SubjectResponse>.Fail("Validation failed.", ModelState));
        var result = await _service.UpdateAsync(id, request);
        if (!result.Success)
            return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        return Ok(result);
    }

    /// <summary>Delete subject by ID (Admin Only)</summary>
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
