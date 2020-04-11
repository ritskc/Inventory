using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using WebApi.IServices;

namespace WebApi.Services
{
    public class PriviledgeService : IPriviledgeService
    {
        private readonly IPriviledgeRepository priviledgeRepository;
        public PriviledgeService(IPriviledgeRepository priviledgeRepository)
        {
            this.priviledgeRepository = priviledgeRepository;
        }
        public async Task AddUserPriviledgeAsync(UserPriviledge userPriviledge)
        {
            await this.priviledgeRepository.AddUserPriviledgeAsync(userPriviledge);
        }

        public async Task DeleteUserPriviledgeAsync(int id)
        {
            await this.priviledgeRepository.DeleteUserPriviledgeAsync(id);
        }

        public async Task<IEnumerable<UserPriviledge>> GetAllPriviledgeAsync()
        {
            return await this.priviledgeRepository.GetAllPriviledgeAsync();
        }

        public async Task<UserPriviledge> GetPriviledgeAsync(int id)
        {
            return await this.priviledgeRepository.GetPriviledgeAsync(id);
        }

        public async Task<IEnumerable<UserPriviledgeDetail>> GetRawPriviledgeAsync()
        {
            return await this.priviledgeRepository.GetRawPriviledgeAsync();
        }

        public async Task UpdateUserPriviledgeAsync(UserPriviledge userPriviledge)
        {
            await this.priviledgeRepository.UpdateUserPriviledgeAsync(userPriviledge);
        }
    }
}
