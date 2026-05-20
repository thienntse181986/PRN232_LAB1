using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Generic;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IEnrollmentRepository : IGenericRepository<Enrollment>
{
    Task<Enrollment?> GetByIdWithDetailsAsync(int id);
}
