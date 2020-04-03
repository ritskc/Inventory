using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using WebApi.IServices;

namespace WebApi.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        public ReportService(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository;
        }
        public async Task<IEnumerable<PurchaseData>> GetPurchaseDataAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            return await this.reportRepository.GetPurchaseDataAsync(companyId,fromDate,toDate);
        }

        public async Task<IEnumerable<SalesData>> GetSalesDataAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            return await this.reportRepository.GetSalesDataAsync(companyId, fromDate, toDate);
        }
    }
}
