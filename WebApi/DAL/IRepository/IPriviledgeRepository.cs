using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IPriviledgeRepository
    {
        Task<IEnumerable<UserPriviledgeDetail>> GetRawPriviledgeAsync();
        Task<IEnumerable<UserPriviledge>> GetAllPriviledgeAsync();
        Task<UserPriviledge> GetPriviledgeAsync(int id);
        Task<UserPriviledge> GetFormattedPriviledgeAsync(int id); 
        Task AddUserPriviledgeAsync(UserPriviledge userPriviledge);
        Task UpdateUserPriviledgeAsync(UserPriviledge userPriviledge);
        Task DeleteUserPriviledgeAsync(int id);
    }
}
