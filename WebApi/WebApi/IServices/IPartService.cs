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
        Task<Part> GetPartAsync(long id);
        Task AddPartAsync(Part part);
        Task UpdatePartAsync(Part part);
        Task DeletePartAsync(long id);
        Task UpdatePartCustomerPriceAsync(string customer, string partcode, decimal price);
        Task UpdatePartSupplierPriceAsync(string supplier, string partcode, decimal price);
    }
}
