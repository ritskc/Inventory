using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IPoService
    {
        Task<IEnumerable<Po>> GetAllPosAsync(int companyId,int userId);
        Task<IEnumerable<Po>> GetAllPosAsync(int companyId, int userId, int supplierId);
        Task<IEnumerable<Po>> GetAllOpenPosAsync(int companyId, int userId);
        Task<Po> GetPoAsync(long poId,int userId);
        Task<Po> GetPoByAccessIdAsync(string poId,int userId);
        Task AddPoAsync(Po po);
        Task UpdatePoAsync(Po po);
        Task AcknowledgePoAsync(Po po);
        Task AcknowledgePoAsync(int poId,string accessId);
        Task DeletePoAsync(long poId);
    }
}
