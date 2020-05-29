using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.IRepository
{
    public interface IUserRepository
    {
        Task<User> GetUserPropertyAsync(string userName);
        Task<User> GetUserWithPasswordAsync(string userName);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GeUserbyIdAsync(int userId);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(long id);
    }
}
