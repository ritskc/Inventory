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

        public PartService(IPartRepository partRepository)
        {
            _partRepository = partRepository;
        }

       
        public async Task<IEnumerable<Part>> GetAllPartsAsync(int companyId)
        {
            return await this._partRepository.GetAllPartsAsync(companyId);
        }  
        
        public async Task<Part> GetPartAsync(int companyId,long id)
        {
            return await Task.Run(() => GetAllPartsAsync(companyId).Result.Where(p => p.Id == id).FirstOrDefault());           
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
          
    }
}
