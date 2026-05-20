using Microsoft.EntityFrameworkCore.Storage;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Generic;

public class UnitOfWork : IUnitOfWork
{
    private readonly LmsDbContext _context;
    private IDbContextTransaction? _transaction;

    public IStudentRepository Students { get; }
    public ICourseRepository Courses { get; }
    public ISubjectRepository Subjects { get; }
    public ISemesterRepository Semesters { get; }
    public IEnrollmentRepository Enrollments { get; }

    public UnitOfWork(
        LmsDbContext context,
        IStudentRepository studentRepository,
        ICourseRepository courseRepository,
        ISubjectRepository subjectRepository,
        ISemesterRepository semesterRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _context = context;
        Students = studentRepository;
        Courses = courseRepository;
        Subjects = subjectRepository;
        Semesters = semesterRepository;
        Enrollments = enrollmentRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction?.Dispose();
    }
}
