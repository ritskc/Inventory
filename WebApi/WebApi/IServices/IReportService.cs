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
    }
}
