using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Models;
using DAL.IRepository;
using DAL.Repository;
using DAL.DBHelper;

namespace Report.Services
{
    public class DataSetService
    {        
        public static List<Company> GetCompanies(string contactname)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);
           
            return companyRepository.GetAllCompany().ToList();
                     
        }
    }
}