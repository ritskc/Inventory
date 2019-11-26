﻿using DAL.IRepository;
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

        public PoService(IPoRepository poRepository,IPartRepository partRepository, IEntityTrackerRepository entityTrackerRepository)
        {
            _poRepository = poRepository;
            _partRepository = partRepository;
            _entityTrackerRepository = entityTrackerRepository;
        }


        public async Task<IEnumerable<Po>> GetAllPosAsync(int companyId)
        {            
            var result = await this._poRepository.GetAllPosAsync(companyId);
            var partList = await this._partRepository.GetAllPartsAsync(companyId);
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

        public async Task<Po> GetPoAsync(long id)
        {
            var result = await this._poRepository.GetPoAsync(id);
            var partList = await this._partRepository.GetAllPartsAsync(result.CompanyId);
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
            await this._entityTrackerRepository.AddEntityAsync(po.CompanyId, po.PoDate, BusinessConstants.ENTITY_TRACKER_PO);
        }

        public async Task UpdatePoAsync(Po po)
        {
            await this._poRepository.UpdatePoAsync(po);
        }

        public async Task DeletePoAsync(long id)
        {
            await Task.Run(() => this._poRepository.DeletePoAsync(id));
        }        
    }
}
