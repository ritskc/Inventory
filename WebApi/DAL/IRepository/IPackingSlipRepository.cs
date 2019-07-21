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
        Task AddPackingSlipAsync(PackingSlip packingSlip);
        Task UpdatePackingSlipAsync(PackingSlip packingSlip);
        Task<int> DeleteSupplierInvoiceAsync(long id);
        Task CreateInvoiceAsync(PackingSlip packingSlip);
    }
}
