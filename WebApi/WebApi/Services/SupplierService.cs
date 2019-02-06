using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.IRepositories;
using WebApi.IServices;
using WebApi.Models;


namespace WebApi.Services
{
    public class SupplierService:ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<Supplier>> GetAllSupplierAsync()
        {
            return await this._supplierRepository.GetAllSupplierAsync();
        }

        public async Task<Supplier> GetSupplierAsync(long id)
        {
            return await Task.Run(() => GetAllSupplierAsync().Result.Where(p => p.Id == id).FirstOrDefault());
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            await this._supplierRepository.AddSupplierAsync(supplier);
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            await this._supplierRepository.UpdateSupplierAsync(supplier);
        }

        public async Task DeleteSupplierAsync(long id)
        {
            await Task.Run(() => this._supplierRepository.DeleteSupplierAsync(id));
        }
    }
}
