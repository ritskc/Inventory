using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using WebApi.IServices;

namespace WebApi.Services
{
    public class UserReportService : IUserReportService
    {
        private readonly IUserReportRepository userReportRepository;
        public UserReportService(IUserReportRepository userReportRepository)
        {
            this.userReportRepository = userReportRepository;
        }
        public Task AddUserPriviledgeAsync(UserMenuReport userMenuReport)
        {
            return this.userReportRepository.AddUserPriviledgeAsync(userMenuReport);
        }

        public  Task DeleteserPriviledgeAsync(int priviledgeId)
        {
            return this.userReportRepository.DeleteserPriviledgeAsync(priviledgeId);
        }

        public Task<IEnumerable<UserMenuReport>> GetDefaultReportsAsync()
        {
            return this.userReportRepository.GetDefaultReportsAsync();
        }

        public Task<IEnumerable<UserMenuReport>> GetReportsAsync()
        {
            return this.userReportRepository.GetReportsAsync();
        }
    }
}
