using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IPartService
    {
        Task<IEnumerable<Part>> GetAllPartsAsync(int companyId);
        Task<Part> GetPartAsync(int companyId,long id);
        Task AddPartAsync(Part part);
        Task UpdatePartAsync(Part part);
        Task DeletePartAsync(long id);
    }
}
