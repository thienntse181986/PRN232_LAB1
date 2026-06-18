using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Context;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Generic;
using PRN232.LMS.Repositories.Interfaces;

namespace PRN232.LMS.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(LmsDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }
}
