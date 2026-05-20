using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<CourseResponse>> GetAllAsync(QueryParameters query)
    {
        var q = _unitOfWork.Courses.GetQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.ToLower();
            q = q.Where(x => x.CourseName.ToLower().Contains(s));
        }

        bool expandSemester = query.Expand?.Contains("semester", StringComparison.OrdinalIgnoreCase) == true;
        bool expandEnrollments = query.Expand?.Contains("enrollment", StringComparison.OrdinalIgnoreCase) == true;

        if (expandSemester) q = q.Include(x => x.Semester);
        if (expandEnrollments) q = q.Include(x => x.Enrollments).ThenInclude(e => e.Student);

        q = QueryHelper.ApplySorting(q, query.Sort);
        var (items, total) = await QueryHelper.ApplyPagingAsync(q, query.Page, query.Size);

        return PagedResponse<CourseResponse>.Ok(items.Select(x => MapToResponse(x, expandSemester, expandEnrollments)), new PaginationMeta
        {
            Page = query.Page, PageSize = query.Size, TotalItems = total
        });
    }

    public async Task<ApiResponse<CourseResponse>> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Courses.GetByIdWithDetailsAsync(id);
        if (entity is null) return ApiResponse<CourseResponse>.Fail($"Course with ID {id} not found.");
        return ApiResponse<CourseResponse>.Ok(MapToResponse(entity, true, true));
    }

    public async Task<ApiResponse<CourseResponse>> CreateAsync(CourseCreateRequest request)
    {
        var semester = await _unitOfWork.Semesters.GetByIdAsync(request.SemesterId);
        if (semester is null) return ApiResponse<CourseResponse>.Fail($"Semester with ID {request.SemesterId} not found.");

        var entity = new Course { CourseName = request.CourseName, SemesterId = request.SemesterId };
        await _unitOfWork.Courses.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        entity.Semester = semester;
        return ApiResponse<CourseResponse>.Ok(MapToResponse(entity, true, false), "Course created successfully.");
    }

    public async Task<ApiResponse<CourseResponse>> UpdateAsync(int id, CourseUpdateRequest request)
    {
        var entity = await _unitOfWork.Courses.GetByIdAsync(id);
        if (entity is null) return ApiResponse<CourseResponse>.Fail($"Course with ID {id} not found.");

        var semester = await _unitOfWork.Semesters.GetByIdAsync(request.SemesterId);
        if (semester is null) return ApiResponse<CourseResponse>.Fail($"Semester with ID {request.SemesterId} not found.");

        entity.CourseName = request.CourseName;
        entity.SemesterId = request.SemesterId;
        entity.Semester = semester;
        _unitOfWork.Courses.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<CourseResponse>.Ok(MapToResponse(entity, true, false), "Course updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Courses.GetByIdAsync(id);
        if (entity is null) return ApiResponse<bool>.Fail($"Course with ID {id} not found.");
        
        _unitOfWork.Courses.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.Ok(true, "Course deleted successfully.");
    }

    private static CourseResponse MapToResponse(Course c, bool includeSemester, bool includeEnrollments) => new()
    {
        CourseId = c.CourseId,
        CourseName = c.CourseName,
        SemesterId = c.SemesterId,
        SemesterName = includeSemester ? c.Semester?.SemesterName : null,
        Enrollments = includeEnrollments && c.Enrollments?.Any() == true
            ? c.Enrollments.Select(e => new EnrollmentResponse
            {
                EnrollmentId = e.EnrollmentId, StudentId = e.StudentId, CourseId = e.CourseId,
                EnrollDate = e.EnrollDate, Status = e.Status,
                Student = e.Student is not null ? new StudentResponse
                {
                    StudentId = e.Student.StudentId, FullName = e.Student.FullName, Email = e.Student.Email, DateOfBirth = e.Student.DateOfBirth
                } : null
            })
            : null
    };
}
