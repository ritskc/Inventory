using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using WebApi.IServices;

namespace WebApi.Services
{
    public class PackingSlipService : IPackingSlipService
    {
        private readonly IPackingSlipRepository packingSlipRepository;
        private readonly IEntityTrackerRepository entityTrackerRepository;
        private readonly ITransactionRepository transactionRepository;

        public PackingSlipService(IPackingSlipRepository packingSlipRepository, IEntityTrackerRepository entityTrackerRepository,ITransactionRepository transactionRepository)
        {
            this.packingSlipRepository = packingSlipRepository;
            this.entityTrackerRepository = entityTrackerRepository;
            this.transactionRepository = transactionRepository;
        }

        public async Task<Int32> AddPackingSlipAsync(PackingSlip packingSlip)
        {
            var entity = await this.entityTrackerRepository.GetEntityAsync(packingSlip.CompanyId, packingSlip.ShippingDate, BusinessConstants.ENTITY_TRACKER_PACKING_SLIP);
            packingSlip.PackingSlipNo = entity.EntityNo;

            var result = await this.packingSlipRepository.AddPackingSlipAsync(packingSlip);

            await this.entityTrackerRepository.AddEntityAsync(packingSlip.CompanyId, packingSlip.ShippingDate, BusinessConstants.ENTITY_TRACKER_PACKING_SLIP);

            foreach(PackingSlipDetails packingSlipDetails in packingSlip.PackingSlipDetails)
            {
                var transactionDetail = new TransactionDetail();
                transactionDetail.PartId = packingSlipDetails.PartId;
                transactionDetail.Qty = packingSlipDetails.Qty;
                transactionDetail.TransactionTypeId = BusinessConstants.TRANSACTION_TYPE.CUSTOMER_PACKINGSLIP;
                transactionDetail.TransactionDate = DateTime.Now;
                transactionDetail.DirectionId = BusinessConstants.DIRECTION.OUT;
                transactionDetail.InventoryType = BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND;
                transactionDetail.ReferenceNo = packingSlip.PackingSlipNo;
                await this.transactionRepository.AddTransactionAsync(transactionDetail);
            }
            return result;
        }

        public async Task CreateInvoiceAsync(PackingSlip packingSlip)
        {
            await this.packingSlipRepository.CreateInvoiceAsync(packingSlip);
        }

        public Task<int> DeleteSupplierInvoiceAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PackingSlip>> GetAllPackingSlipsAsync(int companyId)
        {
            return await this.packingSlipRepository.GetAllPackingSlipsAsync(companyId);
        }

        public async Task<PackingSlip> GetPackingSlipAsync(long Id)
        {
            return await this.packingSlipRepository.GetPackingSlipAsync(Id);
        }

        public Task UpdatePackingSlipAsync(PackingSlip packingSlip)
        {
            throw new NotImplementedException();
        }

        public async Task UpdatePOSAsync(int packingSlipId, string path,string trackingNumber)
        {
            await packingSlipRepository.UpdatePOSAsync(packingSlipId,path, trackingNumber);
        }
    }
}
