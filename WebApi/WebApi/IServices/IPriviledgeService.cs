using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IPriviledgeService
    {
        Task<IEnumerable<UserPriviledgeDetail>> GetRawPriviledgeAsync();
        Task<IEnumerable<UserPriviledge>> GetAllPriviledgeAsync();
        Task<UserPriviledge> GetPriviledgeAsync(int id);
        Task AddUserPriviledgeAsync(UserPriviledge userPriviledge);
        Task UpdateUserPriviledgeAsync(UserPriviledge userPriviledge);
        Task DeleteUserPriviledgeAsync(int id);
    }
}
