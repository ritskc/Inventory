using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IReportService
    {
        Task<IEnumerable<SalesData>> GetSalesDataAsync(int companyId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<PurchaseData>> GetPurchaseDataAsync(int companyId, DateTime fromDate, DateTime toDate);

        Task<IEnumerable<SalesData>> GetSalesDataSummaryAsync(int companyId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<PurchaseData>> GetPurchaseDataSummaryAsync(int companyId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<TransactionDetail>> GetInventoryAdjustmentReport(int companyId, DateTime from, DateTime to);
    }
}
