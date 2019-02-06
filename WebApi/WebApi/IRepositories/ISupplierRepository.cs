using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.IRepositories
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllSupplierAsync();
        Task AddSupplierAsync(Supplier supplier);
        Task UpdateSupplierAsync(Supplier supplier);
        Task<int> DeleteSupplierAsync(long id);
    }
}
