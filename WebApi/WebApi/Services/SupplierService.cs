using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.IServices;



namespace WebApi.Services
{
    public class SupplierService:ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<Supplier>> GetAllSupplierAsync(int companyId)
        {
            return await this._supplierRepository.GetAllSupplierAsync(companyId);
        }

        public async Task<Supplier> GetSupplierAsync(int id)
        {
            return await this._supplierRepository.GetSupplierAsync(id);
            
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

        public async Task<Supplier> GetSupplierByNameAsync(int companyId, string supplierName)
        {
            return await this._supplierRepository.GetSupplierByNameAsync(companyId,supplierName);
        }
    }
}
