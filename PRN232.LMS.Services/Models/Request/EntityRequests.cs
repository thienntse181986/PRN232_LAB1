using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Request;

// ---- Semester ----
public class SemesterCreateRequest
{
    [Required, MaxLength(100)]
    public string SemesterName { get; set; } = string.Empty;
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
}

public class SemesterUpdateRequest : SemesterCreateRequest { }

// ---- Course ----
public class CourseCreateRequest
{
    [Required, MaxLength(100)]
    public string CourseName { get; set; } = string.Empty;
    [Required]
    public int SemesterId { get; set; }
}

public class CourseUpdateRequest : CourseCreateRequest { }

// ---- Subject ----
public class SubjectCreateRequest
{
    [Required, MaxLength(20)]
    public string SubjectCode { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string SubjectName { get; set; } = string.Empty;
    [Required, Range(1, 10)]
    public int Credit { get; set; }
}

public class SubjectUpdateRequest : SubjectCreateRequest { }

// ---- Student ----
public class StudentCreateRequest
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    [Required, MaxLength(100), EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public DateTime DateOfBirth { get; set; }
}

public class StudentUpdateRequest : StudentCreateRequest { }

// ---- Enrollment ----
public class EnrollmentCreateRequest
{
    [Required]
    public int StudentId { get; set; }
    [Required]
    public int CourseId { get; set; }
    [Required]
    public DateTime EnrollDate { get; set; }
    [Required, MaxLength(20)]
    public string Status { get; set; } = "Active";
}

public class EnrollmentUpdateRequest
{
    [Required]
    public DateTime EnrollDate { get; set; }
    [Required, MaxLength(20)]
    public string Status { get; set; } = string.Empty;
}
