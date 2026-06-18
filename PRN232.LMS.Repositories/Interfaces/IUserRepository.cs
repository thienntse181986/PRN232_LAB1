using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Generic;

namespace PRN232.LMS.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}
