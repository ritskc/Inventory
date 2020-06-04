using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.Models;
using DAL.IRepository;
using DAL.Repository;
using DAL.DBHelper;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Net;
using Microsoft.Reporting.WebForms.Internal.Soap.ReportingServices2005.Execution;

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
            IPriviledgeRepository priviledgeRepository = new PriviledgeRepository(sqlHelper);
            IUserRepository userRepository = new UserRepository(priviledgeRepository);
            IOrderRepository oRepository = new OrderRepository(sqlHelper, userRepository);
            IPartRepository partRepository = new PartRepository(sqlHelper, userRepository);
            IEntityTrackerRepository entityTrackerRepository = new EntityTrackerRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper, userRepository);
            ITransactionRepository transactionRepository = new TransactionRepository();
            IUserActivityReportRepository userActivityRepository = new UserActivityReportRepository(sqlHelper);
            IPoRepository poRepository = new PoRepository(sqlHelper, entityTrackerRepository, userRepository);
            ISupplierInvoiceRepository supplierInvoiceRepository = new SupplierInvoiceRepository(sqlHelper, poRepository, transactionRepository, userRepository);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository, partRepository, entityTrackerRepository, customerRepository, userRepository,supplierInvoiceRepository, userActivityRepository);

            return await pRepository.GetPackingSlipAsync(id);
        }

        public static List<PackingSlip> GetPackingSlip(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            IPriviledgeRepository priviledgeRepository = new PriviledgeRepository(sqlHelper);
            IUserRepository userRepository = new UserRepository(priviledgeRepository);
            IOrderRepository oRepository = new OrderRepository(sqlHelper, userRepository);
            IPartRepository partRepository = new PartRepository(sqlHelper, userRepository);
            IEntityTrackerRepository entityTrackerRepository = new EntityTrackerRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper, userRepository);
            ITransactionRepository transactionRepository = new TransactionRepository();
            IPoRepository poRepository = new PoRepository(sqlHelper, entityTrackerRepository, userRepository);
            IUserActivityReportRepository userActivityRepository = new UserActivityReportRepository(sqlHelper);
            ISupplierInvoiceRepository supplierInvoiceRepository = new SupplierInvoiceRepository(sqlHelper, poRepository, transactionRepository, userRepository);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository, partRepository, entityTrackerRepository, customerRepository, userRepository,supplierInvoiceRepository, userActivityRepository);

            var result = pRepository.GetPackingSlip(id);

            List<PackingSlip> packingSlips = new List<PackingSlip>();
            packingSlips.Add(result);
            //packingSlips.Add(GetPackingSlipAsync(id).Result);
            return packingSlips.ToList();
        }

        public static List<CustomerShippingInfo> GetShippingInfo(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            PriviledgeRepository priviledgeRepository = new PriviledgeRepository(sqlHelper);
            IUserRepository userRepository = new UserRepository(priviledgeRepository);
            IOrderRepository oRepository = new OrderRepository(sqlHelper, userRepository);
            IPartRepository partRepository = new PartRepository(sqlHelper, userRepository);
            IEntityTrackerRepository entityTrackerRepository = new EntityTrackerRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper, userRepository);
            ITransactionRepository transactionRepository = new TransactionRepository();
            IPoRepository poRepository = new PoRepository(sqlHelper, entityTrackerRepository, userRepository);
            IUserActivityReportRepository userActivityRepository = new UserActivityReportRepository(sqlHelper);
            ISupplierInvoiceRepository supplierInvoiceRepository = new SupplierInvoiceRepository(sqlHelper, poRepository, transactionRepository, userRepository);
            IPackingSlipRepository pRepository = new PackingSlipRepository(sqlHelper, oRepository, partRepository, entityTrackerRepository, customerRepository, userRepository,supplierInvoiceRepository, userActivityRepository);

            var result = pRepository.GetPackingSlip(id);

            List<CustomerShippingInfo> customerShippingInfos = new List<CustomerShippingInfo>();
            customerShippingInfos.Add(result.customerShippingInfo);
            //packingSlips.Add(GetPackingSlipAsync(id).Result);
            return customerShippingInfos.ToList();
        }

        public static List<PackingSlipReport> GetPackingSlipReport(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            PriviledgeRepository priviledgeRepository = new PriviledgeRepository(sqlHelper);
            IUserRepository userRepository = new UserRepository(priviledgeRepository);
            IOrderRepository orderRepository = new OrderRepository(sqlHelper, userRepository);
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper, userRepository);
            ISupplierRepository supplierRepository = new SupplierRepository(sqlHelper, userRepository);
            IPartRepository partRepository = new PartRepository(sqlHelper, userRepository);
            IReportRepository reportRepository = new ReportRepository(sqlHelper,orderRepository,companyRepository,
                customerRepository,partRepository, supplierRepository);

            var result =  reportRepository.GetPackingSlipReport(id);
            return result;
        }

        public static List<PackingSlipReport> GetRepackingInvoiceReport(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            PriviledgeRepository priviledgeRepository = new PriviledgeRepository(sqlHelper);
            IUserRepository userRepository = new UserRepository(priviledgeRepository);
            IOrderRepository orderRepository = new OrderRepository(sqlHelper, userRepository);
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper, userRepository);
            ISupplierRepository supplierRepository = new SupplierRepository(sqlHelper, userRepository);
            IPartRepository partRepository = new PartRepository(sqlHelper, userRepository);
            IReportRepository reportRepository = new ReportRepository(sqlHelper, orderRepository, companyRepository,
                customerRepository, partRepository, supplierRepository);

            var result = reportRepository.GetRepackingInvoiceReport(id);
            return result;
        }

        public static List<PackingSlipReport> GetMasterPackingSlipReport(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            PriviledgeRepository priviledgeRepository = new PriviledgeRepository(sqlHelper);
            IUserRepository userRepository = new UserRepository(priviledgeRepository);
            IOrderRepository orderRepository = new OrderRepository(sqlHelper, userRepository);
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper, userRepository);
            IPartRepository partRepository = new PartRepository(sqlHelper, userRepository);
            ISupplierRepository supplierRepository = new SupplierRepository(sqlHelper, userRepository);
            IReportRepository reportRepository = new ReportRepository(sqlHelper, orderRepository, companyRepository,
                customerRepository, partRepository, supplierRepository);

            var result = reportRepository.GetMasterPackingSlipReport(id);
            return result;
        }

        public static List<POReport> GetPoReport(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            PriviledgeRepository priviledgeRepository = new PriviledgeRepository(sqlHelper);
            IUserRepository userRepository = new UserRepository(priviledgeRepository);
            IOrderRepository orderRepository = new OrderRepository(sqlHelper, userRepository);
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper, userRepository);
            ISupplierRepository supplierRepository = new SupplierRepository(sqlHelper, userRepository);
            IPartRepository partRepository = new PartRepository(sqlHelper, userRepository);
            IReportRepository reportRepository = new ReportRepository(sqlHelper, orderRepository, companyRepository,
                customerRepository, partRepository, supplierRepository);

            var result = reportRepository.GetPoReport(id);
            return result;
        }

        public static List<PackingSlipReport> GetMonthlyInvoiceReport(int id)
        {
            ISqlHelper sqlHelper = new SqlHelper();
            PriviledgeRepository priviledgeRepository = new PriviledgeRepository(sqlHelper);
            IUserRepository userRepository = new UserRepository(priviledgeRepository);
            IOrderRepository orderRepository = new OrderRepository(sqlHelper, userRepository);
            ICompanyRepository companyRepository = new CompanyRepository(sqlHelper);
            ICustomerRepository customerRepository = new CustomerRepository(sqlHelper, userRepository);
            ISupplierRepository supplierRepository = new SupplierRepository(sqlHelper, userRepository);
            IPartRepository partRepository = new PartRepository(sqlHelper, userRepository);
            IReportRepository reportRepository = new ReportRepository(sqlHelper, orderRepository, companyRepository,
                customerRepository, partRepository, supplierRepository);

            var result = reportRepository.GetMonthlyInvoiceReport(id);
            return result;
        }       

    }
}