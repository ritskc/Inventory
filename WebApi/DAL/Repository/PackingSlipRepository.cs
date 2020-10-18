using DAL.DBHelper;
using DAL.IRepository;
using DAL.Models;
using DAL.Settings;
using DAL.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class PackingSlipRepository : IPackingSlipRepository
    {
        private readonly ISqlHelper _sqlHelper;
        private readonly IOrderRepository _orderRepository;
        private readonly IPartRepository partRepository;
        private readonly IEntityTrackerRepository entityTrackerRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IUserRepository userRepository;
        private readonly ISupplierInvoiceRepository supplierInvoiceRepository;
        private readonly IUserActivityReportRepository userActivityReportRepository;

        public PackingSlipRepository(ISqlHelper sqlHelper,
            IOrderRepository orderRepository,
            IPartRepository partRepository,
            IEntityTrackerRepository entityTrackerRepository,
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            ISupplierInvoiceRepository supplierInvoiceRepository,
            IUserActivityReportRepository userActivityReportRepository)
        {
            this._sqlHelper = sqlHelper;
            this._orderRepository = orderRepository;
            this.partRepository = partRepository;
            this.entityTrackerRepository = entityTrackerRepository;
            this.customerRepository = customerRepository;
            this.userRepository = userRepository;
            this.supplierInvoiceRepository = supplierInvoiceRepository;
            this.userActivityReportRepository = userActivityReportRepository;
        }
        public async Task<Int32> AddPackingSlipAsync(PackingSlip packingSlip)
        {
            Int32 packingSlipId = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    packingSlip.SubTotal = 0;
                    packingSlip.Total = 0;
                    packingSlip.TotalSurcharge = 0;
                    packingSlip.GrossWeight = 0;
                    packingSlip.Boxes = 0;
                    var customer = await this.customerRepository.GetCustomerAsync(packingSlip.CustomerId, connection, transaction);
                    packingSlip.FOB = customer.FOB;
                    packingSlip.Terms = customer.Terms;
                    packingSlip.IsRepackage = false;
                    if (packingSlip.ShipmentInfoId == 0)
                    {
                        packingSlip.ShipmentInfoId = customer.ShippingInfos.Where(x => x.IsDefault).FirstOrDefault().Id;
                    }
                    if (customer.Invoicingtypeid == 3)
                        packingSlip.IsMonthly = true;
                    else
                        packingSlip.IsMonthly = false;


                    foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                    {
                        var partDetail = await this.partRepository.GetPartAsync(packingSlipDetail.PartId, connection, transaction);

                        packingSlipDetail.DefaultWarehouse = partDetail.DefaultWarehouse;
                        packingSlipDetail.WarehouseId = partDetail.WarehouseId;

                        decimal orderPartPrice = 0 ;
                        packingSlipDetail.IsRepackage = partDetail.IsRepackage;
                        if (packingSlipDetail.IsRepackage)
                            packingSlip.IsRepackage = true;

                        if (packingSlip.IsInvoiceCreated)
                            packingSlipDetail.UnitPrice = partDetail.partCustomerAssignments.Where(x => x.CustomerId == packingSlip.CustomerId).Select(x => x.Rate).FirstOrDefault();
                        else
                            packingSlip.IsInvoiceCreated = false;

                        if (packingSlip.IsInvoiceCreated && !packingSlipDetail.IsBlankOrder)
                        {
                            var orderResult = await _orderRepository.GetOrderMasterAsync(packingSlipDetail.OrderId, command.Connection, command.Transaction);

                            if (orderResult != null && !orderResult.IsClosed)
                            {
                                var orderDetail = orderResult.OrderDetails.Where(x => x.Id == packingSlipDetail.OrderDetailId).FirstOrDefault();
                                packingSlipDetail.OrderNo = orderResult.PONo;

                                if (orderDetail != null && !orderDetail.IsClosed)
                                {
                                    packingSlipDetail.LineNumber = orderDetail.LineNumber;
                                    packingSlipDetail.UnitPrice = orderDetail.UnitPrice;
                                    orderPartPrice = orderDetail.UnitPrice;
                                }
                            }

                            var stocks = GetStock(packingSlipDetail.PartId, command.Connection, command.Transaction);

                            int stockAdjusment = packingSlipDetail.Qty;
                            decimal customerPrice = 0;
                            foreach (StockPrice stockPrice in stocks)
                            {
                                if (stockPrice.Qty >= stockAdjusment)
                                {
                                    var sql1 = string.Format($"UPDATE [StockPrice] SET [Qty] = [Qty] - '{stockAdjusment}' WHERE id = '{stockPrice.Id}' ");
                                    command.CommandText = sql1;
                                    await command.ExecuteNonQueryAsync();

                                    if(customerPrice > stockPrice.CustomerPrice)
                                        packingSlipDetail.UnitPrice = customerPrice;
                                    else
                                        packingSlipDetail.UnitPrice = stockPrice.CustomerPrice;
                                    break;
                                }
                                else
                                {
                                    var sql1 = string.Format($"UPDATE [StockPrice] SET [Qty] = 0 WHERE id = '{stockPrice.Id}' ");
                                    command.CommandText = sql1;
                                    await command.ExecuteNonQueryAsync();

                                    stockAdjusment = stockAdjusment - stockPrice.Qty;
                                    packingSlipDetail.UnitPrice = stockPrice.CustomerPrice;
                                }
                            }

                            if (packingSlipDetail.UnitPrice == 0)
                                packingSlipDetail.UnitPrice = orderPartPrice;
                        }
                        else
                            packingSlip.IsInvoiceCreated = false;

                        packingSlipDetail.Price = packingSlipDetail.Qty * packingSlipDetail.UnitPrice;
                        packingSlip.SubTotal = packingSlip.SubTotal + packingSlipDetail.Price;

                        packingSlipDetail.Surcharge = partDetail.partCustomerAssignments.Where(x => x.CustomerId == packingSlip.CustomerId).Select(x => x.SurchargePerPound).FirstOrDefault();
                        packingSlipDetail.SurchargePerPound = packingSlipDetail.Surcharge;
                        packingSlipDetail.SurchargePerUnit = packingSlipDetail.Surcharge * partDetail.WeightInLb;
                        packingSlipDetail.TotalSurcharge = packingSlipDetail.SurchargePerUnit * packingSlipDetail.Qty;
                        packingSlip.TotalSurcharge = packingSlip.TotalSurcharge + packingSlipDetail.TotalSurcharge;
                        packingSlip.GrossWeight = packingSlip.GrossWeight + (packingSlipDetail.Qty * partDetail.WeightInLb);
                        packingSlip.Boxes = packingSlip.Boxes + packingSlipDetail.Boxes;
                        packingSlipDetail.LineNumber = "";
                    }
                    packingSlip.Total = packingSlip.SubTotal + packingSlip.TotalSurcharge + packingSlip.ShippingCharge + packingSlip.CustomCharge;
                    string sql = string.Format($"INSERT INTO [dbo].[PackingSlipMaster]   ([CompanyId]   ,[CustomerId]   ,[PackingSlipNo]   ,[ShippingDate]   ,[ShipVia]   ,[Crates]   ,[Boxes]   ,[GrossWeight]   ,[ShippingCharge]   ,[CustomCharge]   ,[SubTotal]   ,[Total]   ,[IsInvoiceCreated]   ,[IsPaymentReceived]   ,[FOB]   ,[Terms]   ,[ShipmentInfoId]   ,[InvoiceDate],[IsPOSUploaded],[POSPath],[TotalSurcharge],[IsRepackage],[IsMonthly],[IsShipmentVerified],[IsScanned],[AllowScanning]) VALUES   ('{packingSlip.CompanyId}'   ,'{packingSlip.CustomerId}'   ,'{packingSlip.PackingSlipNo}'   ,'{packingSlip.ShippingDate}'   ,'{packingSlip.ShipVia}'   ,'{packingSlip.Crates}'   ,'{packingSlip.Boxes}'   ,'{packingSlip.GrossWeight}'   ,'{packingSlip.ShippingCharge}'   ,'{packingSlip.CustomCharge}'   ,'{packingSlip.SubTotal}'   ,'{packingSlip.Total + packingSlip.TotalSurcharge}'   ,'{packingSlip.IsInvoiceCreated}'   ,'{packingSlip.IsPaymentReceived}'   ,'{packingSlip.FOB}'   ,'{packingSlip.Terms}'   ,'{packingSlip.ShipmentInfoId}'   ,'{null}','{false}','{string.Empty}','{packingSlip.TotalSurcharge}','{packingSlip.IsRepackage}','{packingSlip.IsMonthly}','{false}','{false}','{false}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;

                    packingSlipId = Convert.ToInt32(command.ExecuteScalar());
                    packingSlip.Id = Convert.ToInt32(packingSlipId);


                    foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                    {
                        packingSlipDetail.ExcessQty = 0;
                        if (!packingSlipDetail.IsBlankOrder)
                        {
                            var orderResult = await _orderRepository.GetOrderMasterAsync(packingSlipDetail.OrderId, command.Connection, command.Transaction);

                            if (orderResult != null && !orderResult.IsClosed)
                            {
                                var orderDetail = orderResult.OrderDetails.Where(x => x.Id == packingSlipDetail.OrderDetailId).FirstOrDefault();
                                packingSlipDetail.OrderNo = orderResult.PONo;

                                if (orderDetail != null && !orderDetail.IsClosed)
                                {
                                    packingSlipDetail.LineNumber = orderDetail.LineNumber;
                                    int availableshippedQty = orderDetail.Qty - orderDetail.ShippedQty;
                                    if (packingSlipDetail.Qty < availableshippedQty)
                                    {
                                        sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET [ShippedQty] = '{orderDetail.ShippedQty + packingSlipDetail.Qty}'  WHERE id = '{orderDetail.Id}' ");
                                        command.CommandText = sql;
                                        await command.ExecuteNonQueryAsync();
                                    }
                                    else
                                    {
                                        packingSlipDetail.ExcessQty = packingSlipDetail.Qty - availableshippedQty;
                                        sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET [ShippedQty] = '{orderDetail.ShippedQty + packingSlipDetail.Qty}',[IsClosed] = '{true}',[ClosingDate] = '{DateTime.Now}'   WHERE id = '{orderDetail.Id}' ");
                                        command.CommandText = sql;
                                        await command.ExecuteNonQueryAsync();

                                        orderResult = await _orderRepository.GetOrderMasterAsync(packingSlipDetail.OrderId, command.Connection, command.Transaction);
                                        var openPO = orderResult.OrderDetails.Where(x => x.OrderId == packingSlipDetail.OrderId && x.IsClosed == false).FirstOrDefault();

                                        if (openPO == null)
                                        {
                                            sql = string.Format($"UPDATE [dbo].[OrderMaster]   SET [IsClosed] = '{true}' ,[ClosingDate] = '{DateTime.Now}'  WHERE id = '{packingSlipDetail.OrderId}' ");
                                            command.CommandText = sql;
                                            await command.ExecuteNonQueryAsync();
                                            //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                                        }
                                    }
                                }
                            }
                            if (customer.Invoicingtypeid == 3 && packingSlipDetail.SupplierInvoiceId > 0)
                            {
                                if (packingSlipDetail.SupplierInvoiceOpenQty == packingSlipDetail.Qty)
                                {
                                    sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [AdjustedInvoiceQty] =  [Qty]   WHERE [InvoiceId] = '{packingSlipDetail.SupplierInvoiceId}' ");
                                    command.CommandText = sql;
                                    await command.ExecuteNonQueryAsync();
                                }
                                else
                                {
                                    var supOpenInvoice = await supplierInvoiceRepository.GetSupplierInvoiceAsync(packingSlipDetail.SupplierInvoiceId, connection, transaction);
                                    var openQty = 0;
                                    var remainingQty = packingSlipDetail.Qty;
                                    foreach (SupplierInvoiceDetail supplierInvoiceDetail in supOpenInvoice.supplierInvoiceDetails)
                                    {
                                        if (remainingQty > 0)
                                        {
                                            openQty = supplierInvoiceDetail.Qty - supplierInvoiceDetail.AdjustedInvoiceQty;
                                            if (openQty >= remainingQty)
                                            {
                                                sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [AdjustedInvoiceQty] = [AdjustedInvoiceQty] +  '{openQty}'   WHERE [id] = '{supplierInvoiceDetail.Id}' ");
                                                command.CommandText = sql;
                                                await command.ExecuteNonQueryAsync();
                                            }
                                            else
                                            {
                                                sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [AdjustedInvoiceQty] = [Qty]  WHERE [id] = '{supplierInvoiceDetail.Id}' ");
                                                command.CommandText = sql;
                                                await command.ExecuteNonQueryAsync();
                                            }
                                        }
                                        remainingQty = remainingQty - openQty;
                                    }
                                }
                            }
                        }

                        //var partDetail = await this.partRepository.GetPartAsync(packingSlipDetail.PartId);
                        //packingSlipDetail.SurchargePerUnit = partDetail.partCustomerAssignments.Where(x => x.CustomerId == packingSlip.CustomerId).Select(x => x.SurchargePerPound).FirstOrDefault();
                        //packingSlipDetail.TotalSurcharge = packingSlipDetail.SurchargePerUnit * packingSlipDetail.Qty;
                        sql = string.Format($"INSERT INTO [dbo].[PackingSlipDetails]   ([PackingSlipId]   ,[IsBlankOrder]   ,[OrderNo]   ,[OrderId]   ,[OrderDetailId]   ,[PartId]   ,[Qty]   ,[Boxes]   ,[InBasket]   ,[UnitPrice]   ,[Price]   ,[Surcharge]   ,[SurchargePerPound]   ,[SurchargePerUnit]   ,[TotalSurcharge]   ,[ExcessQty],[SrNo],[LineNumber],[IsRepackage],[SupplierInvoiceId]) VALUES   ('{packingSlipId}'   ,'{packingSlipDetail.IsBlankOrder}'   ,'{packingSlipDetail.OrderNo}'   ,'{packingSlipDetail.OrderId}'   ,'{packingSlipDetail.OrderDetailId}'   ,'{packingSlipDetail.PartId}'   ,'{packingSlipDetail.Qty}'   ,'{packingSlipDetail.Boxes}'   ,'{packingSlipDetail.InBasket}'   ,'{packingSlipDetail.UnitPrice}'   ,'{packingSlipDetail.Price}'   ,'{packingSlipDetail.Surcharge}'   ,'{packingSlipDetail.SurchargePerPound}'   ,'{packingSlipDetail.SurchargePerUnit}'   ,'{packingSlipDetail.TotalSurcharge}'   ,'{packingSlipDetail.ExcessQty}','{packingSlipDetail.SrNo}','{packingSlipDetail.LineNumber}','{packingSlipDetail.IsRepackage}','{packingSlipDetail.SupplierInvoiceId}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        if (packingSlip.IsInvoiceCreated)
                            sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand - '{packingSlipDetail.Qty}', [CurrentPricingInEffectQty] = CurrentPricingInEffectQty - '{packingSlipDetail.Qty}' WHERE id = '{packingSlipDetail.PartId}' ");
                        else
                            sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand - '{packingSlipDetail.Qty}'  WHERE id = '{packingSlipDetail.PartId}' ");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        if (!packingSlipDetail.DefaultWarehouse)
                        {
                            sql = string.Format($"UPDATE [dbo].[PartWarehouseInventory]   SET [QtyInHand] =  QtyInHand - '{packingSlipDetail.Qty}' WHERE PartId = '{packingSlipDetail.PartId}' and CompanyId = '{packingSlip.CompanyId}' and WarehouseId = '{packingSlipDetail.WarehouseId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }

                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{packingSlipDetail.PartId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.CUSTOMER_PACKINGSLIP)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.OUT)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND)}'   ,'{packingSlip.Id.ToString()}'   ,'{packingSlipDetail.Qty}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        //sql = sql + " Select Scope_Identity()";
                        //command.CommandText = sql;
                        //packingSlipDetail.Id = Convert.ToInt32(await _sqlHelper.ExecuteScalarAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text));

                    }
                    if (packingSlip.IsDeletedPackingSlipNoUsed)
                    {
                        sql = string.Format($"DELETE FROM [dbo].[DeletedPackingSlip] WHERE PackingSlipNo = '{packingSlip.PackingSlipNo}' and CompanyId = '{packingSlip.CompanyId}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                    }
                    else
                        await entityTrackerRepository.AddEntityAsync(packingSlip.CompanyId, packingSlip.ShippingDate, BusinessConstants.ENTITY_TRACKER_PACKING_SLIP, command.Connection, command.Transaction, command);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
            return packingSlipId;
        }

        private IEnumerable<StockPrice> GetStock(int partId, SqlConnection conn, SqlTransaction transaction)
        {
            var stockPrices = new List<StockPrice>();
            var commandText = string.Format($"SELECT id,PartId,Qty,CustomerPrice FROM StockPrice WHERE PartId = '{partId}' and Qty > 0 order by id ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = cmd.ExecuteReader(CommandBehavior.Default);

                while (dataReader.Read())
                {
                    var stockPrice = new StockPrice();
                    stockPrice.Id = Convert.ToInt32(dataReader["Id"]);
                    stockPrice.PartId = Convert.ToInt32(dataReader["PartId"]);
                    stockPrice.Qty = Convert.ToInt32(dataReader["Qty"]);
                    stockPrice.CustomerPrice = Convert.ToDecimal(dataReader["CustomerPrice"]);

                    stockPrices.Add(stockPrice);
                }
                dataReader.Close();
            }

            return stockPrices.OrderBy(x => x.Id);
        }

        public async Task CreateInvoiceAsync(PackingSlip packingSlip)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    string sql = "";
                    packingSlip.SubTotal = 0;
                    packingSlip.Total = 0;
                    foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                    {
                        //
                        var orderPartPrice = packingSlipDetail.UnitPrice;
                        var stocks = GetStock(packingSlipDetail.PartId, command.Connection, command.Transaction);

                        int stockAdjusment = packingSlipDetail.Qty;
                        decimal customerPrice = 0;
                        foreach (StockPrice stockPrice in stocks)
                        {
                            if (stockPrice.Qty >= stockAdjusment)
                            {
                                var sql1 = string.Format($"UPDATE [StockPrice] SET [Qty] = [Qty] - '{stockAdjusment}' WHERE id = '{stockPrice.Id}' ");
                                command.CommandText = sql1;
                                await command.ExecuteNonQueryAsync();

                                if(customerPrice > stockPrice.CustomerPrice)
                                    packingSlipDetail.UnitPrice = customerPrice;
                                else
                                    packingSlipDetail.UnitPrice = stockPrice.CustomerPrice;
                                break;
                            }
                            else
                            {
                                var sql1 = string.Format($"UPDATE [StockPrice] SET [Qty] = 0 WHERE id = '{stockPrice.Id}' ");
                                command.CommandText = sql1;
                                await command.ExecuteNonQueryAsync();

                                stockAdjusment = stockAdjusment - stockPrice.Qty;
                                packingSlipDetail.UnitPrice = stockPrice.CustomerPrice;
                            }
                        }
                        if (packingSlipDetail.UnitPrice == 0)
                            packingSlipDetail.UnitPrice = orderPartPrice;

                        packingSlipDetail.Price = packingSlipDetail.UnitPrice * packingSlipDetail.Qty;
                        packingSlip.SubTotal = packingSlip.SubTotal + packingSlipDetail.Price;
                        //packingSlip.TotalSurcharge = packingSlip.TotalSurcharge + (packingSlipDetail.SurchargePerUnit * packingSlipDetail.Qty);
                        sql = string.Format($"UPDATE [dbo].[PackingSlipDetails]   SET [UnitPrice] = '{packingSlipDetail.UnitPrice}' ,[Price] = '{packingSlipDetail.Price}' ,[Surcharge] = '{packingSlipDetail.Surcharge}' ,[SurchargePerPound] = '{packingSlipDetail.SurchargePerPound}' ,[SurchargePerUnit] = '{packingSlipDetail.SurchargePerUnit}' ,[TotalSurcharge] = '{packingSlipDetail.TotalSurcharge}' WHERE Id = '{packingSlipDetail.Id}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        sql = string.Format($"UPDATE [dbo].[PART]   SET [CurrentPricingInEffectQty] = CurrentPricingInEffectQty - '{packingSlipDetail.Qty}'  WHERE Id = '{packingSlipDetail.PartId}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //
                    }

                    packingSlip.Total = packingSlip.SubTotal + packingSlip.ShippingCharge + packingSlip.CustomCharge;
                    sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [ShippingCharge] = '{packingSlip.ShippingCharge}' ,[CustomCharge] = '{packingSlip.CustomCharge}' ,[SubTotal] = '{packingSlip.SubTotal}' ,[Total] = '{packingSlip.Total}' ,[TotalSurcharge] = '{packingSlip.TotalSurcharge}',[IsInvoiceCreated] = '{true}' ,[InvoiceDate] = '{DateTime.Now}' WHERE Id = '{packingSlip.Id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();



                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<bool> DeletePackingSlipAsync(long id)
        {
            var packingslip = await GetPackingSlipAsync(id);

            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

                //1. Get SupplierInvoicePoDetails po transaction detail
                List<SupplierInvoicePoDetails> supplierInvoicePoDetailsList = new List<SupplierInvoicePoDetails>();
                SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;
                string sql = string.Empty;
                try
                {
                    foreach (PackingSlipDetails packingSlipDetail in packingslip.PackingSlipDetails)
                    {
                        var partDetail = await this.partRepository.GetPartAsync(packingSlipDetail.PartId, connection, transaction);

                        packingSlipDetail.DefaultWarehouse = partDetail.DefaultWarehouse;
                        packingSlipDetail.WarehouseId = partDetail.WarehouseId;

                        if (packingSlipDetail.OrderDetailId != null && packingSlipDetail.OrderDetailId > 0)
                        {
                            var reflectedQty = packingSlipDetail.Qty - packingSlipDetail.ExcessQty;
                            sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET [ShippedQty] = [ShippedQty] - '{reflectedQty}',[IsClosed] = '{false}',[ClosingDate] = NULL   WHERE id = '{packingSlipDetail.OrderDetailId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();

                            sql = string.Format($"UPDATE [dbo].[OrderMaster]   SET [IsClosed] = '{false}',[ClosingDate] = NULL   WHERE id = '{packingSlipDetail.OrderId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }

                        if (packingslip.IsInvoiceCreated)
                        {
                            sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand + '{packingSlipDetail.Qty}', [CurrentPricingInEffectQty] = CurrentPricingInEffectQty + '{packingSlipDetail.Qty}' WHERE id = '{packingSlipDetail.PartId}' ");

                            var stocks = GetStock(packingSlipDetail.PartId, command.Connection, command.Transaction);
                            
                            foreach (StockPrice stockPrice in stocks)
                            {
                                var sql1 = string.Format($"UPDATE [StockPrice] SET [Qty] = [Qty] + '{packingSlipDetail.Qty}' WHERE id = '{stockPrice.Id}' ");
                                command.CommandText = sql1;
                                await command.ExecuteNonQueryAsync();                                
                                break;
                            }                            
                        }
                        else
                            sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand + '{packingSlipDetail.Qty}'  WHERE id = '{packingSlipDetail.PartId}' ");


                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        if (!packingSlipDetail.DefaultWarehouse)
                        {
                            sql = string.Format($"UPDATE [dbo].[PartWarehouseInventory]   SET [QtyInHand] =  QtyInHand + '{packingSlipDetail.Qty}' WHERE PartId = '{packingSlipDetail.PartId}' and CompanyId = '{packingslip.CompanyId}' and WarehouseId = '{packingSlipDetail.WarehouseId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }//await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{packingSlipDetail.PartId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.REVERT_CUSTOMER_PACKINGSLIP)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.IN)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND)}'   ,'{id.ToString()}'   ,'{packingSlipDetail.Qty}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        sql = string.Format($"DELETE FROM [dbo].[PackingSlipDetails] WHERE id = '{packingSlipDetail.Id}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        if (packingslip.IsMonthly)
                        {
                            sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [AdjustedInvoiceQty] =  0   WHERE InvoiceId = '{packingSlipDetail.SupplierInvoiceId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }

                    }

                    sql = string.Format($"DELETE FROM [dbo].[PackingSlipBoxDetails]  WHERE PackingSlipId = '{id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"DELETE FROM [dbo].[PackingSlipDetails]  WHERE PackingSlipId = '{id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"DELETE FROM [dbo].[PackingSlipMaster]  WHERE id = '{id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"INSERT INTO [dbo].[DeletedPackingSlip]   ([PackingSlipNo],[CompanyId]) VALUES   ('{packingslip.PackingSlipNo}','{packingslip.CompanyId}')");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return true;
        }

        public async Task<IEnumerable<PackingSlip>> GetAllPackingSlipsAsync(int companyId, int userId)
        {
            List<PackingSlip> packingSlips = new List<PackingSlip>();

            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate],[IsPOSUploaded],[POSPath],[TotalSurcharge],[IsMasterPackingSlip],[MasterPackingSlipId],[IsRepackage],[TrakingNumber],[IsShipmentVerified],[IsScanned],[AllowScanning]   " +
                $"FROM [dbo].[PackingSlipMaster] where CompanyId = '{companyId}' ");
            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);

                commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
               $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
               $"[ShipmentInfoId] ,[InvoiceDate],[IsPOSUploaded],[POSPath],[TotalSurcharge],[IsMasterPackingSlip],[MasterPackingSlipId],[IsRepackage],[TrakingNumber] ,[IsShipmentVerified],[IsScanned],[AllowScanning]  " +
               $"FROM [dbo].[PackingSlipMaster] where CompanyId = '{companyId}' and  [CustomerId] in ({companylist}) ");
            }
            if (userInfo.UserTypeId == 3)
            {
                return packingSlips;
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new PackingSlip();
                    packingSlip.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlip.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    packingSlip.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    packingSlip.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    packingSlip.Boxes = Convert.ToInt32(dataReader["Boxes"]);
                    packingSlip.GrossWeight = Convert.ToDecimal(dataReader["GrossWeight"]);
                    packingSlip.ShippingCharge = Convert.ToDecimal(dataReader["ShippingCharge"]);
                    packingSlip.CustomCharge = Convert.ToDecimal(dataReader["CustomCharge"]);
                    packingSlip.SubTotal = Convert.ToDecimal(dataReader["SubTotal"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlip.IsInvoiceCreated = Convert.ToBoolean(dataReader["IsInvoiceCreated"]);
                    packingSlip.IsPaymentReceived = Convert.ToBoolean(dataReader["IsPaymentReceived"]);
                    packingSlip.FOB = Convert.ToString(dataReader["FOB"]);
                    packingSlip.Terms = Convert.ToString(dataReader["Terms"]);
                    packingSlip.ShipmentInfoId = Convert.ToInt32(dataReader["ShipmentInfoId"]);
                    packingSlip.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    packingSlip.IsPOSUploaded = Convert.ToBoolean(dataReader["IsPOSUploaded"]);
                    packingSlip.POSPath = Convert.ToString(dataReader["POSPath"]);
                    packingSlip.TotalSurcharge = Convert.ToDecimal(dataReader["TotalSurcharge"]);
                    packingSlip.IsMasterPackingSlip = Convert.ToBoolean(dataReader["IsMasterPackingSlip"]);
                    packingSlip.MasterPackingSlipId = Convert.ToInt32(dataReader["MasterPackingSlipId"]);
                    packingSlip.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    packingSlip.TrakingNumber = Convert.ToString(dataReader["TrakingNumber"]);
                    packingSlip.IsShipmentVerified = Convert.ToBoolean(dataReader["IsShipmentVerified"]);
                    packingSlip.IsScanned = Convert.ToBoolean(dataReader["IsScanned"]);
                    packingSlip.AllowScanning = Convert.ToBoolean(dataReader["AllowScanning"]);
                    packingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }

            foreach (PackingSlip packingSlip in packingSlips)
            {
                List<PackingSlipDetails> packingSlipDetails = new List<PackingSlipDetails>();
                commandText = string.Format($"SELECT [Id] ,[PackingSlipId] ,[IsBlankOrder] ,[OrderNo] ,[OrderId] ,[OrderDetailId] ,[PartId] ,[Qty] ," +
                    $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty],[SrNo],[IsRepackage],[LineNumber]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var packingSlipDetail = new PackingSlipDetails();
                        packingSlipDetail.Id = Convert.ToInt32(dataReader1["Id"]);
                        packingSlipDetail.PackingSlipId = Convert.ToInt32(dataReader1["PackingSlipId"]);
                        packingSlipDetail.IsBlankOrder = Convert.ToBoolean(dataReader1["IsBlankOrder"]);
                        packingSlipDetail.OrderNo = Convert.ToString(dataReader1["OrderNo"]);
                        packingSlipDetail.OrderId = Convert.ToInt32(dataReader1["OrderId"]);
                        packingSlipDetail.OrderDetailId = Convert.ToInt32(dataReader1["OrderDetailId"]);
                        packingSlipDetail.PartId = Convert.ToInt32(dataReader1["PartId"]);
                        packingSlipDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                        packingSlipDetail.Boxes = Convert.ToInt32(dataReader1["Boxes"]);
                        packingSlipDetail.InBasket = Convert.ToBoolean(dataReader1["InBasket"]);
                        packingSlipDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                        packingSlipDetail.Price = Convert.ToDecimal(dataReader1["Price"]);
                        packingSlipDetail.Surcharge = Convert.ToDecimal(dataReader1["Surcharge"]);
                        packingSlipDetail.SurchargePerPound = Convert.ToDecimal(dataReader1["SurchargePerPound"]);
                        packingSlipDetail.SurchargePerUnit = Convert.ToDecimal(dataReader1["SurchargePerUnit"]);
                        packingSlipDetail.TotalSurcharge = Convert.ToDecimal(dataReader1["TotalSurcharge"]);
                        packingSlipDetail.ExcessQty = Convert.ToInt32(dataReader1["ExcessQty"]);
                        packingSlipDetail.IsRepackage = Convert.ToBoolean(dataReader1["IsRepackage"]);

                        if (dataReader1["SrNo"] != DBNull.Value)
                            packingSlipDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                        else
                            packingSlipDetail.SrNo = 0;

                        if (dataReader1["LineNumber"] != DBNull.Value)
                            packingSlipDetail.LineNumber = Convert.ToString(dataReader1["LineNumber"]);
                        else
                            packingSlipDetail.LineNumber = "";

                        packingSlipDetails.Add(packingSlipDetail);
                    }
                    dataReader1.Close();
                }
                packingSlip.PackingSlipDetails = packingSlipDetails;
                conn.Close();
            }

            foreach (PackingSlip packingSlip in packingSlips)
            {
                foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                {
                    var packingSlipBoxDetails = new List<PackingSlipBoxDetails>();
                    commandText = string.Format($"SELECT  psbd.[Id] ,[PackingSlipId] ,[PackingSlipDetailId] ,[PartId],p.Code ,[Qty],[BoxeNo] ,[Barcode] ,[IsScanned] FROM [dbo].[PackingSlipBoxDetails] psbd inner join part p on p.id = psbd.PartId where psbd.PackingSlipDetailId = '{packingSlipDetail.Id}' ");

                    using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                    {
                        cmd1.CommandType = CommandType.Text;
                        conn.Open();
                        var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                        while (dataReader1.Read())
                        {
                            var packingSlipBoxDetail = new PackingSlipBoxDetails();
                            packingSlipBoxDetail.Id = Convert.ToInt32(dataReader1["Id"]);
                            packingSlipBoxDetail.PackingSlipId = Convert.ToInt32(dataReader1["PackingSlipId"]);
                            packingSlipBoxDetail.PackingSlipDetailId = Convert.ToInt32(dataReader1["PackingSlipDetailId"]);
                            packingSlipBoxDetail.PartId = Convert.ToInt32(dataReader1["PartId"]);
                            packingSlipBoxDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                            packingSlipBoxDetail.BoxeNo = Convert.ToInt32(dataReader1["BoxeNo"]);
                            packingSlipBoxDetail.Barcode = Convert.ToString(dataReader1["Barcode"]);
                            packingSlipBoxDetail.IsScanned = Convert.ToBoolean(dataReader1["IsScanned"]);
                            packingSlipBoxDetail.PartCode = Convert.ToString(dataReader1["Code"]);
                            packingSlipBoxDetails.Add(packingSlipBoxDetail);
                        }
                        dataReader1.Close();
                    }
                    packingSlipDetail.PackingSlipBoxDetails = packingSlipBoxDetails;
                    conn.Close();
                }
            }

            return packingSlips.OrderByDescending(x => x.PackingSlipNo); ;
        }

        public async Task<PackingSlip> GetPackingSlipAsync(long id)
        {
            var packingSlip = new PackingSlip();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate],[IsPOSUploaded],[POSPath],[TotalSurcharge],[IsMasterPackingSlip],[MasterPackingSlipId],[IsRepackage],[TrakingNumber],[IsShipmentVerified],[IsScanned],[AllowScanning] " +
                $"  FROM [dbo].[PackingSlipMaster] where Id = '{id}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    packingSlip.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlip.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    packingSlip.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    packingSlip.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    packingSlip.Boxes = Convert.ToInt32(dataReader["Boxes"]);
                    packingSlip.GrossWeight = Convert.ToDecimal(dataReader["GrossWeight"]);
                    packingSlip.ShippingCharge = Convert.ToDecimal(dataReader["ShippingCharge"]);
                    packingSlip.CustomCharge = Convert.ToDecimal(dataReader["CustomCharge"]);
                    packingSlip.SubTotal = Convert.ToDecimal(dataReader["SubTotal"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlip.IsInvoiceCreated = Convert.ToBoolean(dataReader["IsInvoiceCreated"]);
                    packingSlip.IsPaymentReceived = Convert.ToBoolean(dataReader["IsPaymentReceived"]);
                    packingSlip.FOB = Convert.ToString(dataReader["FOB"]);
                    packingSlip.Terms = Convert.ToString(dataReader["Terms"]);
                    packingSlip.ShipmentInfoId = Convert.ToInt32(dataReader["ShipmentInfoId"]);
                    packingSlip.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    packingSlip.IsPOSUploaded = Convert.ToBoolean(dataReader["IsPOSUploaded"]);
                    packingSlip.POSPath = Convert.ToString(dataReader["POSPath"]);
                    packingSlip.TotalSurcharge = Convert.ToDecimal(dataReader["TotalSurcharge"]);
                    packingSlip.IsMasterPackingSlip = Convert.ToBoolean(dataReader["IsMasterPackingSlip"]);
                    packingSlip.MasterPackingSlipId = Convert.ToInt32(dataReader["MasterPackingSlipId"]);
                    packingSlip.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    packingSlip.TrakingNumber = Convert.ToString(dataReader["TrakingNumber"]);
                    packingSlip.IsShipmentVerified = Convert.ToBoolean(dataReader["IsShipmentVerified"]);
                    packingSlip.IsScanned = Convert.ToBoolean(dataReader["IsScanned"]);
                    packingSlip.AllowScanning = Convert.ToBoolean(dataReader["AllowScanning"]);

                }
                dataReader.Close();
                conn.Close();
            }

            List<PackingSlipDetails> packingSlipDetails = new List<PackingSlipDetails>();
            commandText = string.Format($"SELECT [Id] ,[PackingSlipId] ,[IsBlankOrder] ,[OrderNo] ,[OrderId] ,[OrderDetailId] ,[PartId] ,[Qty] ," +
                $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty],[SrNo],[IsRepackage],[SupplierInvoiceId],[LineNumber]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var packingSlipDetail = new PackingSlipDetails();
                    packingSlipDetail.Id = Convert.ToInt32(dataReader1["Id"]);
                    packingSlipDetail.PackingSlipId = Convert.ToInt32(dataReader1["PackingSlipId"]);
                    packingSlipDetail.IsBlankOrder = Convert.ToBoolean(dataReader1["IsBlankOrder"]);
                    packingSlipDetail.OrderNo = Convert.ToString(dataReader1["OrderNo"]);
                    packingSlipDetail.OrderId = Convert.ToInt32(dataReader1["OrderId"]);
                    packingSlipDetail.OrderDetailId = Convert.ToInt32(dataReader1["OrderDetailId"]);
                    packingSlipDetail.PartId = Convert.ToInt32(dataReader1["PartId"]);
                    packingSlipDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                    packingSlipDetail.Boxes = Convert.ToInt32(dataReader1["Boxes"]);
                    packingSlipDetail.InBasket = Convert.ToBoolean(dataReader1["InBasket"]);
                    packingSlipDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                    packingSlipDetail.Price = Convert.ToDecimal(dataReader1["Price"]);
                    packingSlipDetail.Surcharge = Convert.ToDecimal(dataReader1["Surcharge"]);
                    packingSlipDetail.SurchargePerPound = Convert.ToDecimal(dataReader1["SurchargePerPound"]);
                    packingSlipDetail.SurchargePerUnit = Convert.ToDecimal(dataReader1["SurchargePerUnit"]);
                    packingSlipDetail.TotalSurcharge = Convert.ToDecimal(dataReader1["TotalSurcharge"]);
                    packingSlipDetail.ExcessQty = Convert.ToInt32(dataReader1["ExcessQty"]);
                    packingSlipDetail.IsRepackage = Convert.ToBoolean(dataReader1["IsRepackage"]);
                    packingSlipDetail.SupplierInvoiceId = Convert.ToInt32(dataReader1["SupplierInvoiceId"]);
                    if (dataReader1["SrNo"] != DBNull.Value)
                        packingSlipDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                    else
                        packingSlipDetail.SrNo = 0;

                    if (dataReader1["LineNumber"] != DBNull.Value)
                        packingSlipDetail.LineNumber = Convert.ToString(dataReader1["LineNumber"]);
                    else
                        packingSlipDetail.LineNumber = "";

                    packingSlipDetails.Add(packingSlipDetail);
                }
                dataReader1.Close();
            }
            packingSlip.PackingSlipDetails = packingSlipDetails;
            conn.Close();

            foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
            {
                var packingSlipBoxDetails = new List<PackingSlipBoxDetails>();
                commandText = string.Format($"SELECT  psbd.[Id] ,[PackingSlipId] ,[PackingSlipDetailId] ,[PartId],p.Code ,[Qty],[BoxeNo] ,[Barcode] ,[IsScanned] FROM [dbo].[PackingSlipBoxDetails] psbd inner join part p on p.id = psbd.PartId where psbd.PackingSlipDetailId = '{packingSlipDetail.Id}' ");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var packingSlipBoxDetail = new PackingSlipBoxDetails();
                        packingSlipBoxDetail.Id = Convert.ToInt32(dataReader1["Id"]);
                        packingSlipBoxDetail.PackingSlipId = Convert.ToInt32(dataReader1["PackingSlipId"]);
                        packingSlipBoxDetail.PackingSlipDetailId = Convert.ToInt32(dataReader1["PackingSlipDetailId"]);
                        packingSlipBoxDetail.PartId = Convert.ToInt32(dataReader1["PartId"]);
                        packingSlipBoxDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                        packingSlipBoxDetail.BoxeNo = Convert.ToInt32(dataReader1["BoxeNo"]);
                        packingSlipBoxDetail.Barcode = Convert.ToString(dataReader1["Barcode"]);
                        packingSlipBoxDetail.IsScanned = Convert.ToBoolean(dataReader1["IsScanned"]);
                        packingSlipBoxDetail.PartCode = Convert.ToString(dataReader1["Code"]);
                        packingSlipBoxDetails.Add(packingSlipBoxDetail);
                    }
                    dataReader1.Close();
                }
                packingSlipDetail.PackingSlipBoxDetails = packingSlipBoxDetails;
                conn.Close();
            }

            return packingSlip;
        }

        public async Task<IEnumerable<DeletedPackingSlip>> GetDeletedPackingSlipAsync(int companyId)
        {
            var packingSlips = new List<DeletedPackingSlip>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [id] ,[PackingSlipNo] FROM [DeletedPackingSlip] where CompanyId = '{companyId}'");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new DeletedPackingSlip();
                    packingSlip.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlip.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);

                    packingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }


            return packingSlips;
        }

        public PackingSlip GetPackingSlip(long id)
        {
            var packingSlip = new PackingSlip();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate],[IsPOSUploaded],[POSPath],[IsRepackage]  FROM [dbo].[PackingSlipMaster] where Id = '{id}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    packingSlip.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlip.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    packingSlip.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    packingSlip.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    packingSlip.Boxes = Convert.ToInt32(dataReader["Boxes"]);
                    packingSlip.GrossWeight = Convert.ToDecimal(dataReader["GrossWeight"]);
                    packingSlip.ShippingCharge = Convert.ToDecimal(dataReader["ShippingCharge"]);
                    packingSlip.CustomCharge = Convert.ToDecimal(dataReader["CustomCharge"]);
                    packingSlip.SubTotal = Convert.ToDecimal(dataReader["SubTotal"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlip.IsInvoiceCreated = Convert.ToBoolean(dataReader["IsInvoiceCreated"]);
                    packingSlip.IsPaymentReceived = Convert.ToBoolean(dataReader["IsPaymentReceived"]);
                    packingSlip.FOB = Convert.ToString(dataReader["FOB"]);
                    packingSlip.Terms = Convert.ToString(dataReader["Terms"]);
                    packingSlip.ShipmentInfoId = Convert.ToInt32(dataReader["ShipmentInfoId"]);
                    packingSlip.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    packingSlip.IsPOSUploaded = Convert.ToBoolean(dataReader["IsPOSUploaded"]);
                    packingSlip.POSPath = Convert.ToString(dataReader["POSPath"]);

                    packingSlip.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);

                }
                dataReader.Close();
                conn.Close();
            }

            packingSlip.customerShippingInfo = new CustomerShippingInfo();
            commandText = string.Format($"SELECT  [id] ,[CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] " +
                $",[City] ,[State] ,[ZIPCode] ,[IsDefault]  FROM [customershippinginfo] where Id = '{packingSlip.ShipmentInfoId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    packingSlip.customerShippingInfo.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlip.customerShippingInfo.Name = Convert.ToString(dataReader["Name"]);
                    packingSlip.customerShippingInfo.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                    packingSlip.customerShippingInfo.AddressLine1 = Convert.ToString(dataReader["AddressLine1"]);
                    packingSlip.customerShippingInfo.City = Convert.ToString(dataReader["City"]);
                    packingSlip.customerShippingInfo.State = Convert.ToString(dataReader["State"]);
                    packingSlip.customerShippingInfo.ZIPCode = Convert.ToString(dataReader["ZIPCode"]);
                    packingSlip.customerShippingInfo.IsDefault = Convert.ToBoolean(dataReader["IsDefault"]);
                }
                dataReader.Close();
                conn.Close();
            }

            List<PackingSlipDetails> packingSlipDetails = new List<PackingSlipDetails>();
            commandText = string.Format($"SELECT [Id] ,[PackingSlipId] ,[IsBlankOrder] ,[OrderNo] ,[OrderId] ,[OrderDetailId] ,[PartId] ,[Qty] ," +
                $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty],[SrNo],[IsRepackage]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var packingSlipDetail = new PackingSlipDetails();
                    packingSlipDetail.Id = Convert.ToInt32(dataReader1["Id"]);
                    packingSlipDetail.PackingSlipId = Convert.ToInt32(dataReader1["PackingSlipId"]);
                    packingSlipDetail.IsBlankOrder = Convert.ToBoolean(dataReader1["IsBlankOrder"]);
                    packingSlipDetail.OrderNo = Convert.ToString(dataReader1["OrderNo"]);
                    packingSlipDetail.OrderId = Convert.ToInt32(dataReader1["OrderId"]);
                    packingSlipDetail.OrderDetailId = Convert.ToInt32(dataReader1["OrderDetailId"]);
                    packingSlipDetail.PartId = Convert.ToInt32(dataReader1["PartId"]);
                    packingSlipDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                    packingSlipDetail.Boxes = Convert.ToInt32(dataReader1["Boxes"]);
                    packingSlipDetail.InBasket = Convert.ToBoolean(dataReader1["InBasket"]);
                    packingSlipDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                    packingSlipDetail.Price = Convert.ToDecimal(dataReader1["Price"]);
                    packingSlipDetail.Surcharge = Convert.ToDecimal(dataReader1["Surcharge"]);
                    packingSlipDetail.SurchargePerPound = Convert.ToDecimal(dataReader1["SurchargePerPound"]);
                    packingSlipDetail.SurchargePerUnit = Convert.ToDecimal(dataReader1["SurchargePerUnit"]);
                    packingSlipDetail.TotalSurcharge = Convert.ToDecimal(dataReader1["TotalSurcharge"]);
                    packingSlipDetail.ExcessQty = Convert.ToInt32(dataReader1["ExcessQty"]);
                    packingSlipDetail.LineNumber = Convert.ToString(dataReader1["LineNumber"]);
                    packingSlipDetail.IsRepackage = Convert.ToBoolean(dataReader1["IsRepackage"]);

                    if (dataReader1["SrNo"] != DBNull.Value)
                        packingSlipDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                    else
                        packingSlipDetail.SrNo = 0;

                    packingSlipDetails.Add(packingSlipDetail);
                }
                dataReader1.Close();
            }
            packingSlip.PackingSlipDetails = packingSlipDetails;
            conn.Close();

            return packingSlip;
        }

        public async Task<bool> UpdatePackingSlipAsync(PackingSlip packingSlip)
        {
            var id = packingSlip.Id;
            var packingslip = await GetPackingSlipAsync(id);

            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

                //1. Get SupplierInvoicePoDetails po transaction detail
                List<SupplierInvoicePoDetails> supplierInvoicePoDetailsList = new List<SupplierInvoicePoDetails>();
                SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;
                string sql = string.Empty;
                try
                {
                    foreach (PackingSlipDetails packingSlipDetail in packingslip.PackingSlipDetails)
                    {
                        var partDetail = await this.partRepository.GetPartAsync(packingSlipDetail.PartId, connection, transaction);

                        packingSlipDetail.DefaultWarehouse = partDetail.DefaultWarehouse;
                        packingSlipDetail.WarehouseId = partDetail.WarehouseId;

                        if (packingSlipDetail.OrderDetailId != null && packingSlipDetail.OrderDetailId > 0)
                        {
                            var reflectedQty = packingSlipDetail.Qty - packingSlipDetail.ExcessQty;
                            sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET [ShippedQty] = [ShippedQty] - '{reflectedQty}',[IsClosed] = '{false}',[ClosingDate] = NULL   WHERE id = '{packingSlipDetail.OrderDetailId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();

                            sql = string.Format($"UPDATE [dbo].[OrderMaster]   SET [IsClosed] = '{false}',[ClosingDate] = NULL   WHERE id = '{packingSlipDetail.OrderId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }

                        if (packingslip.IsInvoiceCreated)
                        {
                            sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand + '{packingSlipDetail.Qty}', [CurrentPricingInEffectQty] = CurrentPricingInEffectQty + '{packingSlipDetail.Qty}' WHERE id = '{packingSlipDetail.PartId}' ");

                            var stocks = GetStock(packingSlipDetail.PartId, command.Connection, command.Transaction);

                            foreach (StockPrice stockPrice in stocks)
                            {
                                var sql1 = string.Format($"UPDATE [StockPrice] SET [Qty] = [Qty] + '{packingSlipDetail.Qty}' WHERE id = '{stockPrice.Id}' ");
                                command.CommandText = sql1;
                                await command.ExecuteNonQueryAsync();
                                break;
                            }

                            if (!packingSlipDetail.DefaultWarehouse)
                            {
                                sql = string.Format($"UPDATE [dbo].[PartWarehouseInventory]   SET [QtyInHand] =  QtyInHand + '{packingSlipDetail.Qty}' WHERE PartId = '{packingSlipDetail.PartId}' and CompanyId = '{packingSlip.CompanyId}' and WarehouseId = '{packingSlipDetail.WarehouseId}' ");
                                command.CommandText = sql;
                                await command.ExecuteNonQueryAsync();
                            }

                        }
                        else
                            sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand + '{packingSlipDetail.Qty}'  WHERE id = '{packingSlipDetail.PartId}' ");


                        //sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand + '{packingSlipDetail.Qty}'  WHERE id = '{packingSlipDetail.PartId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{packingSlipDetail.PartId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.REVERT_CUSTOMER_PACKINGSLIP)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.IN)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND)}'   ,'{id.ToString()}'   ,'{packingSlipDetail.Qty}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        sql = string.Format($"DELETE FROM [dbo].[PackingSlipBoxDetails]  WHERE PackingSlipId = '{id}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        sql = string.Format($"DELETE FROM [dbo].[PackingSlipDetails] WHERE id = '{packingSlipDetail.Id}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        if (packingslip.IsMonthly)
                        {
                            sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [AdjustedInvoiceQty] =  0   WHERE InvoiceId = '{packingSlipDetail.SupplierInvoiceId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }

                    }
                    sql = string.Format($"DELETE FROM [dbo].[PackingSlipDetails]  WHERE PackingSlipId = '{id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    packingSlip.SubTotal = 0;
                    packingSlip.Total = 0;
                    packingSlip.TotalSurcharge = 0;
                    packingSlip.GrossWeight = 0;
                    packingSlip.Boxes = 0;
                    var customer = await this.customerRepository.GetCustomerAsync(packingSlip.CustomerId, connection, transaction);
                    packingSlip.FOB = customer.FOB;
                    packingSlip.Terms = customer.Terms;

                    if (packingSlip.ShipmentInfoId == 0)
                    {
                        packingSlip.ShipmentInfoId = customer.ShippingInfos.Where(x => x.IsDefault).FirstOrDefault().Id;
                    }
                    packingSlip.IsRepackage = false;
                    foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                    {
                        var partDetail = await this.partRepository.GetPartAsync(packingSlipDetail.PartId, connection, transaction);
                        packingSlipDetail.DefaultWarehouse = partDetail.DefaultWarehouse;
                        packingSlipDetail.WarehouseId = partDetail.WarehouseId;

                        packingSlipDetail.IsRepackage = partDetail.IsRepackage;
                        if (packingSlipDetail.IsRepackage)
                            packingSlip.IsRepackage = true;

                        if (packingSlip.IsInvoiceCreated)
                            packingSlipDetail.UnitPrice = partDetail.partCustomerAssignments.Where(x => x.CustomerId == packingSlip.CustomerId).Select(x => x.Rate).FirstOrDefault();
                        else
                            packingSlip.IsInvoiceCreated = false;

                        if (packingSlip.IsInvoiceCreated && !packingSlipDetail.IsBlankOrder)
                        {
                            var orderResult = await _orderRepository.GetOrderMasterAsync(packingSlipDetail.OrderId, command.Connection, command.Transaction);

                            if (orderResult != null && !orderResult.IsClosed)
                            {
                                var orderDetail = orderResult.OrderDetails.Where(x => x.Id == packingSlipDetail.OrderDetailId).FirstOrDefault();
                                packingSlipDetail.OrderNo = orderResult.PONo;

                                if (orderDetail != null && !orderDetail.IsClosed)
                                {
                                    packingSlipDetail.LineNumber = orderDetail.LineNumber;
                                    packingSlipDetail.UnitPrice = orderDetail.UnitPrice;
                                }
                            }
                        }
                        else
                            packingSlip.IsInvoiceCreated = false;

                        packingSlipDetail.Price = packingSlipDetail.Qty * packingSlipDetail.UnitPrice;
                        packingSlip.SubTotal = packingSlip.SubTotal + packingSlipDetail.Price;

                        packingSlipDetail.Surcharge = partDetail.partCustomerAssignments.Where(x => x.CustomerId == packingSlip.CustomerId).Select(x => x.SurchargePerPound).FirstOrDefault();
                        packingSlipDetail.SurchargePerPound = packingSlipDetail.Surcharge;
                        packingSlipDetail.SurchargePerUnit = packingSlipDetail.Surcharge * partDetail.WeightInLb;
                        packingSlipDetail.TotalSurcharge = packingSlipDetail.SurchargePerUnit * packingSlipDetail.Qty;
                        packingSlip.TotalSurcharge = packingSlip.TotalSurcharge + packingSlipDetail.TotalSurcharge;
                        packingSlip.GrossWeight = packingSlip.GrossWeight + (packingSlipDetail.Qty * partDetail.WeightInLb);
                        packingSlip.Boxes = packingSlip.Boxes + packingSlipDetail.Boxes;
                        packingSlipDetail.LineNumber = "";
                    }
                    packingSlip.Total = packingSlip.SubTotal + packingSlip.TotalSurcharge + packingSlip.ShippingCharge + packingSlip.CustomCharge;

                    sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [CompanyId] = '{packingSlip.CompanyId}' ,[CustomerId] = '{packingSlip.CustomerId}' " +
                        $",[PackingSlipNo] = '{packingSlip.PackingSlipNo}' ,[ShippingDate] = '{packingSlip.ShippingDate}' ,[ShipVia] = '{packingSlip.ShipVia}' " +
                        $",[Crates] = '{packingSlip.Crates}' ,[Boxes] = '{packingSlip.Boxes}' ,[GrossWeight] = '{packingSlip.GrossWeight}' " +
                        $",[ShippingCharge] = '{packingSlip.ShippingCharge}' ,[CustomCharge] = '{packingSlip.CustomCharge}' ,[SubTotal] = '{packingSlip.SubTotal}' ,[Total] = '{packingSlip.Total}' " +
                        $",[IsPaymentReceived] = '{packingSlip.IsPaymentReceived}' ,[FOB] = '{packingSlip.FOB}' ,[Terms] = '{packingSlip.Terms}' ," +
                        $"[ShipmentInfoId] = '{packingSlip.ShipmentInfoId}'  ,[TotalSurcharge] = '{packingSlip.TotalSurcharge}' ,[IsRepackage] = '{packingSlip.IsRepackage}' ,[IsInvoiceCreated] = '{packingSlip.IsInvoiceCreated}'" +
                        $" ,[IsShipmentVerified] = '{false}' ,[AllowScanning] = '{false}' WHERE id = '{packingSlip.Id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    var packingSlipId = packingSlip.Id;

                    foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                    {
                        packingSlipDetail.ExcessQty = 0;
                        if (!packingSlipDetail.IsBlankOrder)
                        {
                            var orderResult = await _orderRepository.GetOrderMasterAsync(packingSlipDetail.OrderId, command.Connection, command.Transaction);

                            if (orderResult != null && !orderResult.IsClosed)
                            {
                                var orderDetail = orderResult.OrderDetails.Where(x => x.Id == packingSlipDetail.OrderDetailId).FirstOrDefault();
                                packingSlipDetail.OrderNo = orderResult.PONo;

                                if (orderDetail != null && !orderDetail.IsClosed)
                                {
                                    packingSlipDetail.LineNumber = orderDetail.LineNumber;
                                    int availableshippedQty = orderDetail.Qty - orderDetail.ShippedQty;
                                    if (packingSlipDetail.Qty < availableshippedQty)
                                    {
                                        sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET [ShippedQty] = '{orderDetail.ShippedQty + packingSlipDetail.Qty}'  WHERE id = '{orderDetail.Id}' ");
                                        command.CommandText = sql;
                                        await command.ExecuteNonQueryAsync();
                                    }
                                    else
                                    {
                                        packingSlipDetail.ExcessQty = packingSlipDetail.Qty - availableshippedQty;
                                        sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET [ShippedQty] = '{orderDetail.ShippedQty + packingSlipDetail.Qty}',[IsClosed] = '{true}',[ClosingDate] = '{DateTime.Now}'   WHERE id = '{orderDetail.Id}' ");
                                        command.CommandText = sql;
                                        await command.ExecuteNonQueryAsync();

                                        orderResult = await _orderRepository.GetOrderMasterAsync(packingSlipDetail.OrderId, command.Connection, command.Transaction);
                                        var openPO = orderResult.OrderDetails.Where(x => x.OrderId == packingSlipDetail.OrderId && x.IsClosed == false).FirstOrDefault();

                                        if (openPO == null)
                                        {
                                            sql = string.Format($"UPDATE [dbo].[OrderMaster]   SET [IsClosed] = '{true}' ,[ClosingDate] = '{DateTime.Now}'  WHERE id = '{packingSlipDetail.OrderId}' ");
                                            command.CommandText = sql;
                                            await command.ExecuteNonQueryAsync();
                                            //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                                        }
                                    }
                                }
                            }
                        }

                        if (customer.Invoicingtypeid == 3 && packingSlipDetail.SupplierInvoiceId > 0)
                        {
                            if (packingSlipDetail.SupplierInvoiceOpenQty == packingSlipDetail.Qty)
                            {
                                sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [AdjustedInvoiceQty] =  [Qty]   WHERE [InvoiceId] = '{packingSlipDetail.SupplierInvoiceId}' ");
                                command.CommandText = sql;
                                await command.ExecuteNonQueryAsync();
                            }
                            else
                            {
                                var supOpenInvoice = await supplierInvoiceRepository.GetSupplierInvoiceAsync(packingSlipDetail.SupplierInvoiceId, connection, transaction);
                                var openQty = 0;
                                var remainingQty = packingSlipDetail.Qty;
                                foreach (SupplierInvoiceDetail supplierInvoiceDetail in supOpenInvoice.supplierInvoiceDetails)
                                {
                                    if (remainingQty > 0)
                                    {
                                        openQty = supplierInvoiceDetail.Qty - supplierInvoiceDetail.AdjustedInvoiceQty;
                                        if (openQty >= remainingQty)
                                        {
                                            sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [AdjustedInvoiceQty] = [AdjustedInvoiceQty] +  '{openQty}'   WHERE [id] = '{supplierInvoiceDetail.Id}' ");
                                            command.CommandText = sql;
                                            await command.ExecuteNonQueryAsync();
                                        }
                                        else
                                        {
                                            sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [AdjustedInvoiceQty] = [Qty]  WHERE [id] = '{supplierInvoiceDetail.Id}' ");
                                            command.CommandText = sql;
                                            await command.ExecuteNonQueryAsync();
                                        }
                                    }
                                    remainingQty = remainingQty - openQty;
                                }
                            }
                        }

                        //var partDetail = await this.partRepository.GetPartAsync(packingSlipDetail.PartId);
                        //packingSlipDetail.SurchargePerUnit = partDetail.partCustomerAssignments.Where(x => x.CustomerId == packingSlip.CustomerId).Select(x => x.SurchargePerPound).FirstOrDefault();
                        //packingSlipDetail.TotalSurcharge = packingSlipDetail.SurchargePerUnit * packingSlipDetail.Qty;
                        sql = string.Format($"INSERT INTO [dbo].[PackingSlipDetails]   ([PackingSlipId]   ,[IsBlankOrder]   ,[OrderNo]   ,[OrderId]   ,[OrderDetailId]   ,[PartId]   ,[Qty]   ,[Boxes]   ,[InBasket]   ,[UnitPrice]   ,[Price]   ,[Surcharge]   ,[SurchargePerPound]   ,[SurchargePerUnit]   ,[TotalSurcharge]   ,[ExcessQty],[SrNo],[LineNumber],[IsRepackage],[SupplierInvoiceId]) VALUES   ('{packingSlipId}'   ,'{packingSlipDetail.IsBlankOrder}'   ,'{packingSlipDetail.OrderNo}'   ,'{packingSlipDetail.OrderId}'   ,'{packingSlipDetail.OrderDetailId}'   ,'{packingSlipDetail.PartId}'   ,'{packingSlipDetail.Qty}'   ,'{packingSlipDetail.Boxes}'   ,'{packingSlipDetail.InBasket}'   ,'{packingSlipDetail.UnitPrice}'   ,'{packingSlipDetail.Price}'   ,'{packingSlipDetail.Surcharge}'   ,'{packingSlipDetail.SurchargePerPound}'   ,'{packingSlipDetail.SurchargePerUnit}'   ,'{packingSlipDetail.TotalSurcharge}'   ,'{packingSlipDetail.ExcessQty}','{packingSlipDetail.SrNo}','{packingSlipDetail.LineNumber}','{packingSlipDetail.IsRepackage}','{packingSlipDetail.SupplierInvoiceId}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        if (packingslip.IsInvoiceCreated)
                            sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand - '{packingSlipDetail.Qty}', [CurrentPricingInEffectQty] = CurrentPricingInEffectQty - '{packingSlipDetail.Qty}' WHERE id = '{packingSlipDetail.PartId}' ");
                        else
                            sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand - '{packingSlipDetail.Qty}'  WHERE id = '{packingSlipDetail.PartId}' ");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        if (!packingSlipDetail.DefaultWarehouse)
                        {
                            sql = string.Format($"UPDATE [dbo].[PartWarehouseInventory]   SET [QtyInHand] =  QtyInHand - '{packingSlipDetail.Qty}' WHERE PartId = '{packingSlipDetail.PartId}' and CompanyId = '{packingSlip.CompanyId}' and WarehouseId = '{packingSlipDetail.WarehouseId}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{packingSlipDetail.PartId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.CUSTOMER_PACKINGSLIP)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.OUT)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND)}'   ,'{packingSlip.Id.ToString()}'   ,'{packingSlipDetail.Qty}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return true;
        }

        public async Task UpdatePOSAsync(int packingSlipId, string path, string trackingNumber, string accessId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    string sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [IsPOSUploaded] = '{true}' ,[POSPath] = '{path}',[TrakingNumber]='{trackingNumber}' ,[AccessId]='{accessId}'  WHERE Id = '{packingSlipId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task<bool> VerifyPackingSlipAsync(PackingSlip packingSlip, int userId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;
                string sql = string.Empty;
                try
                {
                    sql = string.Format($"DELETE FROM [dbo].[PackingSlipBoxDetails]  WHERE PackingSlipId = '{packingSlip.Id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    packingSlip.Boxes = 0;
                    foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                    {
                        packingSlipDetail.Boxes = 0;
                        foreach (PackingSlipBoxDetails packingSlipBoxDetail in packingSlipDetail.PackingSlipBoxDetails)
                        {
                            packingSlipDetail.Boxes = packingSlipDetail.Boxes + 1;
                            packingSlip.Boxes = packingSlip.Boxes + 1;
                            sql = string.Format($"INSERT INTO [dbo].[PackingSlipBoxDetails]   ([PackingSlipId]   ,[PackingSlipDetailId]   ,[PartId]   ,[Qty]   ,[BoxeNo]   ,[Barcode]   ,[IsScanned]   ) VALUES   " +
                                $"('{packingSlip.Id}'   ,'{packingSlipDetail.Id}'   ,'{packingSlipDetail.PartId}'   ,'{packingSlipBoxDetail.Qty}'   ,'{packingSlipBoxDetail.BoxeNo}'   ,'{string.Empty}'   ,'{false}' )");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }

                        sql = string.Format($"UPDATE [dbo].[PackingSlipDetails]   SET [Boxes] = '{packingSlipDetail.Boxes}' WHERE id = '{packingSlipDetail.Id}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [Boxes] = '{packingSlip.Boxes}' ,[IsShipmentVerified] = '{true}'   WHERE id = '{packingSlip.Id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    UserActivityReport userActivityReport = new UserActivityReport();
                    userActivityReport.UserId = userId;
                    userActivityReport.Module = BusinessConstants.MODULE.PACKING_SLIP.ToString();
                    userActivityReport.Action = BusinessConstants.ACTION.VERIFY_SHIPMENT.ToString();
                    userActivityReport.Reference = packingSlip.PackingSlipNo;
                    await this.userActivityReportRepository.AddActivityAsync(userActivityReport, connection, transaction, command);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            var packingSlipBoxDetails = new List<PackingSlipBoxDetails>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [Id]  FROM [dbo].[PackingSlipBoxDetails] Where [PackingSlipId] = {0};", packingSlip.Id);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlipBoxDetail = new PackingSlipBoxDetails();

                    packingSlipBoxDetail.Id = Convert.ToInt32(dataReader["Id"]);

                    packingSlipBoxDetails.Add(packingSlipBoxDetail);
                }
                dataReader.Close();
                conn.Close();
            }

            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    foreach (PackingSlipBoxDetails packingSlipBoxDetail in packingSlipBoxDetails)
                    {
                        var sql = string.Format($"UPDATE [dbo].[PackingSlipBoxDetails]   SET [Barcode] =  '{BarCodeUtil.GetBarCodeString(packingSlipBoxDetail.Id)}' WHERE id = '{packingSlipBoxDetail.Id}' ");
                        await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return true;
        }

        public async Task<bool> UndoVerifyPackingSlipAsync(int packingSlipId, int userId)
        {
            var packingSlip = await GetPackingSlipAsync(packingSlipId);

            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;
                string sql = string.Empty;
                try
                {
                    sql = string.Format($"DELETE FROM [dbo].[PackingSlipBoxDetails]  WHERE PackingSlipId = '{packingSlipId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();



                    sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [IsShipmentVerified] = '{false}'   WHERE id = '{packingSlipId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    UserActivityReport userActivityReport = new UserActivityReport();
                    userActivityReport.UserId = userId;
                    userActivityReport.Module = BusinessConstants.MODULE.PACKING_SLIP.ToString();
                    userActivityReport.Action = BusinessConstants.ACTION.UNDO_VERIFY_SHIPMENT.ToString();
                    userActivityReport.Reference = packingSlip.PackingSlipNo;
                    await this.userActivityReportRepository.AddActivityAsync(userActivityReport, connection, transaction, command);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            var packingSlipBoxDetails = new List<PackingSlipBoxDetails>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [Id]  FROM [dbo].[PackingSlipBoxDetails] Where [PackingSlipId] = {0};", packingSlip.Id);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlipBoxDetail = new PackingSlipBoxDetails();

                    packingSlipBoxDetail.Id = Convert.ToInt32(dataReader["Id"]);

                    packingSlipBoxDetails.Add(packingSlipBoxDetail);
                }
                dataReader.Close();
                conn.Close();
            }

            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    foreach (PackingSlipBoxDetails packingSlipBoxDetail in packingSlipBoxDetails)
                    {
                        var sql = string.Format($"UPDATE [dbo].[PackingSlipBoxDetails]   SET [Barcode] =  '{BarCodeUtil.GetBarCodeString(packingSlipBoxDetail.Id)}' WHERE id = '{packingSlipBoxDetail.Id}' ");
                        await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return true;
        }

        public async Task<List<PackingSlipScanBoxeStatus>> ScanPackingSlipBox(string barcode, int userId)
        {
            var packingslipId = await GetPackingslipFromBarcodeAsync(barcode);
            var packingSlip = await GetPackingSlipAsync(packingslipId);
            //throw new NotImplementedException();
            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {

                    var sql = string.Format($"UPDATE [dbo].[PackingSlipBoxDetails]   SET [IsScanned] = '{true}'  WHERE barcode = '{barcode}' ");

                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                    //var packingslipId = await GetPackingslipFromBarcodeAsync(barcode, command.Connection, command.Transaction); ;
                    var BoxDetails = await GetBoxDetailFromBarcodeAsync(barcode, command.Connection, command.Transaction);
                    foreach (PackingSlipDetails packingSlipDetails in packingSlip.PackingSlipDetails)
                    {
                        foreach (PackingSlipBoxDetails packingSlipBox in packingSlipDetails.PackingSlipBoxDetails)
                        {
                            if (packingSlipBox.Barcode == barcode)
                            {
                                UserActivityReport userActivityReport = new UserActivityReport();
                                userActivityReport.UserId = userId;
                                userActivityReport.Module = BusinessConstants.MODULE.PACKING_SLIP.ToString();
                                userActivityReport.Action = BusinessConstants.ACTION.SCAN_BOX.ToString();
                                userActivityReport.Reference = packingSlip.PackingSlipNo;
                                var part = await partRepository.GetPartAsync(packingSlipBox.PartId, command.Connection, command.Transaction);

                                try
                                {
                                    if (packingSlipDetails.LineNumber == null)
                                        packingSlipDetails.LineNumber = string.Empty;
                                    if (part != null)
                                        userActivityReport.Description = "Line # : " + packingSlipDetails.LineNumber.ToString() + " Part : " + part.Code.ToString() + " Box # : " + packingSlipBox.BoxeNo.ToString() + " scanned";
                                    else
                                        userActivityReport.Description = "Line # : " + packingSlipDetails.LineNumber.ToString() + " PartId : " + packingSlipBox.PartId.ToString() + " Box # : " + packingSlipBox.BoxeNo.ToString() + " scanned";
                                    await this.userActivityReportRepository.AddActivityAsync(userActivityReport, connection, transaction, command);
                                }
                                catch
                                {

                                }
                           }
                        }
                    }

                    var packingSlipBoxDetails = await GetBarcodeFromPackingSlipAsync(packingslipId, command.Connection, command.Transaction);
                    var unScannedBoxes = packingSlipBoxDetails.Where(x => x.IsScanned == false).FirstOrDefault();

                    if (unScannedBoxes == null)
                    {
                        sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [IsScanned] = '{true}'  WHERE id = '{packingslipId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();


                        var userActivityReport = new UserActivityReport();
                        userActivityReport.UserId = userId;
                        userActivityReport.Module = BusinessConstants.MODULE.PACKING_SLIP.ToString();
                        userActivityReport.Action = BusinessConstants.ACTION.SCAN_SHIPMENT.ToString();
                        userActivityReport.Reference = packingSlip.PackingSlipNo;
                        userActivityReport.Description = "Shipment is ready for trucking";
                        await this.userActivityReportRepository.AddActivityAsync(userActivityReport, connection, transaction, command);
                    }



                    transaction.Commit();
                    connection.Close();
                    packingSlip = await GetPackingSlipAsync(packingslipId);
                    var packingboxestatuses = new List<PackingSlipScanBoxeStatus>();
                    foreach (PackingSlipDetails packingSlipDetails in packingSlip.PackingSlipDetails)
                    {
                        foreach (PackingSlipBoxDetails packingSlipBox in packingSlipDetails.PackingSlipBoxDetails)
                        {
                            var part = await partRepository.GetPartAsync(packingSlipBox.PartId);

                            var packingboxestatus = new PackingSlipScanBoxeStatus();
                            packingboxestatus.Id = packingSlipBox.Id;
                            packingboxestatus.PackingSlipId = packingSlipBox.PackingSlipId;
                            packingboxestatus.PackingSlipNo = packingSlip.PackingSlipNo;
                            packingboxestatus.PackingSlipDetailId = packingSlipBox.PackingSlipDetailId;
                            packingboxestatus.LineNumber = packingSlipDetails.LineNumber;
                            packingboxestatus.PartId = packingSlipBox.PartId;
                            packingboxestatus.PartCode = part.Code;
                            packingboxestatus.Qty = packingSlipBox.Qty;
                            packingboxestatus.TotalBox = packingSlipDetails.Boxes;
                            packingboxestatus.BoxeNo = packingSlipBox.BoxeNo;
                            packingboxestatus.Barcode = packingSlipBox.Barcode;
                            packingboxestatus.IsScanned = packingSlipBox.IsScanned;

                            packingboxestatuses.Add(packingboxestatus);
                        }
                    }
                    return packingboxestatuses;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return null;
                }

            }
        }

        public async Task<PackingSlip> GetPackingSlipFromBarcodeAsync(string barcode)
        {
            var packingslipId = await GetPackingslipFromBarcodeAsync(barcode);
            return await GetPackingSlipAsync(packingslipId);
        }

        public async Task<bool> ScanAutoPackingSlip(int packingSlipId, int userId)
        {
            var packingSlip = await GetPackingSlipAsync(packingSlipId);
            //throw new NotImplementedException();
            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {

                    var sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [IsScanned] = '{true}',  [IsShipmentVerified] = '{true}' WHERE id = '{packingSlipId}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();


                    var userActivityReport = new UserActivityReport();
                    userActivityReport.UserId = userId;
                    userActivityReport.Module = BusinessConstants.MODULE.PACKING_SLIP.ToString();
                    userActivityReport.Action = BusinessConstants.ACTION.SCAN_SHIPMENT.ToString();
                    userActivityReport.Reference = packingSlip.PackingSlipNo;
                    userActivityReport.Description = "Shipment is ready for trucking. This shipement has been verified and scanned automatically.";
                    await this.userActivityReportRepository.AddActivityAsync(userActivityReport, connection, transaction, command);

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<int> GetPackingslipFromBarcodeAsync(string barcode, SqlConnection conn, SqlTransaction transaction)
        {
            int packingSlipId = 0;
            var commandText = string.Format($"SELECT  [PackingSlipId]  FROM [dbo].[PackingSlipBoxDetails] where Barcode = '{barcode}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                while (dataReader.Read())
                {
                    packingSlipId = Convert.ToInt32(dataReader["PackingSlipId"]);
                }

                dataReader.Close();
            }

            return packingSlipId;
        }

        public async Task<int> GetPackingslipFromBarcodeAsync(string barcode)
        {
            int packingSlipId = 0;
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT  [PackingSlipId]  FROM [dbo].[PackingSlipBoxDetails] where Barcode = '{barcode}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    packingSlipId = Convert.ToInt32(dataReader["PackingSlipId"]);
                }
                dataReader.Close();
                conn.Close();
            }

            return packingSlipId;
        }

        public async Task<IEnumerable<PackingSlipBoxDetails>> GetBarcodeFromPackingSlipAsync(int packingSlipId, SqlConnection conn, SqlTransaction transaction)
        {
            var packingSlipBoxDetails = new List<PackingSlipBoxDetails>();

            var commandText = string.Format($"SELECT  [Id] ,[PackingSlipId] ,[PackingSlipDetailId] ,[PartId] ,[Qty],[BoxeNo] ,[Barcode] ,[IsScanned] FROM [dbo].[PackingSlipBoxDetails] where PackingSlipId = '{packingSlipId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                while (dataReader.Read())
                {
                    var packingSlipBoxDetail = new PackingSlipBoxDetails();
                    packingSlipBoxDetail.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlipBoxDetail.PackingSlipId = Convert.ToInt32(dataReader["PackingSlipId"]);
                    packingSlipBoxDetail.PackingSlipDetailId = Convert.ToInt32(dataReader["PackingSlipDetailId"]);
                    packingSlipBoxDetail.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlipBoxDetail.Qty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlipBoxDetail.BoxeNo = Convert.ToInt32(dataReader["BoxeNo"]);
                    packingSlipBoxDetail.Barcode = Convert.ToString(dataReader["Barcode"]);
                    packingSlipBoxDetail.IsScanned = Convert.ToBoolean(dataReader["IsScanned"]);

                    packingSlipBoxDetails.Add(packingSlipBoxDetail);
                }

                dataReader.Close();
            }
            return packingSlipBoxDetails;
        }

        public async Task<IEnumerable<PackingSlipBoxDetails>> GetBoxDetailFromBarcodeAsync(string barcode, SqlConnection conn, SqlTransaction transaction)
        {
            var packingSlipBoxDetails = new List<PackingSlipBoxDetails>();

            var commandText = string.Format($"SELECT  [Id] ,[PackingSlipId] ,[PackingSlipDetailId] ,[PartId] ,[Qty],[BoxeNo] ,[Barcode] ,[IsScanned] FROM [dbo].[PackingSlipBoxDetails] where Barcode = '{barcode}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                while (dataReader.Read())
                {
                    var packingSlipBoxDetail = new PackingSlipBoxDetails();
                    packingSlipBoxDetail.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlipBoxDetail.PackingSlipId = Convert.ToInt32(dataReader["PackingSlipId"]);
                    packingSlipBoxDetail.PackingSlipDetailId = Convert.ToInt32(dataReader["PackingSlipDetailId"]);
                    packingSlipBoxDetail.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlipBoxDetail.Qty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlipBoxDetail.BoxeNo = Convert.ToInt32(dataReader["BoxeNo"]);
                    packingSlipBoxDetail.Barcode = Convert.ToString(dataReader["Barcode"]);
                    packingSlipBoxDetail.IsScanned = Convert.ToBoolean(dataReader["IsScanned"]);

                    packingSlipBoxDetails.Add(packingSlipBoxDetail);
                }

                dataReader.Close();
            }
            return packingSlipBoxDetails;
        }

        public async Task<IEnumerable<PackingSlipBoxDetails>> GetPackingSlipBoxDetailsAsyncAsync(int packingSlipId)
        {
            var packingSlipBoxDetails = new List<PackingSlipBoxDetails>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT  [Id] ,[PackingSlipId] ,[PackingSlipDetailId] ,[PartId] ,[Qty],[BoxeNo] ,[Barcode] ,[IsScanned] FROM [dbo].[PackingSlipBoxDetails] where PackingSlipId = '{packingSlipId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlipBoxDetail = new PackingSlipBoxDetails();
                    packingSlipBoxDetail.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlipBoxDetail.PackingSlipId = Convert.ToInt32(dataReader["PackingSlipId"]);
                    packingSlipBoxDetail.PackingSlipDetailId = Convert.ToInt32(dataReader["PackingSlipDetailId"]);
                    packingSlipBoxDetail.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlipBoxDetail.Qty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlipBoxDetail.BoxeNo = Convert.ToInt32(dataReader["BoxeNo"]);
                    packingSlipBoxDetail.Barcode = Convert.ToString(dataReader["Barcode"]);
                    packingSlipBoxDetail.IsScanned = Convert.ToBoolean(dataReader["IsScanned"]);

                    packingSlipBoxDetails.Add(packingSlipBoxDetail);

                }
                dataReader.Close();
                conn.Close();
            }

            return packingSlipBoxDetails;
        }

        public async Task<bool> AllowScanning(int packingSlipId, int userId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSettings.ConnectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connection.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    string sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [AllowScanning] = '{true}'   WHERE Id = '{packingSlipId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }



        //public async Task<Customer> GetIdByAccessIdAsync(string accessId)
        //{

        //    int id = 0;
        //    int customerId = 0;
        //    SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

        //    var commandText = string.Format($"SELECT [Id],[CustomerId]  FROM [dbo].[PackingSlipMaster] WHERE  [AccessId] = = '{id}' ");

        //    using (SqlCommand cmd = new SqlCommand(commandText, conn))
        //    {
        //        cmd.CommandType = CommandType.Text;

        //        conn.Open();

        //        var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

        //        while (dataReader.Read())
        //        {
        //            id = Convert.ToInt32(dataReader["Id"]);
        //            customerId = Convert.ToInt32(dataReader["CustomerId"]);
        //        }
        //        dataReader.Close();
        //        conn.Close();
        //    }
        //    var customer = await this.customerRepository.GetCustomerAsync(customerId);
        //    customer.PackingSlipId = id;
        //    return customer;
        //}

        public async Task<int> GetIdByAccessIdAsync(string accessId)
        {

            int id = 0;
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id]  FROM [dbo].[PackingSlipMaster] WHERE  [AccessId] = '{accessId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    id = Convert.ToInt32(dataReader["Id"]);
                }
                dataReader.Close();
                conn.Close();
            }
            return id;
        }
    }
}
