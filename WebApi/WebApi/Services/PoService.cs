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

        public PoService(IPoRepository poRepository,IPartRepository partRepository)
        {
            _poRepository = poRepository;
            _partRepository = partRepository;
        }


        public async Task<IEnumerable<Po>> GetAllPosAsync(int companyId)
        {            
            var result = await this._poRepository.GetAllPosAsync(companyId);
            foreach (Po pos in result)
            {
                foreach (PoDetail poDetail in pos.poDetails)
                {
                    var partDetail = await this._partRepository.GetPartAsync(poDetail.PartId);
                    poDetail.part = partDetail;
                }                
            }
            return result;
        }

        public async Task<Po> GetPoAsync(long id)
        {
            var result = await this._poRepository.GetPoAsync(id);
            foreach(PoDetail poDetail in result.poDetails)
            {
                var partDetail = await this._partRepository.GetPartAsync(poDetail.PartId);
                poDetail.part = partDetail;
            }
            return result;            
        }

        public async Task AddPoAsync(Po po)
        {
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
    }
}
