using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IPoService
    {
        Task<IEnumerable<Po>> GetAllPosAsync(int companyId);
        Task<Po> GetPoAsync(long poId);
        Task AddPoAsync(Po po);
        Task UpdatePoAsync(Po po);
        Task DeletePoAsync(long poId);
    }
}
