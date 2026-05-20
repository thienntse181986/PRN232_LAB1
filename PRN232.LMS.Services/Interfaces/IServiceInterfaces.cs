using PRN232.LMS.Services.Models.Request;
using PRN232.LMS.Services.Models.Response;

namespace PRN232.LMS.Services.Interfaces;

public interface IStudentService
{
    Task<PagedResponse<StudentResponse>> GetAllAsync(QueryParameters query);
    Task<ApiResponse<StudentResponse>> GetByIdAsync(int id);
    Task<ApiResponse<StudentResponse>> CreateAsync(StudentCreateRequest request);
    Task<ApiResponse<StudentResponse>> UpdateAsync(int id, StudentUpdateRequest request);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface ICourseService
{
    Task<PagedResponse<CourseResponse>> GetAllAsync(QueryParameters query);
    Task<ApiResponse<CourseResponse>> GetByIdAsync(int id);
    Task<ApiResponse<CourseResponse>> CreateAsync(CourseCreateRequest request);
    Task<ApiResponse<CourseResponse>> UpdateAsync(int id, CourseUpdateRequest request);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface ISubjectService
{
    Task<PagedResponse<SubjectResponse>> GetAllAsync(QueryParameters query);
    Task<ApiResponse<SubjectResponse>> GetByIdAsync(int id);
    Task<ApiResponse<SubjectResponse>> CreateAsync(SubjectCreateRequest request);
    Task<ApiResponse<SubjectResponse>> UpdateAsync(int id, SubjectUpdateRequest request);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface ISemesterService
{
    Task<PagedResponse<SemesterResponse>> GetAllAsync(QueryParameters query);
    Task<ApiResponse<SemesterResponse>> GetByIdAsync(int id);
    Task<ApiResponse<SemesterResponse>> CreateAsync(SemesterCreateRequest request);
    Task<ApiResponse<SemesterResponse>> UpdateAsync(int id, SemesterUpdateRequest request);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}

public interface IEnrollmentService
{
    Task<PagedResponse<EnrollmentResponse>> GetAllAsync(QueryParameters query);
    Task<ApiResponse<EnrollmentResponse>> GetByIdAsync(int id);
    Task<ApiResponse<EnrollmentResponse>> CreateAsync(EnrollmentCreateRequest request);
    Task<ApiResponse<EnrollmentResponse>> UpdateAsync(int id, EnrollmentUpdateRequest request);
    Task<ApiResponse<bool>> DeleteAsync(int id);
}
