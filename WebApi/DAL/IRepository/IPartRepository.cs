using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IPartRepository
    {
        Task<IEnumerable<Part>> GetAllPartsAsync(int companyId);
        Task<Part> GetPartAsync(long partId);
        Task AddPartAsync(Part part);
        Task UpdatePartAsync(Part part);
        Task<int> DeletePartAsync(long id);
    }
}
