using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllSupplierAsync(int companyId);
        Task<Supplier> GetSupplierAsync(int id);
        Task<Supplier> GetSupplierByNameAsync(int companyId,string supplierName);
        Task AddSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task<int> DeleteSupplierAsync(long id);
        Supplier GetSupplier(int id);
    }
}
