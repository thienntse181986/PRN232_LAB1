using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;

namespace PRN232.LMS.Repositories.Implementations;

public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(LmsDbContext context) : base(context) { }

    public async Task<Enrollment?> GetByIdWithDetailsAsync(int id)
        => await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
                .ThenInclude(c => c.Semester)
            .FirstOrDefaultAsync(e => e.EnrollmentId == id);
}
