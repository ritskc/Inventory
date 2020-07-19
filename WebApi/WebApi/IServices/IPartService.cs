using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IPartService
    {
        Task<IEnumerable<Part>> GetAllPartsAsync(int companyId,int userId);
        Task<Part> GetPartAsync(long id);
        Task<IEnumerable<Part>> GetPartBySupplierIdAsync(int supplierId);
        Task<IEnumerable<Part>> GetPartByCustomerIdAsync(int customerId);
        Task AddPartAsync(Part part);
        Task SetStockPriceAsync(List<StockPrice> stockPrices);
        Task SetStockPriceAsync(StockPrice stockPrice, int companyId);
        Task UpdatePartAsync(Part part);
        Task DeletePartAsync(long id);
        Task UpdatePartCustomerPriceAsync(int companyId, string customer, string partcode, decimal price);
        Task UpdatePartSupplierPriceAsync(int companyId, string supplier, string partcode, decimal price);
        Task UpdateOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty);
        Task UpdateOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty);
        Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int qtyInHand);
        Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note);
        Task UpdateMonthlyQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note);

        Task UpdateMonthlyOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty);       
        Task UpdateMonthlyOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty);


        Task<IEnumerable<PartInTransit>> GetPartInTransitDetailAsync(long partId, int companyId);
        Task<IEnumerable<PartOpenOrder>> GetPartOpenOrderDetailAsync(long partId, int companyId);
        Task<IEnumerable<PartLatestShipment>> GetPartLatestShipmentAsync(long partId, int companyId);
        Task<IEnumerable<PartInTransit>> GetPartLatestReceivedAsync(long partId, int companyId);
        Task<IEnumerable<SupplierOpenPO>> GetPartOpenPODetailAsync(long partId, int companyId);
        Task<IEnumerable<StockPrice>> GetStock(long partId, int companyId);
        Task<IEnumerable<StockPrice>> GetAllPartsStocksAsync(int companyId);
    }
}
