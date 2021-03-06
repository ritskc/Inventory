﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;

namespace WebApi.IServices
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAllCompanyAsync();
        Task<Company> GetCompanyAsync(long id);        
        Task<Company> GetCompanyByNameAsync(string name);
        Task AddCompanyAsync(Company company);
        Task UpdateCompanyAsync(Company company);
        Task DeleteCompanyAsync(long id);
    }
}
