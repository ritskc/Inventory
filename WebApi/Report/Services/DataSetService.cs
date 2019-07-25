using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Models;
using DAL.IRepository;
using DAL.Repository;
using DAL.DBHelper;
using System.Threading.Tasks;

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

        public static async Task<PackingSlip> GetPackingSlipAsync(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            IOrderRepository oRepository = new OrderRepository(sqlHelper);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository);

            return await pRepository.GetPackingSlipAsync(id);
        }

        public static List<PackingSlip> GetPackingSlip(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            IOrderRepository oRepository = new OrderRepository(sqlHelper);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository);

            var result = pRepository.GetPackingSlip(id);

            List<PackingSlip> packingSlips = new List<PackingSlip>();
            packingSlips.Add(result);
            //packingSlips.Add(GetPackingSlipAsync(id).Result);
            return packingSlips.ToList();
        }
    }
}