using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.IServices;

namespace WebApi.Services
{
    public class MonthlyInvoiceService : IMonthlyInvoiceService
    {
        private readonly IMonthlyInvoiceRepository packingSlipRepository;
        private readonly IEntityTrackerRepository entityTrackerRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly IPartService partService;

        public MonthlyInvoiceService(IMonthlyInvoiceRepository packingSlipRepository,
            IEntityTrackerRepository entityTrackerRepository, ITransactionRepository transactionRepository
            , IPartService partService)
        {
            this.packingSlipRepository = packingSlipRepository;
            this.entityTrackerRepository = entityTrackerRepository;
            this.transactionRepository = transactionRepository;
            this.partService = partService;
        }

        public async Task<Int32> AddPackingSlipAsync(PackingSlip packingSlip)
        {
            //var deletedPackingSlips = await this.packingSlipRepository.GetDeletedPackingSlipAsync(packingSlip.CompanyId);
            //if (deletedPackingSlips != null & deletedPackingSlips.Count() > 0)
            //{
            //    packingSlip.PackingSlipNo = deletedPackingSlips.Select(x => x.PackingSlipNo).FirstOrDefault();
            //    packingSlip.IsDeletedPackingSlipNoUsed = true;
            //}
            //else
            //{
            //    var entity = await this.entityTrackerRepository.GetEntityAsync(packingSlip.CompanyId, packingSlip.ShippingDate, BusinessConstants.ENTITY_TRACKER_PACKING_SLIP);
            //    packingSlip.PackingSlipNo = entity.EntityNo;
            //    packingSlip.IsDeletedPackingSlipNoUsed = false;
            //}

            var entity = await this.entityTrackerRepository.GetEntityAsync(packingSlip.CompanyId, packingSlip.ShippingDate, BusinessConstants.ENTITY_TRACKER_MONTHLY_INVOICE);
            packingSlip.PackingSlipNo = entity.EntityNo;
            var result = await this.packingSlipRepository.AddPackingSlipAsync(packingSlip);
            return result;
        }        

        public async Task<bool> DeletePackingSlipAsync(long id)
        {
            var result = await this.packingSlipRepository.DeletePackingSlipAsync(id);
            return result;
        }

        public async Task<IEnumerable<PackingSlip>> GetAllPackingSlipsAsync(int companyId, int userId)
        {
            var partList = await this.partService.GetAllPartsAsync(companyId, userId);
            var result = await this.packingSlipRepository.GetAllPackingSlipsAsync(companyId, userId);

            foreach (PackingSlip packingSlip in result)
            {
                foreach (PackingSlipDetails packingSlipDetails in packingSlip.PackingSlipDetails)
                {
                    var partDetail = partList.Where(p => p.Id == packingSlipDetails.PartId).FirstOrDefault();
                    packingSlipDetails.PartDetail = partDetail;
                }
            }
            return result;
        }

        public async Task<PackingSlip> GetPackingSlipAsync(long Id)
        {
            var result = await this.packingSlipRepository.GetPackingSlipAsync(Id);
            //var partList = await this.partService.GetAllPartsAsync(result.CompanyId);
            foreach (PackingSlipDetails packingSlipDetails in result.PackingSlipDetails)
            {
                var partDetail = await this.partService.GetPartAsync(packingSlipDetails.PartId);//partList.Where(p => p.Id == packingSlipDetails.PartId).FirstOrDefault();
                packingSlipDetails.PartDetail = partDetail;
            }
            return result;
        }

        public async Task<bool> UpdatePackingSlipAsync(PackingSlip packingSlip)
        {
            var result = await this.packingSlipRepository.UpdatePackingSlipAsync(packingSlip);
            return result;
        }        
    }
}
