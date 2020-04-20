using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IReportRepository
    {
        List<PackingSlipReport> GetPackingSlipReport(long id);
        List<PackingSlipReport> GetRepackingInvoiceReport(long id);
        List<PackingSlipReport> GetMasterPackingSlipReport(long id);
        List<POReport> GetPoReport(long poId);
        List<PackingSlipReport> GetMonthlyInvoiceReport(long id);

        Task<IEnumerable<SalesData>> GetSalesDataAsync(int companyId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<PurchaseData>> GetPurchaseDataAsync(int companyId, DateTime fromDate, DateTime toDate);
    }
}
