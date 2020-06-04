using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.IServices;

namespace WebApi.Services
{
    public class MasterPackingSlipService : IMasterPackingSlipService
    {
        private readonly IMasterPackingSlipRepository masterPackingSlipRepository;
        private readonly IEntityTrackerRepository entityTrackerRepository;
       

        public MasterPackingSlipService(IMasterPackingSlipRepository masterPackingSlipRepository,
            IEntityTrackerRepository entityTrackerRepository)
        {
            this.masterPackingSlipRepository = masterPackingSlipRepository;
            this.entityTrackerRepository = entityTrackerRepository;            
        }

        public async Task<int> AddMasterPackingSlipAsync(MasterPackingSlip packingSlip)
        {
            var entity = await this.entityTrackerRepository.GetEntityAsync(packingSlip.CompanyId, DateTime.Now, BusinessConstants.ENTITY_TRACKER_MASTER_PACKING_SLIP);
            packingSlip.MasterPackingSlipNo = entity.EntityNo;

            var result = await this.masterPackingSlipRepository.AddMasterPackingSlipAsync(packingSlip);
            return result;
        }

        public async Task<bool> DeleteMasterPackingSlipAsync(long id)
        {
            var result = await this.masterPackingSlipRepository.DeleteMasterPackingSlipAsync(id);
            return result;
        }

        public async Task<IEnumerable<MasterPackingSlip>> GetAllMasterPackingSlipsAsync(int companyId,int userId)
        {
            var result = await this.masterPackingSlipRepository.GetAllMasterPackingSlipsAsync(companyId,userId);
            return result;
        }

        public MasterPackingSlip GetMasterPackingSlip(long id)
        {
            var result = this.masterPackingSlipRepository.GetMasterPackingSlip(id);
            return result;
        }

        public async Task<MasterPackingSlip> GetMasterPackingSlipAsync(long Id)
        {
            var result = await this.masterPackingSlipRepository.GetMasterPackingSlipAsync(Id);
            return result;
        }

        public async Task<bool> UpdateMasterPackingSlipAsync(MasterPackingSlip packingSlip)
        {
            var result = await this.masterPackingSlipRepository.UpdateMasterPackingSlipAsync(packingSlip);
            return result;
        }

        public async Task UpdatePOSAsync(int masterPackingSlipId, string path, string trackingNumber, string accessId)
        {
            await masterPackingSlipRepository.UpdatePOSAsync(masterPackingSlipId, path, trackingNumber, accessId);
        }

        public async Task<bool> AllowScanning(int masterPackingSlipId, int userId)
        {
            var result = await this.masterPackingSlipRepository.AllowScanning(masterPackingSlipId, userId);
            return result;
        }

        public async Task<int> GetIdByAccessIdAsync(string accessId)
        {
            return await this.masterPackingSlipRepository.GetIdByAccessIdAsync(accessId);
        }
    }
}
