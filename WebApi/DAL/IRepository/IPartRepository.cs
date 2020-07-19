using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IPartRepository
    {
        Task<IEnumerable<Part>> GetAllPartsAsync(int companyId,int userId);
        Task<Part> GetPartAsync(long partId);
        Task<Part> GetPartAsync(long partId, SqlConnection conn, SqlTransaction transaction);
        Part GetPart(long partId);
        Task<Part> GetPartByNameAsync(int companyId, string name);
        Task<Part> GetPartByMapCodeAsync(int? supplierId, string mapCode);
        Task<IEnumerable<Part>> GetPartBySupplierIdAsync(int supplierId);
        Task<IEnumerable<Part>> GetPartByCustomerIdAsync(int customerId);
        Task AddPartAsync(Part part);
        Task SetStockPriceAsync(List<StockPrice> stockPrice);
        Task SetStockPriceAsync(StockPrice stockPrice, int companyId);
        Task UpdatePartAsync(Part part);
        Task DeletePartAsync(long id);
        Task UpdatePartCustomerPriceAsync(int companyId, string customer, string partcode, decimal price);
        Task UpdatePartSupplierPriceAsync(int companyId, string supplier, string partcode, decimal price);

        Task UpdateOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty);
        Task UpdateMonthlyOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty);
        Task UpdateOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty);
        Task UpdateMonthlyOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty);
        Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand);
        Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note);
        Task UpdateMonthlyQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note);

        Task<IEnumerable<PartTotalShipment>> GetPartTotalShipmentAsync(long partId);
        Task<IEnumerable<PartTotalInvoiceQty>> GetPartTotalInvoiceQtyAsync(long partId);

        Task<IEnumerable<PartInTransit>> GetPartInTransitDetailAsync(long partId, int companyId);
        Task<IEnumerable<PartOpenOrder>> GetPartOpenOrderDetailAsync(long partId, int companyId);
        Task<IEnumerable<PartLatestShipment>> GetPartLatestShipmentAsync(long partId, int companyId);
        Task<IEnumerable<PartInTransit>> GetPartLatestReceivedAsync(long partId, int companyId);
        Task<IEnumerable<SupplierOpenPO>> GetPartOpenPODetailAsync(long partId, int companyId);
        Task<IEnumerable<StockPrice>> GetStock(long partId, int companyId);
        Task<IEnumerable<StockPrice>> GetAllPartsStocksAsync(int companyId);
    }
}
