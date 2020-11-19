using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.IServices;

namespace WebApi.Services
{
    public class PoService : IPoService
    {
        private readonly IPoRepository _poRepository;
        private readonly IPartRepository _partRepository;
        private readonly IEntityTrackerRepository _entityTrackerRepository;
        private readonly IPartService _partService;
        private readonly ICompanyRepository companyRepository;

        public PoService(IPoRepository poRepository,IPartRepository partRepository, IEntityTrackerRepository entityTrackerRepository,
            IPartService partService, ICompanyRepository companyRepository)
        {
            _poRepository = poRepository;
            _partRepository = partRepository;
            _entityTrackerRepository = entityTrackerRepository;
            _partService = partService;
            this.companyRepository = companyRepository;
        }


        public async Task<IEnumerable<Po>> GetAllPosAsync(int companyId,int userId)
        {            
            var result = await this._poRepository.GetAllPosAsync(companyId,userId);
            var partList = await this._partService.GetAllPartsAsync(companyId,userId);
            foreach (Po pos in result)
            {
                foreach (PoDetail poDetail in pos.poDetails)
                {
                    var partDetail = partList.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                    poDetail.part = partDetail;
                }                
            }
            return result;
        }

        public async Task<IEnumerable<Po>> GetAllPosAsync(int companyId, int userId,int supplierId)
        {
            var result = await this._poRepository.GetAllPosAsync(companyId, userId, supplierId);
            var partList = await this._partService.GetAllPartsAsync(companyId, userId);
            foreach (Po pos in result)
            {
                foreach (PoDetail poDetail in pos.poDetails)
                {
                    var partDetail = partList.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                    poDetail.part = partDetail;
                }
            }
            return result;
        }

        public async Task<IEnumerable<Po>> GetAllOpenPosAsync(int companyId, int userId)
        {
            var result = await this._poRepository.GetAllOpenPosAsync(companyId, userId);
            var partList = await this._partService.GetAllPartsCompactAsync(companyId, userId);
            foreach (Po pos in result)
            {
                foreach (PoDetail poDetail in pos.poDetails)
                {
                    var partDetail = partList.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                    poDetail.part = partDetail;
                }
            }
            return result;
        }

        public async Task<Po> GetPoAsync(long id,int userId)
        {
            var result = await this._poRepository.GetPoAsync(id);
            var partList = await this._partService.GetAllPartsAsync(result.CompanyId,userId);
            foreach (PoDetail poDetail in result.poDetails)
            {
                var partDetail = partList.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                poDetail.part = partDetail;
            }
            return result;            
        }

        public async Task<Po> GetPoByAccessIdAsync(string id,int userId)
        {
            var result = await this._poRepository.GetPoByAccessIdAsync(id);
            var partList = await this._partService.GetAllPartsAsync(result.CompanyId,userId);
            foreach (PoDetail poDetail in result.poDetails)
            {
                var partDetail = partList.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                poDetail.part = partDetail;
            }
            return result;
        }

        public async Task AddPoAsync(Po po)
        {
            var entity = await this._entityTrackerRepository.GetEntityAsync(po.CompanyId,po.PoDate,BusinessConstants.ENTITY_TRACKER_PO);
            po.PoNo = entity.EntityNo;           
            await this._poRepository.AddPoAsync(po);            
        }

        public async Task UpdatePoAsync(Po po)
        {
            await this._poRepository.UpdatePoAsync(po);
        }

        public async Task DeletePoAsync(long id)
        {
            await Task.Run(() => this._poRepository.DeletePoAsync(id));
        }

        public async Task AcknowledgePoAsync(Po po)
        {
            await this._poRepository.AcknowledgePoAsync(po);
        }

        public async Task AcknowledgePoAsync(int poId,string accessId)
        {
            await this._poRepository.AcknowledgePoAsync(poId, accessId);
        }
    }
}
