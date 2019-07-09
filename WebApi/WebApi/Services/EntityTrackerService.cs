using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using WebApi.IServices;

namespace WebApi.Services
{
    public class EntityTrackerService : IEntityTrackerService
    {
        private readonly IEntityTrackerRepository entityTrackerRepository;
        public EntityTrackerService(IEntityTrackerRepository entityTrackerRepository)
        {
            this.entityTrackerRepository = entityTrackerRepository;
        }

        public Task AddEntityTrackerAsyncAsync(EntityTracker entityTracker)
        {
            throw new NotImplementedException();
        }       

        public async Task<IEnumerable<EntityTracker>> GetAllEntityTrackerAsync(int companyId)
        {
            return await this.entityTrackerRepository.GetAllEntityTrackerAsync(companyId);
        }        

        public async Task<EntityTracker> GetEntityTrackerAsync(int companyId, string FinYear)
        {
            return await this.entityTrackerRepository.GetEntityTrackerAsync(companyId, FinYear);
        }

        public async Task<EntityTracker> GetEntityAsync(int companyId, DateTime dateTime,string entity)
        {
            return await this.entityTrackerRepository.GetEntityAsync(companyId, dateTime,entity);
        }

        public async Task AddEntityAsync(int companyId, DateTime dateTime, string entity)
        {
            await this.entityTrackerRepository.AddEntityAsync(companyId, dateTime, entity);
        }
    }
}
