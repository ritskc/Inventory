using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IEntityTrackerService
    {
        Task<IEnumerable<EntityTracker>> GetAllEntityTrackerAsync(int companyId);
        Task<EntityTracker> GetEntityTrackerAsync(int companyId, string FinYear);
        Task AddEntityTrackerAsyncAsync(EntityTracker entityTracker);
        Task<EntityTracker> GetEntityAsync(int companyId, DateTime dateTime, string entity);
        Task AddEntityAsync(int companyId, DateTime dateTime, string entity);
    }
}
