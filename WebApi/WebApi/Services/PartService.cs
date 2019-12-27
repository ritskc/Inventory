using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.IServices;

namespace WebApi.Services
{
    public class PartService : IPartService
    {

        private readonly IPartRepository _partRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerRepository _customerRepository;

        public PartService(IPartRepository partRepository, ISupplierRepository supplierRepository, ICustomerRepository customerRepository)
        {
            _partRepository = partRepository;
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
        }

       
        public async Task<IEnumerable<Part>> GetAllPartsAsync(int companyId)
        {
            var suppliers = await _supplierRepository.GetAllSupplierAsync(companyId);
            var customers = await _customerRepository.GetAllCustomerAsync(companyId);

            var parts=  await this._partRepository.GetAllPartsAsync(companyId);
            foreach(Part part in parts)
            {
                foreach(PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
                {
                    partSupplierAssignment.SupplierName = suppliers.Where(x => x.Id == partSupplierAssignment.SupplierID).Select(x => x.Name).FirstOrDefault();
                }
            }

            foreach (Part part in parts)
            {
                foreach (PartCustomerAssignment partCustomerAssignments in part.partCustomerAssignments)
                {
                    partCustomerAssignments.CustomerName = suppliers.Where(x => x.Id == partCustomerAssignments.CustomerId).Select(x => x.Name).FirstOrDefault();
                }
            }

            return parts;
        }

        public async Task<IEnumerable<Part>> GetPartBySupplierIdAsync(int supplierId)
        {
            return await this._partRepository.GetPartBySupplierIdAsync(supplierId);
        }

        public async Task<IEnumerable<Part>> GetPartByCustomerIdAsync(int customerId)
        {
            return await this._partRepository.GetPartByCustomerIdAsync(customerId);
        }


        public async Task<Part> GetPartAsync(long id)
        {
            return await this._partRepository.GetPartAsync(id);
        }       

        public async Task AddPartAsync(Part part)
        {            
            await this._partRepository.AddPartAsync(part);
        }        

        public async Task UpdatePartAsync(Part part)
        {
            await this._partRepository.UpdatePartAsync(part);
        }       
        
        public async Task DeletePartAsync(long id)
        {            
            await Task.Run(() => this._partRepository.DeletePartAsync(id));
        }

        public async Task UpdatePartCustomerPriceAsync(string customer, string partcode, decimal price)
        {
            await Task.Run(() => this._partRepository.UpdatePartCustomerPriceAsync(customer,partcode,price));
        }

        public async Task UpdatePartSupplierPriceAsync(string supplier, string partcode, decimal price)
        {
            await Task.Run(() => this._partRepository.UpdatePartSupplierPriceAsync(supplier, partcode, price));
        }

    }
}
