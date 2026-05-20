using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Repositories.Generic;

namespace PRN232.LMS.Repositories.Implementations;

public class CourseRepository : GenericRepository<Course>, ICourseRepository
{
    public CourseRepository(LmsDbContext context) : base(context) { }

    public async Task<Course?> GetByIdWithDetailsAsync(int id)
        => await _context.Courses
            .Include(c => c.Semester)
            .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.CourseId == id);
}
