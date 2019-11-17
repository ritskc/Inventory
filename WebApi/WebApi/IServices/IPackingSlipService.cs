using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IPackingSlipService
    {
        Task<IEnumerable<PackingSlip>> GetAllPackingSlipsAsync(int companyId);
        Task<PackingSlip> GetPackingSlipAsync(long Id);
        Task<Int32> AddPackingSlipAsync(PackingSlip packingSlip);
        Task UpdatePackingSlipAsync(PackingSlip packingSlip);
        Task<int> DeleteSupplierInvoiceAsync(long id);
        Task CreateInvoiceAsync(PackingSlip packingSlip);
        Task UpdatePOSAsync(int packingSlipId, string path,string trackingNumber);
    }
}
