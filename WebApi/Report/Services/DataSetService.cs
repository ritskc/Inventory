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
            IPartRepository partRepository = new PartRepository(sqlHelper);
            IEntityTrackerRepository entityTrackerRepository = new EntityTrackerRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository, partRepository, entityTrackerRepository, customerRepository);

            return await pRepository.GetPackingSlipAsync(id);
        }

        public static List<PackingSlip> GetPackingSlip(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            IOrderRepository oRepository = new OrderRepository(sqlHelper);
            IPartRepository partRepository = new PartRepository(sqlHelper);
            IEntityTrackerRepository entityTrackerRepository = new EntityTrackerRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository, partRepository, entityTrackerRepository, customerRepository);

            var result = pRepository.GetPackingSlip(id);

            List<PackingSlip> packingSlips = new List<PackingSlip>();
            packingSlips.Add(result);
            //packingSlips.Add(GetPackingSlipAsync(id).Result);
            return packingSlips.ToList();
        }

        public static List<CustomerShippingInfo> GetShippingInfo(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            IOrderRepository oRepository = new OrderRepository(sqlHelper);
            IPartRepository partRepository = new PartRepository(sqlHelper);
            IEntityTrackerRepository entityTrackerRepository = new EntityTrackerRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository, partRepository, entityTrackerRepository, customerRepository);

            var result = pRepository.GetPackingSlip(id);

            List<CustomerShippingInfo> customerShippingInfos = new List<CustomerShippingInfo>();
            customerShippingInfos.Add(result.customerShippingInfo);
            //packingSlips.Add(GetPackingSlipAsync(id).Result);
            return customerShippingInfos.ToList();
        }

        public static List<PackingSlipReport> GetPackingSlipReport(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            IOrderRepository orderRepository = new OrderRepository(sqlHelper);
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper);
            IPartRepository partRepository = new PartRepository(sqlHelper);
            IReportRepository reportRepository = new ReportRepository(sqlHelper,orderRepository,companyRepository,
                customerRepository,partRepository);

            var result =  reportRepository.GetPackingSlipReport(id);
            return result;
        }

        public static List<PackingSlipReport> GetMasterPackingSlipReport(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            IOrderRepository orderRepository = new OrderRepository(sqlHelper);
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper);
            IPartRepository partRepository = new PartRepository(sqlHelper);
            IReportRepository reportRepository = new ReportRepository(sqlHelper, orderRepository, companyRepository,
                customerRepository, partRepository);

            var result = reportRepository.GetMasterPackingSlipReport(id);
            return result;
        }
    }
}