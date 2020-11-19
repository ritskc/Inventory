using DAL.IRepository;
using DAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using WebApi.IServices;

namespace WebApi.Services
{
    public class PartService : IPartService
    {

        private readonly IPartRepository _partRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerRepository _customerRepository;

        public PartService(IPartRepository partRepository, ISupplierRepository supplierRepository, ICustomerRepository customerRepository)
        {
            _partRepository = partRepository;
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
        }

       
        public async Task<IEnumerable<Part>> GetAllPartsAsync(int companyId,int userId)
        {
            var suppliers = await _supplierRepository.GetAllSupplierAsync(companyId,userId);
            var customers = await _customerRepository.GetAllCustomerAsync(companyId,userId);

            var parts=  await this._partRepository.GetAllPartsAsync(companyId,userId);
            foreach(Part part in parts)
            {
                foreach(PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
                {
                    partSupplierAssignment.SupplierName = suppliers.Where(x => x.Id == partSupplierAssignment.SupplierID).Select(x => x.Name).FirstOrDefault();
                    part.SupplierPrice = partSupplierAssignment.UnitPrice;
                }
            }

            foreach (Part part in parts)
            {
                foreach (PartCustomerAssignment partCustomerAssignments in part.partCustomerAssignments)
                {
                    partCustomerAssignments.CustomerName = customers.Where(x => x.Id == partCustomerAssignments.CustomerId).Select(x => x.Name).FirstOrDefault();
                    partCustomerAssignments.Invoicingtypeid = customers.Where(x => x.Id == partCustomerAssignments.CustomerId).Select(x => x.Invoicingtypeid).FirstOrDefault();
                    part.CustomerPrice = partCustomerAssignments.Rate;

                    if(partCustomerAssignments.Invoicingtypeid == 3)
                    {

                        var shipmentQtys = await this._partRepository.GetPartTotalShipmentAsync(part.Id);
                        part.ShippedQty = shipmentQtys.Select(x => x.ShippedQty).FirstOrDefault();
                        part.MonthlyExcessQty = shipmentQtys.Select(x => x.MonthlyExcessQty).FirstOrDefault();

                        var invoiceQtys = await this._partRepository.GetPartTotalInvoiceQtyAsync(part.Id);
                        part.InvoiceQty = invoiceQtys.Select(x => x.InvoiceQty).FirstOrDefault();

                        part.MonthlyClosingQty = part.MonthlyOpeningQty + part.ShippedQty - part.InvoiceQty;
                    }

                }
            }

            return parts;
        }

        public async Task<IEnumerable<PartCompact>> GetAllPartsCompactAsync1(int companyId, int userId)
        {
            var parts = await this._partRepository.GetAllPartsCompactAsync1(companyId, userId);
            return parts;
        }

        public async Task<IEnumerable<Part>> GetAllPartsCompactAsync(int companyId, int userId)
        {
            var parts = await this._partRepository.GetAllPartsCompactAsync(companyId, userId);
            return parts;
        }

        public async Task<IEnumerable<Part>> GetAllPartsByDateAsync(int companyId, int userId,DateTime dateTime)
        {
            var suppliers = await _supplierRepository.GetAllSupplierAsync(companyId, userId);
            var customers = await _customerRepository.GetAllCustomerAsync(companyId, userId);

            var parts = await this._partRepository.GetAllPartsByDateAsync(companyId, userId, dateTime);
            foreach (Part part in parts)
            {
                foreach (PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
                {
                    partSupplierAssignment.SupplierName = suppliers.Where(x => x.Id == partSupplierAssignment.SupplierID).Select(x => x.Name).FirstOrDefault();
                    part.SupplierPrice = partSupplierAssignment.UnitPrice;
                }
            }

            //foreach (Part part in parts)
            //{
            //    foreach (PartCustomerAssignment partCustomerAssignments in part.partCustomerAssignments)
            //    {
            //        partCustomerAssignments.CustomerName = customers.Where(x => x.Id == partCustomerAssignments.CustomerId).Select(x => x.Name).FirstOrDefault();
            //        partCustomerAssignments.Invoicingtypeid = customers.Where(x => x.Id == partCustomerAssignments.CustomerId).Select(x => x.Invoicingtypeid).FirstOrDefault();
            //        part.CustomerPrice = partCustomerAssignments.Rate;

            //        if (partCustomerAssignments.Invoicingtypeid == 3)
            //        {

            //            var shipmentQtys = await this._partRepository.GetPartTotalShipmentAsync(part.Id);
            //            part.ShippedQty = shipmentQtys.Select(x => x.ShippedQty).FirstOrDefault();
            //            part.MonthlyExcessQty = shipmentQtys.Select(x => x.MonthlyExcessQty).FirstOrDefault();

            //            var invoiceQtys = await this._partRepository.GetPartTotalInvoiceQtyAsync(part.Id);
            //            part.InvoiceQty = invoiceQtys.Select(x => x.InvoiceQty).FirstOrDefault();

            //            part.MonthlyClosingQty = part.MonthlyOpeningQty + part.ShippedQty - part.InvoiceQty;
            //        }

            //    }
            //}

            return parts;
        }

        public async Task<IEnumerable<Part>> GetPartBySupplierIdAsync(int supplierId)
        {
            return await this._partRepository.GetPartBySupplierIdAsync(supplierId);
        }

        public async Task<IEnumerable<Part>> GetAllPartsbyWarehouseAsync(int companyId, int userId, int warehouseId)
        {
            return await this._partRepository.GetAllPartsbyWarehouseAsync(companyId, userId, warehouseId);
        }

        public async Task<IEnumerable<Part>> GetPartByCustomerIdAsync(int customerId)
        {
            return await this._partRepository.GetPartByCustomerIdAsync(customerId);
        }


        public async Task<Part> GetPartAsync(long id)
        {
            return await this._partRepository.GetPartAsync(id);
        }       

        public async Task AddPartAsync(Part part)
        {            
            await this._partRepository.AddPartAsync(part);
        }

        public async Task SetStockPriceAsync(int partId,List<StockPrice> stockPrices)
        {
            await this._partRepository.SetStockPriceAsync(partId,stockPrices);
        }

        public async Task SetStockPriceAsync(List<StockPrice> stockPrices,int companyId)
        {
            await this._partRepository.SetStockPriceAsync(stockPrices, companyId);
        }

        public async Task UpdatePartAsync(Part part)
        {
            await this._partRepository.UpdatePartAsync(part);
        }       
        
        public async Task DeletePartAsync(long id)
        {            
            await Task.Run(() => this._partRepository.DeletePartAsync(id));
        }

        public async Task UpdatePartCustomerPriceAsync(int companyId, string customer, string partcode, decimal price)
        {
            await Task.Run(() => this._partRepository.UpdatePartCustomerPriceAsync(companyId,customer, partcode,price));
        }

        public async Task UpdatePartSupplierPriceAsync(int companyId, string supplier, string partcode, decimal price)
        {
            await Task.Run(() => this._partRepository.UpdatePartSupplierPriceAsync(companyId,supplier, partcode, price));
        }

        public async Task UpdateOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty)
        {
            await Task.Run(() => this._partRepository.UpdateOpeningQtyByPartCodeAsync(companyId,  partcode, openingQty));
        }

        public async Task UpdateOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty)
        {
            await Task.Run(() => this._partRepository.UpdateOpeningQtyByPartIdAsync(companyId, partId, openingQty));
        }

