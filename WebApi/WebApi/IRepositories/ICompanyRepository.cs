using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.IRepositories
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompanyAsync();
        Task<int> AddCompanyAsync(Company company);
        Task<int> UpdateCompanyAsync(Company company);
        Task<int> DeleteCompanyAsync(long id);
    }
}
