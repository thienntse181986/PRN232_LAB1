using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.Services.Implementations;

public class SemesterService : ISemesterService
{
    private readonly IUnitOfWork _unitOfWork;

    public SemesterService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<PagedResponse<SemesterResponse>> GetAllAsync(QueryParameters query)
    {
        var q = _unitOfWork.Semesters.GetQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.ToLower();
            q = q.Where(x => x.SemesterName.ToLower().Contains(s));
        }

        bool expandCourses = query.Expand?.Contains("course", StringComparison.OrdinalIgnoreCase) == true;
        if (expandCourses) q = q.Include(x => x.Courses);

        q = QueryHelper.ApplySorting(q, query.Sort);
        var (items, total) = await QueryHelper.ApplyPagingAsync(q, query.Page, query.Size);

        return PagedResponse<SemesterResponse>.Ok(items.Select(x => MapToResponse(x, expandCourses)), new PaginationMeta
        {
            Page = query.Page, PageSize = query.Size, TotalItems = total
        });
    }

    public async Task<ApiResponse<SemesterResponse>> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Semesters.GetByIdWithCoursesAsync(id);
        if (entity is null) return ApiResponse<SemesterResponse>.Fail($"Semester with ID {id} not found.");
        return ApiResponse<SemesterResponse>.Ok(MapToResponse(entity, true));
    }

    public async Task<ApiResponse<SemesterResponse>> CreateAsync(SemesterCreateRequest request)
    {
        if (request.EndDate <= request.StartDate)
            return ApiResponse<SemesterResponse>.Fail("EndDate must be after StartDate.");

        var entity = new Semester { SemesterName = request.SemesterName, StartDate = request.StartDate, EndDate = request.EndDate };
        await _unitOfWork.Semesters.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<SemesterResponse>.Ok(MapToResponse(entity, false), "Semester created successfully.");
    }

    public async Task<ApiResponse<SemesterResponse>> UpdateAsync(int id, SemesterUpdateRequest request)
    {
        var entity = await _unitOfWork.Semesters.GetByIdAsync(id);
        if (entity is null) return ApiResponse<SemesterResponse>.Fail($"Semester with ID {id} not found.");
        if (request.EndDate <= request.StartDate)
            return ApiResponse<SemesterResponse>.Fail("EndDate must be after StartDate.");

        entity.SemesterName = request.SemesterName;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        _unitOfWork.Semesters.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<SemesterResponse>.Ok(MapToResponse(entity, false), "Semester updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Semesters.GetByIdAsync(id);
        if (entity is null) return ApiResponse<bool>.Fail($"Semester with ID {id} not found.");
        
        _unitOfWork.Semesters.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.Ok(true, "Semester deleted successfully.");
    }

    private static SemesterResponse MapToResponse(Semester s, bool includeCourses) => new()
    {
        SemesterId = s.SemesterId,
        SemesterName = s.SemesterName,
        StartDate = s.StartDate,
        EndDate = s.EndDate,
        Courses = includeCourses && s.Courses?.Any() == true
            ? s.Courses.Select(c => new CourseResponse { CourseId = c.CourseId, CourseName = c.CourseName, SemesterId = c.SemesterId })
            : null
    };
}
