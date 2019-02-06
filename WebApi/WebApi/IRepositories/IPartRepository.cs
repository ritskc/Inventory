using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.IRepositories
{
    public interface IPartRepository
    {
        Task<IEnumerable<Part>> GetAllPartsAsync();
        Task<int> AddPartAsync(Part part);
        Task<int> UpdatePartAsync(Part part);
        Task<int> DeletePartAsync(long id);
    }
}
