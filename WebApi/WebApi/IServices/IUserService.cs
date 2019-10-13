using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;

namespace WebApi.IServices
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        Task<User> GetUserAsync(string userName);

        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GeUserbyIdAsync(int userId);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(long id);
    }
}
