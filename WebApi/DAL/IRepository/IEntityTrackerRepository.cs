using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IEntityTrackerRepository
    {
        Task<IEnumerable<EntityTracker>> GetAllEntityTrackerAsync(int companyId);
        Task<EntityTracker> GetEntityTrackerAsync(int companyId,string FinYear);
        Task AddEntityTrackerAsyncAsync(EntityTracker entityTracker);
    }
}
