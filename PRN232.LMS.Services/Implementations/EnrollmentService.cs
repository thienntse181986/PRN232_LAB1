using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<EnrollmentResponse>> GetAllAsync(QueryParameters query)
    {
        var q = _unitOfWork.Enrollments.GetQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.ToLower();
            q = q.Include(e => e.Student).Include(e => e.Course)
                 .Where(x => x.Student.FullName.ToLower().Contains(s)
                           || x.Course.CourseName.ToLower().Contains(s)
                           || x.Status.ToLower().Contains(s));
        }

        bool expandStudent = query.Expand?.Contains("student", StringComparison.OrdinalIgnoreCase) == true;
        bool expandCourse  = query.Expand?.Contains("course",  StringComparison.OrdinalIgnoreCase) == true;

        if (expandStudent && !q.Expression.ToString().Contains("Student"))
            q = q.Include(e => e.Student);
        if (expandCourse && !q.Expression.ToString().Contains("Course"))
            q = q.Include(e => e.Course).ThenInclude(c => c.Semester);

        q = QueryHelper.ApplySorting(q, query.Sort);
        var (items, total) = await QueryHelper.ApplyPagingAsync(q, query.Page, query.Size);

        return PagedResponse<EnrollmentResponse>.Ok(items.Select(x => MapToResponse(x, expandStudent, expandCourse)), new PaginationMeta
        {
            Page = query.Page, PageSize = query.Size, TotalItems = total
        });
    }

    public async Task<ApiResponse<EnrollmentResponse>> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Enrollments.GetByIdWithDetailsAsync(id);
        if (entity is null) return ApiResponse<EnrollmentResponse>.Fail($"Enrollment with ID {id} not found.");
        return ApiResponse<EnrollmentResponse>.Ok(MapToResponse(entity, true, true));
    }

    public async Task<ApiResponse<EnrollmentResponse>> CreateAsync(EnrollmentCreateRequest request)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId);
        if (student is null) return ApiResponse<EnrollmentResponse>.Fail($"Student with ID {request.StudentId} not found.");

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null) return ApiResponse<EnrollmentResponse>.Fail($"Course with ID {request.CourseId} not found.");

        // Check duplicate
        var exists = await _unitOfWork.Enrollments.FindAsync(e => e.StudentId == request.StudentId && e.CourseId == request.CourseId);
        if (exists.Any()) return ApiResponse<EnrollmentResponse>.Fail("Student is already enrolled in this course.");

        var entity = new Enrollment
        {
            StudentId = request.StudentId,
            CourseId = request.CourseId,
            EnrollDate = request.EnrollDate,
            Status = request.Status
        };
        await _unitOfWork.Enrollments.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        entity.Student = student;
        entity.Course = course;
        return ApiResponse<EnrollmentResponse>.Ok(MapToResponse(entity, true, true), "Enrollment created successfully.");
    }

    public async Task<ApiResponse<EnrollmentResponse>> UpdateAsync(int id, EnrollmentUpdateRequest request)
    {
        var entity = await _unitOfWork.Enrollments.GetByIdAsync(id);
        if (entity is null) return ApiResponse<EnrollmentResponse>.Fail($"Enrollment with ID {id} not found.");

        entity.EnrollDate = request.EnrollDate;
        entity.Status = request.Status;
        _unitOfWork.Enrollments.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<EnrollmentResponse>.Ok(MapToResponse(entity, false, false), "Enrollment updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Enrollments.GetByIdAsync(id);
        if (entity is null) return ApiResponse<bool>.Fail($"Enrollment with ID {id} not found.");
        
        _unitOfWork.Enrollments.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.Ok(true, "Enrollment deleted successfully.");
    }

    private static EnrollmentResponse MapToResponse(Enrollment e, bool includeStudent, bool includeCourse) => new()
    {
        EnrollmentId = e.EnrollmentId,
        StudentId = e.StudentId,
        CourseId = e.CourseId,
        EnrollDate = e.EnrollDate,
        Status = e.Status,
        Student = includeStudent && e.Student is not null ? new StudentResponse
        {
            StudentId = e.Student.StudentId,
            FullName = e.Student.FullName,
            Email = e.Student.Email,
            DateOfBirth = e.Student.DateOfBirth
        } : null,
        Course = includeCourse && e.Course is not null ? new CourseResponse
        {
            CourseId = e.Course.CourseId,
            CourseName = e.Course.CourseName,
            SemesterId = e.Course.SemesterId,
            SemesterName = e.Course.Semester?.SemesterName
        } : null
    };
}
