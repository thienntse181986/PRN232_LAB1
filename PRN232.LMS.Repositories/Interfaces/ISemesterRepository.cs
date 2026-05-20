using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Generic;

namespace PRN232.LMS.Repositories.Interfaces;

public interface ISemesterRepository : IGenericRepository<Semester>
{
    Task<Semester?> GetByIdWithCoursesAsync(int id);
}
