using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IUserReportService
    {
        Task<IEnumerable<UserMenuReport>> GetDefaultReportsAsync();
        Task<IEnumerable<UserMenuReport>> GetReportsAsync();
        Task AddUserPriviledgeAsync(UserMenuReport userMenuReport);
        Task DeleteserPriviledgeAsync(int priviledgeId);
    }
}
