using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompanyAsync();
        IEnumerable<Company> GetAllCompany();
        Task<Company> GetCompanyAsync(int id);
        Task<Company> GetCompanyByNameAsync(string name);
        Task<int> AddCompanyAsync(Company company);
        Task<int> UpdateCompanyAsync(Company company);
        Task<int> DeleteCompanyAsync(long id);
    }
}
