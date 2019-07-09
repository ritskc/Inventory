using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepository;
using WebApi.IServices;
using DAL.Models;

namespace WebApi.Services
{
    public class CompanyService:ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }


        public async Task<IEnumerable<Company>> GetAllCompanyAsync()
        {
            return await this._companyRepository.GetAllCompanyAsync();
        }

        public async Task<Company> GetCompanyAsync(long id)
        {
            return await Task.Run(() => GetAllCompanyAsync().Result.Where(p => p.Id == id).FirstOrDefault());
        }

        public async Task<Company> GetCompanyByNameAsync(string name)
        {
            return await Task.Run(() => GetAllCompanyAsync().Result.Where(p => p.Name == name).FirstOrDefault());
        }

        public async Task AddCompanyAsync(Company company)
        {
            await this._companyRepository.AddCompanyAsync(company);
        }

        public async Task UpdateCompanyAsync(Company company)
        {
            await this._companyRepository.UpdateCompanyAsync(company);
        }

        public async Task DeleteCompanyAsync(long id)
        {
            await Task.Run(() => this._companyRepository.DeleteCompanyAsync(id));
        }
    }
}
