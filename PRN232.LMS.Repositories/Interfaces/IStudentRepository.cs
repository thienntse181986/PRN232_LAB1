using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Generic;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByIdWithEnrollmentsAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}
