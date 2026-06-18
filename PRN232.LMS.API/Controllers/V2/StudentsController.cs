using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.API.Controllers.V2;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/students")]
[Produces("application/json", "application/xml")]
[Authorize]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _service;

    public StudentsController(IStudentService service)
    {
        _service = service;
    }

    /// <summary>Get all students - API Version 2.0 (Includes V2 signature header and consistent wrapper)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<StudentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParameters query)
    {
        var result = await _service.GetAllAsync(query);
        
        // Append a custom header to indicate V2 processing
        Response.Headers.Append("X-API-Version", "2.0-Beta");
        
        // We return the same paged result, but with a modified message indicating V2 execution
        var v2Result = PagedResponse<StudentResponse>.Ok(
            result.Data ?? new List<StudentResponse>(), 
            result.Pagination ?? new PaginationMeta(), 
            "Request processed successfully via LMS API V2.0");

        return Ok(v2Result);
    }

    /// <summary>Get student by ID - API Version 2.0</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] int id, [FromQuery] string? expand = null)
    {
        var result = await _service.GetByIdAsync(id, expand);
        if (!result.Success)
        {
            return NotFound(result);
        }

        var v2Result = ApiResponse<StudentResponse>.Ok(result.Data!, "Student retrieved successfully via LMS API V2.0");
        return Ok(v2Result);
    }
}
