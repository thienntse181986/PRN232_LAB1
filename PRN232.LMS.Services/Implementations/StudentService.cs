using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;
using PRN232.LMS.Services.Helpers;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResponse<StudentResponse>> GetAllAsync(QueryParameters query)
    {
        var q = _unitOfWork.Students.GetQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = query.Search.ToLower();
            q = q.Where(x => x.FullName.ToLower().Contains(s) || x.Email.ToLower().Contains(s));
        }

        // Expand enrollments
        bool expandEnrollments = query.Expand?.Contains("enrollment", StringComparison.OrdinalIgnoreCase) == true;
        if (expandEnrollments)
        {
            q = q.Include(x => x.Enrollments).ThenInclude(e => e.Course).ThenInclude(c => c.Semester);
        }

        // Sort
        q = QueryHelper.ApplySorting(q, query.Sort);

        var (items, total) = await QueryHelper.ApplyPagingAsync(q, query.Page, query.Size);

        var response = items.Select(s => MapToResponse(s, expandEnrollments));

        return PagedResponse<StudentResponse>.Ok(response, new PaginationMeta
        {
            Page = query.Page,
            PageSize = query.Size,
            TotalItems = total
        });
    }

    public async Task<ApiResponse<StudentResponse>> GetByIdAsync(int id)
    {
        var student = await _unitOfWork.Students.GetByIdWithEnrollmentsAsync(id);
        if (student is null)
            return ApiResponse<StudentResponse>.Fail($"Student with ID {id} not found.");
        return ApiResponse<StudentResponse>.Ok(MapToResponse(student, true));
    }

    public async Task<ApiResponse<StudentResponse>> CreateAsync(StudentCreateRequest request)
    {
        if (await _unitOfWork.Students.EmailExistsAsync(request.Email))
            return ApiResponse<StudentResponse>.Fail($"Email '{request.Email}' already exists.");

        var entity = new Student
        {
            FullName = request.FullName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth
        };
        await _unitOfWork.Students.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<StudentResponse>.Ok(MapToResponse(entity, false), "Student created successfully.");
    }

    public async Task<ApiResponse<StudentResponse>> UpdateAsync(int id, StudentUpdateRequest request)
    {
        var entity = await _unitOfWork.Students.GetByIdAsync(id);
        if (entity is null)
            return ApiResponse<StudentResponse>.Fail($"Student with ID {id} not found.");

        if (await _unitOfWork.Students.EmailExistsAsync(request.Email, id))
            return ApiResponse<StudentResponse>.Fail($"Email '{request.Email}' already exists.");

        entity.FullName = request.FullName;
        entity.Email = request.Email;
        entity.DateOfBirth = request.DateOfBirth;

        _unitOfWork.Students.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponse<StudentResponse>.Ok(MapToResponse(entity, false), "Student updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Students.GetByIdAsync(id);
        if (entity is null)
            return ApiResponse<bool>.Fail($"Student with ID {id} not found.");
            
        _unitOfWork.Students.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return ApiResponse<bool>.Ok(true, "Student deleted successfully.");
    }

    private static StudentResponse MapToResponse(Student s, bool includeEnrollments)
    {
        return new StudentResponse
        {
            StudentId = s.StudentId,
            FullName = s.FullName,
            Email = s.Email,
            DateOfBirth = s.DateOfBirth,
            Enrollments = includeEnrollments && s.Enrollments?.Any() == true
                ? s.Enrollments.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                    Course = e.Course is not null ? new CourseResponse
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        SemesterId = e.Course.SemesterId,
                        SemesterName = e.Course.Semester?.SemesterName
                    } : null
                })
                : null
        };
    }
}
