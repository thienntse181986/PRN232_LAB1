using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Generic;

public interface IUnitOfWork : IDisposable
{
    IStudentRepository Students { get; }
    ICourseRepository Courses { get; }
    ISubjectRepository Subjects { get; }
    ISemesterRepository Semesters { get; }
    IEnrollmentRepository Enrollments { get; }

    Task<int> SaveChangesAsync();
    
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
