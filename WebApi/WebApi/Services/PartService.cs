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
                    part.SupplierPrice = partSupplierAssignment.UnitPrice;
                }
            }

            foreach (Part part in parts)
            {
                foreach (PartCustomerAssignment partCustomerAssignments in part.partCustomerAssignments)
                {
                    partCustomerAssignments.CustomerName = customers.Where(x => x.Id == partCustomerAssignments.CustomerId).Select(x => x.Name).FirstOrDefault();
                    part.CustomerPrice = partCustomerAssignments.Rate;
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

        public async Task UpdatePartCustomerPriceAsync(int companyId, string customer, string partcode, decimal price)
        {
            await Task.Run(() => this._partRepository.UpdatePartCustomerPriceAsync(companyId,customer, partcode,price));
        }

        public async Task UpdatePartSupplierPriceAsync(int companyId, string supplier, string partcode, decimal price)
        {
            await Task.Run(() => this._partRepository.UpdatePartSupplierPriceAsync(companyId,supplier, partcode, price));
        }

        public async Task UpdateOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty)
        {
            await Task.Run(() => this._partRepository.UpdateOpeningQtyByPartCodeAsync(companyId,  partcode, openingQty));
        }

        public async Task UpdateOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty)
        {
            await Task.Run(() => this._partRepository.UpdateOpeningQtyByPartIdAsync(companyId, partId, openingQty));
        }

        public async Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand)
        {
            await Task.Run(() => this._partRepository.UpdateQtyInHandByPartIdAsync(companyId, partId, QtyInHand));
        }

        public async Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note)
        {
            await Task.Run(() => this._partRepository.UpdateQtyInHandByPartIdAsync(companyId, partId, QtyInHand,direction,note));
        }

        public async Task<IEnumerable<PartInTransit>> GetPartInTransitDetailAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartInTransitDetailAsync(partId, companyId));
        }

        public async Task<IEnumerable<PartOpenOrder>> GetPartOpenOrderDetailAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartOpenOrderDetailAsync(partId, companyId));
        }

        public async Task<IEnumerable<SupplierOpenPO>> GetPartOpenPODetailAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartOpenPODetailAsync(partId, companyId));
        }

        public async Task<IEnumerable<PartLatestShipment>> GetPartLatestShipmentAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartLatestShipmentAsync(partId, companyId));
        }

        public async Task<IEnumerable<PartInTransit>> GetPartLatestReceivedAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartLatestReceivedAsync(partId, companyId));
        }
    }
}
