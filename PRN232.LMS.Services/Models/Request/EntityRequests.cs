using System.ComponentModel.DataAnnotations;
using PRN232.LMS.Services.Validation;

namespace PRN232.LMS.Services.Models.Request;

// ---- Semester ----
public class SemesterCreateRequest
{
    [Required(ErrorMessage = "Semester Name is required.")]
    [StringLength(100, ErrorMessage = "Semester Name cannot exceed 100 characters.")]
    public string SemesterName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Start Date is required.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End Date is required.")]
    public DateTime EndDate { get; set; }
}

public class SemesterUpdateRequest : SemesterCreateRequest { }

// ---- Course ----
public class CourseCreateRequest
{
    [Required(ErrorMessage = "Course Name is required.")]
    [StringLength(100, ErrorMessage = "Course Name cannot exceed 100 characters.")]
    public string CourseName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Semester ID is required.")]
    public int SemesterId { get; set; }
}

public class CourseUpdateRequest : CourseCreateRequest { }

// ---- Subject ----
public class SubjectCreateRequest
{
    [Required(ErrorMessage = "Subject Code is required.")]
    [StringLength(20, ErrorMessage = "Subject Code cannot exceed 20 characters.")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Subject Name is required.")]
    [StringLength(100, ErrorMessage = "Subject Name cannot exceed 100 characters.")]
    public string SubjectName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Credit is required.")]
    [Range(1, 10, ErrorMessage = "Credit must be between 1 and 10.")]
    public int Credit { get; set; }
}

public class SubjectUpdateRequest : SubjectCreateRequest { }

// ---- Student ----
public class StudentCreateRequest
{
    [Required(ErrorMessage = "Student Code is required.")]
    [FptStudentCode]
    public string StudentCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 100 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of Birth is required.")]
    public DateTime DateOfBirth { get; set; }
}

public class StudentUpdateRequest : StudentCreateRequest { }

// ---- Enrollment ----
public class EnrollmentCreateRequest
{
    [Required(ErrorMessage = "Student ID is required.")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Course ID is required.")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Enroll Date is required.")]
    public DateTime EnrollDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
    public string Status { get; set; } = "Active";
}

public class EnrollmentUpdateRequest
{
    [Required(ErrorMessage = "Enroll Date is required.")]
    public DateTime EnrollDate { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
    public string Status { get; set; } = string.Empty;
}
