using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;

namespace PRN232.LMS.Repositories.Implementations;

public class SemesterRepository : GenericRepository<Semester>, ISemesterRepository
{
    public SemesterRepository(LmsDbContext context) : base(context) { }

    public async Task<Semester?> GetByIdWithCoursesAsync(int id)
        => await _context.Semesters
            .Include(s => s.Courses)
            .FirstOrDefaultAsync(s => s.SemesterId == id);
}
