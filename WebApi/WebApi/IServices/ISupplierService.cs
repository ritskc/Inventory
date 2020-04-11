using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;

namespace WebApi.IServices
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSupplierAsync(int companyId,int userId);
        Task<Supplier> GetSupplierAsync(int id);
        Task<Supplier> GetSupplierByNameAsync(int companyId, string supplierName);
        Task AddSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task DeleteSupplierAsync(long id);
    }
}
