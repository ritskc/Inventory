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

        public PoService(IPoRepository poRepository)
        {
            _poRepository = poRepository;
        }


        public async Task<IEnumerable<Po>> GetAllPosAsync(int companyId)
        {
            return await this._poRepository.GetAllPosAsync(companyId);
        }

        public async Task<Po> GetPoAsync(long id)
        {
            return await this._poRepository.GetPoAsync(id);
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
