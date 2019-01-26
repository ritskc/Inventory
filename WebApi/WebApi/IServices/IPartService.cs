using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.IServices
{
    public interface IPartService
    {
        Task<IEnumerable<Part>> GetAllPartsAsync();
        Task<Part> GetPartAsync(long id);
        Task AddPartAsync(Part part);
        Task UpdatePartAsync(Part part);
        Task<Part> DeletePartAsync(long id);
    }
}
