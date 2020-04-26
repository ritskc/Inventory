using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IUserReportRepository
    {
        Task<IEnumerable<UserMenuReport>> GetDefaultReportsAsync();
        Task<IEnumerable<UserMenuReport>> GetReportsAsync();
        Task AddUserPriviledgeAsync(UserMenuReport userMenuReport);
        Task DeleteserPriviledgeAsync(int priviledgeId);
    }
}
