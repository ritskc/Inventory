using DAL.IRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.IServices;

namespace WebApi.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync(int companyId)
        {
            return await this._customerRepository.GetAllCustomerAsync(companyId);
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            return await this._customerRepository.GetCustomerAsync(id);

        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await this._customerRepository.AddCustomerAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            await this._customerRepository.UpdateCustomerAsync(customer);
        }

        public async Task DeleteCustomerAsync(long id)
        {
            await Task.Run(() => this._customerRepository.DeleteCustomerAsync(id));
        }
    }
}
