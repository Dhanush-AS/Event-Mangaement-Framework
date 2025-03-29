using EMS2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMS2.Services
{
    public interface IUserService
    {
        Task AddUserAsync(Users user);
        Task UpdateUserAsync(Users user);
        Task DeleteUserAsync(Guid Id);
        Task<Users> GetUserByIdAsync(Guid Id);
        Task<List<Users>> GetByEmailAsync(string email);
        Task<IEnumerable<Users>> GetAllUsersAsync();
    }
}