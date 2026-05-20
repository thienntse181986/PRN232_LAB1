using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;

namespace PRN232.LMS.Repositories.Implementations;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(LmsDbContext context) : base(context) { }

    public async Task<Student?> GetByIdWithEnrollmentsAsync(int id)
        => await _context.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                    .ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(s => s.StudentId == id);

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _context.Students.Where(s => s.Email == email);
        if (excludeId.HasValue)
            query = query.Where(s => s.StudentId != excludeId.Value);
        return await query.AnyAsync();
    }
}
