using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomerAsync(int companyId,int userId);
        Task<Customer> GetCustomerAsync(int id);
        Task<Customer> GetCustomerAsync(int id, SqlConnection conn, SqlTransaction transaction);
        Customer GetCustomer(int id);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task<int> DeleteCustomerAsync(long id);
    }
}
