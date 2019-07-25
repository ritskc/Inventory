using DAL.DBHelper;
using DAL.IRepository;
using DAL.Models;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reports.Services
{
    public class DataSetService
    {
        public static async Task<PackingSlip> GetPackingSlipAsync(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            IOrderRepository oRepository = new OrderRepository(sqlHelper);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository);

            return await pRepository.GetPackingSlipAsync(id);
        }

        public static List<PackingSlip> GetPackingSlip(int id)
        {
            List<PackingSlip> packingSlips = new List<PackingSlip>();
            packingSlips.Add(GetPackingSlipAsync(id).Result);
            return packingSlips.ToList();
        }

        public static List<Company> GetCompanies(string contactname)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);

            return companyRepository.GetAllCompany().ToList();

        }

        public List<PackingSlip> GetPackingSlipById(int id)
        {
            List<PackingSlip> packingSlips = new List<PackingSlip>();
            packingSlips.Add(GetPackingSlipAsync(id).Result);
            return packingSlips.ToList();
        }

    }
}
