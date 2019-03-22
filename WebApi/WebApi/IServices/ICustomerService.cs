using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllCustomerAsync(int companyId);
        Task<Customer> GetCustomerAsync(int id);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(long id);
    }
}
