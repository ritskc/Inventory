using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IMonthlyInvoiceService
    {
        Task<IEnumerable<PackingSlip>> GetAllPackingSlipsAsync(int companyId, int userId);
        Task<PackingSlip> GetPackingSlipAsync(long Id);
        Task<Int32> AddPackingSlipAsync(PackingSlip packingSlip);
        Task<bool> UpdatePackingSlipAsync(PackingSlip packingSlip);
        Task<bool> DeletePackingSlipAsync(long id);        
    }
}
