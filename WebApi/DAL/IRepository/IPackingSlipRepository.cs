using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IPackingSlipRepository
    {
        Task<IEnumerable<PackingSlip>> GetAllPackingSlipsAsync(int companyId);
        Task<PackingSlip> GetPackingSlipAsync(long Id);
        PackingSlip GetPackingSlip(long id);
        Task<Int32> AddPackingSlipAsync(PackingSlip packingSlip);
        Task<bool> UpdatePackingSlipAsync(PackingSlip packingSlip);
        Task<bool> DeletePackingSlipAsync(long id);
        Task CreateInvoiceAsync(PackingSlip packingSlip);
        Task UpdatePOSAsync(int packingSlipId, string path,string trackingNumber);
    }
}