        public async Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand)
        {
            await Task.Run(() => this._partRepository.UpdateQtyInHandByPartIdAsync(companyId, partId, QtyInHand));
        }

        public async Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note)
        {
            await Task.Run(() => this._partRepository.UpdateQtyInHandByPartIdAsync(companyId, partId, QtyInHand,direction,note));
        }

        public async Task UpdateMonthlyQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note)
        {
            await Task.Run(() => this._partRepository.UpdateMonthlyQtyInHandByPartIdAsync(companyId, partId, QtyInHand, direction, note));
        }

        public async Task<IEnumerable<PartInTransit>> GetPartInTransitDetailAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartInTransitDetailAsync(partId, companyId));
        }

        public async Task<IEnumerable<PartOpenOrder>> GetPartOpenOrderDetailAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartOpenOrderDetailAsync(partId, companyId));
        }

        public async Task<IEnumerable<SupplierOpenPO>> GetPartOpenPODetailAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartOpenPODetailAsync(partId, companyId));
        }

        public async Task<IEnumerable<PartLatestShipment>> GetPartLatestShipmentAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartLatestShipmentAsync(partId, companyId));
        }

        public async Task<IEnumerable<PartInTransit>> GetPartLatestReceivedAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartLatestReceivedAsync(partId, companyId));
        }

        public async Task<IEnumerable<WarehouseInventory>> GetPartWarehouseInventoryAsync(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetPartWarehouseInventoryAsync(partId, companyId));
        }


        public async Task<IEnumerable<StockPrice>> GetStock(long partId, int companyId)
        {
            return await Task.Run(() => this._partRepository.GetStock(partId, companyId));
        }

        public async Task<IEnumerable<StockPrice>> GetAllPartsStocksAsync(int companyId)
        {
            return await Task.Run(() => this._partRepository.GetAllPartsStocksAsync(companyId));
        }

        public async Task UpdateMonthlyOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty)
        {
            await Task.Run(() => this._partRepository.UpdateMonthlyOpeningQtyByPartCodeAsync(companyId, partcode, openingQty));
        }

        public async Task UpdateMonthlyOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty)
        {
            await Task.Run(() => this._partRepository.UpdateMonthlyOpeningQtyByPartIdAsync(companyId, partId, openingQty));
        }

        public async Task TransferInventoryInternallyAsync(PartTransfer partTransfer)
        {
            await Task.Run(() => this._partRepository.TransferInventoryInternallyAsync(partTransfer));
        }
    }
}
