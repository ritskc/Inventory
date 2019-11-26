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
    public class SupplierInvoiceRepository : ISupplierInvoiceRepository
    {
        private readonly ISqlHelper _sqlHelper;
        private readonly IPoRepository _poRepository;
        private readonly ITransactionRepository _transactionRepository;

        public SupplierInvoiceRepository(ISqlHelper sqlHelper, IPoRepository poRepository, ITransactionRepository transactionRepository)
        {
            _sqlHelper = sqlHelper;
            _poRepository = poRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<Int64> AddSupplierInvoiceAsync(SupplierInvoice supplierInvoice)
        {
            Int64 supplierInvoiceId;
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
                    string sql = string.Format($"INSERT INTO [dbo].[SupplierInvoiceMaster]   ([CompanyId]   ,[SupplierId]   ,[InvoiceNo]   ,[InvoiceDate]   ,[ETA]   ,[IsAirShipment]   ,[PoNo]   ,[ReferenceNo]   ,[Email]   ,[ByCourier]   ,[IsInvoiceUploaded]   ,[IsPackingSlipUploaded]   ,[IsTenPlusUploaded]   ,[IsBLUploaded]   ,[IsTCUploaded]   ,[InvoicePath]   ,[PackingSlipPath]   ,[TenPlusPath]   ,[BLPath]   ,[IsInvoiceReceived]   ,[UploadedDate]   ,[ReceivedDate]) VALUES   ('{supplierInvoice.CompanyId}'   ,'{supplierInvoice.SupplierId}'   ,'{supplierInvoice.InvoiceNo}'   ,'{supplierInvoice.InvoiceDate}'   ,'{supplierInvoice.ETA}'   ,'{supplierInvoice.IsAirShipment}'   ,'{supplierInvoice.PoNo}'   ,'{supplierInvoice.ReferenceNo}'   ,'{supplierInvoice.Email}'   ,'{supplierInvoice.ByCourier}'   ,'{supplierInvoice.IsInvoiceUploaded}'   ,'{supplierInvoice.IsPackingSlipUploaded}'   ,'{supplierInvoice.IsTenPlusUploaded}'   ,'{supplierInvoice.IsBLUploaded}'   ,'{supplierInvoice.IsTCUploaded}'   ,'{supplierInvoice.InvoicePath}'   ,'{supplierInvoice.PackingSlipPath}'   ,'{supplierInvoice.TenPlusPath}'   ,'{supplierInvoice.BLPath}'   ,'{supplierInvoice.IsInvoiceReceived}'   ,'{supplierInvoice.UploadedDate}'   ,'{supplierInvoice.ReceivedDate}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;
                    var invoiceId = command.ExecuteScalar();
                    supplierInvoice.Id = Convert.ToInt64(invoiceId.ToString());
                    supplierInvoiceId = supplierInvoice.Id;

                    foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[SupplierInvoiceDetails]   ([InvoiceId]   ,[SrNo]   ,[PartId]   ,[Qty]   ,[Price]   ,[Total]   ,[AdjustedPOQty]   ,[ExcessQty]   ,[BoxNo],[IsBoxReceived]) VALUES   ('{invoiceId}'   ,'{supplierInvoiceDetail.SrNo}'   ,'{supplierInvoiceDetail.PartId}'   ,'{supplierInvoiceDetail.Qty}'   ,'{supplierInvoiceDetail.Price}'   ,'{supplierInvoiceDetail.Total}'   ,'{supplierInvoiceDetail.AdjustedPOQty}'   ,'{supplierInvoiceDetail.ExcessQty}'   ,'{supplierInvoiceDetail.BoxNo}','{false}')");

                        sql = sql + " Select Scope_Identity()";
                        command.CommandText = sql;
                        supplierInvoiceDetail.Id = Convert.ToInt32(command.ExecuteScalar());

                        foreach (SupplierInvoicePoDetails supplierInvoicePoDetails in supplierInvoiceDetail.supplierInvoicePoDetails)
                        {
                            supplierInvoicePoDetails.InvoiceDetailId = supplierInvoiceDetail.Id;

                            sql = string.Format($"INSERT INTO [dbo].[SupplierInvoicePoDetails]   ([PartId],[InvoiceId],[InvoiceDetailId],[PoId],[PODetailId],[PONo],[Qty]) VALUES   ('{supplierInvoicePoDetails.PartId}', '{invoiceId}','{supplierInvoicePoDetails.InvoiceDetailId}','{supplierInvoicePoDetails.PoId}','{supplierInvoicePoDetails.PODetailId}'   ,'{supplierInvoicePoDetails.PONo}'   ,'{supplierInvoicePoDetails.Qty}')");

                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();

                            var poResult = await _poRepository.GetPoAsync(supplierInvoicePoDetails.PoId);

                            var poDetail = poResult.poDetails.Where(x => x.Id == supplierInvoicePoDetails.PODetailId).FirstOrDefault();

                            var calQty = supplierInvoicePoDetails.Qty + poDetail.InTransitQty + poDetail.ReceivedQty;
                            if (poDetail.AckQty <= calQty)
                            {
                                sql = string.Format($"UPDATE [dbo].[PoDetails]   SET [InTransitQty] = '{supplierInvoicePoDetails.Qty + poDetail.InTransitQty}' ,[IsClosed] = '{true}' ,[ClosingDate] = '{DateTime.Now}'  WHERE id = '{supplierInvoicePoDetails.PODetailId}' ");
                            }
                            else
                            {
                                sql = string.Format($"UPDATE [dbo].[PoDetails]   SET [InTransitQty] = '{supplierInvoicePoDetails.Qty + poDetail.InTransitQty}'  WHERE id = '{supplierInvoicePoDetails.PODetailId}' ");
                            }
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                            //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                            poResult = await _poRepository.GetPoAsync(supplierInvoicePoDetails.PoId);
                            var openPO = poResult.poDetails.Where(x => x.PoId == supplierInvoicePoDetails.PoId && x.IsClosed == false).FirstOrDefault();

                            if (openPO == null)
                            {
                                sql = string.Format($"UPDATE [dbo].[PoMaster]   SET [IsClosed] = '{true}' ,[ClosingDate] = '{DateTime.Now}'  WHERE id = '{supplierInvoicePoDetails.PoId}' ");
                                command.CommandText = sql;
                                await command.ExecuteNonQueryAsync();
                                //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                            }
                        }

                        sql = string.Format($"UPDATE [dbo].[part]   SET [IntransitQty] =  IntransitQty + '{supplierInvoiceDetail.Qty}'  WHERE id = '{supplierInvoiceDetail.PartId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{supplierInvoiceDetail.PartId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.UPLOAD_SUPPLIER_INVOICE)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.IN)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.INTRANSIT_QTY)}'   ,'{invoiceId.ToString()}'   ,'{supplierInvoiceDetail.Qty}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
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
                    var sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]   SET [Barcode] =  '{BarCodeUtil.GetBarCodeString(supplierInvoice.Id)}' WHERE id = '{supplierInvoice.Id}' ");
                    await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            return supplierInvoiceId;
        }

        public async Task<bool> DeleteSupplierInvoiceAsync(long supplierInvoiceId)
        {
            //1. Get SupplierInvoicePoDetails po transaction detail
            //2. update PODetail with qty and IsClose status
            //3. update POMaster IsClose status
            //4. delete from SupplierInvoiceDetails
            //5. delete from SupplierInvoiceMaster
            //6. delete from SupplierInvoicePoDetails
            //7. adjust the inventory

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

                    var commandText = string.Format($"SELECT  [Id]  ,[PartId] ,[InvoiceId] ,[InvoiceDetailId] ,[POId] ,[PODetailId] ,[PONo] ,[Qty]" +
                   $" FROM [SupplierInvoicePoDetails] where InvoiceId = '{supplierInvoiceId}' ");

                    using (SqlCommand cmd = new SqlCommand(commandText, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        conn.Open();

                        var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                        while (dataReader.Read())
                        {
                            var supplierInvoicePoDetail = new SupplierInvoicePoDetails();
                            supplierInvoicePoDetail.Id = Convert.ToInt64(dataReader["Id"]);
                            supplierInvoicePoDetail.PartId = Convert.ToInt32(dataReader["PartId"]);
                            supplierInvoicePoDetail.InvoiceId = Convert.ToInt32(dataReader["InvoiceId"]);
                            supplierInvoicePoDetail.InvoiceDetailId = Convert.ToInt32(dataReader["InvoiceDetailId"]);
                            supplierInvoicePoDetail.PoId = Convert.ToInt32(dataReader["POId"]);
                            supplierInvoicePoDetail.PODetailId = Convert.ToInt32(dataReader["PODetailId"]);
                            supplierInvoicePoDetail.PONo = Convert.ToString(dataReader["PONo"]);
                            supplierInvoicePoDetail.Qty = Convert.ToInt32(dataReader["Qty"]);

                            supplierInvoicePoDetailsList.Add(supplierInvoicePoDetail);
                        }
                        conn.Close();
                    }

                    //long invoiceId = supplierInvoicePoDetailsList.Select(x => x.InvoiceId).FirstOrDefault();
                    long poId = supplierInvoicePoDetailsList.Select(x => x.PoId).FirstOrDefault();
                    foreach (SupplierInvoicePoDetails supplierInvoicePoDetails in supplierInvoicePoDetailsList)
                    {
                        sql = string.Format($"UPDATE [dbo].[PoDetails]   SET [InTransitQty] = [InTransitQty] - '{supplierInvoicePoDetails.Qty}' ,[IsClosed] = '{false}' ,[ClosingDate] = '{null}'  WHERE id = '{supplierInvoicePoDetails.PODetailId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"UPDATE [dbo].[PoMaster]   SET [IsClosed] = '{false}' ,[ClosingDate] = '{null}'  WHERE id = '{poId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                    }

                    commandText = string.Format($"SELECT [Id] ,[InvoiceId] ,[SrNo] ,[PartId] ,[Qty],[Price],[Total] ,[AdjustedPOQty],[ExcessQty],[BoxNo],[Barcode]," +
                        $"[IsBoxReceived] FROM [SupplierInvoiceDetails] where InvoiceId = '{supplierInvoiceId}' ");
                    List<SupplierInvoiceDetail> supplierInvoiceDetails = new List<SupplierInvoiceDetail>();
                    using (SqlCommand cmd = new SqlCommand(commandText, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        conn.Open();

                        var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                        while (dataReader.Read())
                        {
                            var supplierInvoiceDetail = new SupplierInvoiceDetail();
                            supplierInvoiceDetail.Id = Convert.ToInt64(dataReader["Id"]);
                            supplierInvoiceDetail.InvoiceId = Convert.ToInt32(dataReader["InvoiceId"]);
                            supplierInvoiceDetail.SrNo = Convert.ToInt32(dataReader["SrNo"]);
                            supplierInvoiceDetail.PartId = Convert.ToInt32(dataReader["PartId"]);
                            supplierInvoiceDetail.Qty = Convert.ToInt32(dataReader["Qty"]);
                            supplierInvoiceDetail.Price = Convert.ToInt32(dataReader["Price"]);
                            supplierInvoiceDetail.Total = Convert.ToDecimal(dataReader["Total"]);
                            supplierInvoiceDetail.AdjustedPOQty = Convert.ToInt32(dataReader["AdjustedPOQty"]);
                            supplierInvoiceDetail.ExcessQty = Convert.ToInt32(dataReader["ExcessQty"]);
                            supplierInvoiceDetail.BoxNo = Convert.ToInt32(dataReader["BoxNo"]);
                            supplierInvoiceDetail.Barcode = Convert.ToString(dataReader["Barcode"]);                            
                            if (dataReader["IsBoxReceived"] != DBNull.Value)
                                supplierInvoiceDetail.IsBoxReceived = Convert.ToBoolean(dataReader["IsBoxReceived"]);
                            else
                                supplierInvoiceDetail.IsBoxReceived = false;
                            
                            supplierInvoiceDetails.Add(supplierInvoiceDetail);
                        }
                        conn.Close();
                    }                    

                    foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoiceDetails)
                    {
                        sql = string.Format($"UPDATE [dbo].[part]   SET [IntransitQty] =  IntransitQty - '{supplierInvoiceDetail.Qty}'  WHERE id = '{supplierInvoiceDetail.PartId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{supplierInvoiceDetail.PartId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.REVERT_UPLOAD_SUPPLIER_INVOICE)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.OUT)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.INTRANSIT_QTY)}'   ,'{supplierInvoiceId.ToString()}'   ,'{supplierInvoiceDetail.Qty}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                    }


                    sql = string.Format($"DELETE FROM[dbo].[SupplierInvoiceDetails]  WHERE invoiceId = '{supplierInvoiceId}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                    //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                    sql = string.Format($"DELETE FROM[dbo].[SupplierInvoiceMaster]  WHERE id = '{supplierInvoiceId}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                    //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

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

        public async Task<IEnumerable<SupplierInvoice>> GetAllSupplierInvoicesAsync(int companyId)
        {
            List<SupplierInvoice> supplierInvoices = new List<SupplierInvoice>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
                $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
                $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate],[Barcode]  FROM [SupplierInvoiceMaster] where CompanyId = '{companyId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var supplierInvoice = new SupplierInvoice();
                    supplierInvoice.Id = Convert.ToInt64(dataReader["Id"]);
                    supplierInvoice.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    supplierInvoice.SupplierId = Convert.ToInt32(dataReader["SupplierId"]);
                    supplierInvoice.InvoiceNo = Convert.ToString(dataReader["InvoiceNo"]);
                    supplierInvoice.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    supplierInvoice.ETA = Convert.ToDateTime(dataReader["ETA"]);
                    supplierInvoice.IsAirShipment = Convert.ToBoolean(dataReader["IsAirShipment"]);
                    supplierInvoice.PoNo = Convert.ToString(dataReader["PoNo"]);
                    supplierInvoice.ReferenceNo = Convert.ToString(dataReader["ReferenceNo"]);
                    supplierInvoice.Email = Convert.ToString(dataReader["Email"]);
                    supplierInvoice.ByCourier = Convert.ToBoolean(dataReader["ByCourier"]);
                    supplierInvoice.IsInvoiceUploaded = Convert.ToBoolean(dataReader["IsInvoiceUploaded"]);
                    supplierInvoice.IsPackingSlipUploaded = Convert.ToBoolean(dataReader["IsPackingSlipUploaded"]);
                    supplierInvoice.IsTenPlusUploaded = Convert.ToBoolean(dataReader["IsTenPlusUploaded"]);
                    supplierInvoice.IsBLUploaded = Convert.ToBoolean(dataReader["IsBLUploaded"]);
                    supplierInvoice.IsTCUploaded = Convert.ToBoolean(dataReader["IsTCUploaded"]);
                    supplierInvoice.InvoicePath = Convert.ToString(dataReader["InvoicePath"]);
                    supplierInvoice.PackingSlipPath = Convert.ToString(dataReader["PackingSlipPath"]);
                    supplierInvoice.TenPlusPath = Convert.ToString(dataReader["TenPlusPath"]);
                    supplierInvoice.BLPath = Convert.ToString(dataReader["BLPath"]);
                    supplierInvoice.IsInvoiceReceived = Convert.ToBoolean(dataReader["IsInvoiceReceived"]);
                    supplierInvoice.UploadedDate = Convert.ToDateTime(dataReader["UploadedDate"]);
                    supplierInvoice.ReceivedDate = Convert.ToDateTime(dataReader["ReceivedDate"]);
                    supplierInvoice.Barcode = DBNull.Value.Equals(dataReader["Barcode"]) ? string.Empty : Convert.ToString(dataReader["Barcode"]);
                    supplierInvoices.Add(supplierInvoice);
                }
                conn.Close();
            }

            foreach (SupplierInvoice supplierInvoice in supplierInvoices)
            {
                List<SupplierInvoiceDetail> supplierInvoiceDetails = new List<SupplierInvoiceDetail>();
                commandText = string.Format($"SELECT  [Id] ,[InvoiceId] ,[SrNo] ,[PartId] ,[Qty] ,[Price] ,[Total] ,[AdjustedPOQty] ,[ExcessQty] ,[BoxNo],[Barcode]  FROM [dbo].[SupplierInvoiceDetails] where InvoiceId = '{ supplierInvoice.Id}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var supplierInvoiceDetail = new SupplierInvoiceDetail();
                        supplierInvoiceDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                        supplierInvoiceDetail.InvoiceId = Convert.ToInt64(dataReader1["InvoiceId"]);
                        supplierInvoiceDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                        supplierInvoiceDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                        supplierInvoiceDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                        supplierInvoiceDetail.Price = Convert.ToDecimal(dataReader1["Price"]);
                        supplierInvoiceDetail.Total = Convert.ToDecimal(dataReader1["Total"]);
                        supplierInvoiceDetail.AdjustedPOQty = Convert.ToInt32(dataReader1["AdjustedPOQty"]);
                        supplierInvoiceDetail.ExcessQty = Convert.ToInt32(dataReader1["ExcessQty"]);
                        supplierInvoiceDetail.BoxNo = Convert.ToInt32(dataReader1["BoxNo"]);
                        supplierInvoiceDetail.Barcode = DBNull.Value.Equals(dataReader1["Barcode"]) ? string.Empty : Convert.ToString(dataReader1["Barcode"]);
                        supplierInvoiceDetails.Add(supplierInvoiceDetail);
                    }
                }
                supplierInvoice.supplierInvoiceDetails = supplierInvoiceDetails;
                conn.Close();
            }

            foreach (SupplierInvoice supplierInvoice in supplierInvoices)
            {
                foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                {
                    List<SupplierInvoicePoDetails> supplierInvoicePoDetails = new List<SupplierInvoicePoDetails>();
                    commandText = string.Format("SELECT  [Id] ,[PartId] ,[InvoiceId] ,[InvoiceDetailId] ,[POId] ,[PODetailId] ,[PONo] ,[Qty]  FROM [dbo].[SupplierInvoicePoDetails] " +
                        "where InvoiceDetailId = '{0}'", supplierInvoiceDetail.Id);

                    using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                    {
                        cmd1.CommandType = CommandType.Text;
                        conn.Open();
                        var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                        while (dataReader1.Read())
                        {
                            var supplierInvoicePoDetail = new SupplierInvoicePoDetails();
                            supplierInvoicePoDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                            supplierInvoicePoDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                            supplierInvoicePoDetail.InvoiceId = Convert.ToInt64(dataReader1["InvoiceId"]);
                            supplierInvoicePoDetail.InvoiceDetailId = Convert.ToInt64(dataReader1["InvoiceDetailId"]);
                            supplierInvoicePoDetail.PoId = Convert.ToInt64(dataReader1["POId"]);
                            supplierInvoicePoDetail.PODetailId = Convert.ToInt64(dataReader1["PODetailId"]);
                            supplierInvoicePoDetail.PONo = Convert.ToString(dataReader1["PONo"]);
                            supplierInvoicePoDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);

                            supplierInvoicePoDetails.Add(supplierInvoicePoDetail);
                        }
                    }
                    supplierInvoiceDetail.supplierInvoicePoDetails = supplierInvoicePoDetails;
                    conn.Close();
                }

            }

            return supplierInvoices;
        }

        public async Task<SupplierInvoice> GetSupplierInvoiceAsync(long supplierInvoiceId)
        {
            SupplierInvoice supplierInvoice = new SupplierInvoice();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
                $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
                $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate],[Barcode]  FROM [SupplierInvoiceMaster] where Id = '{supplierInvoiceId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    supplierInvoice.Id = Convert.ToInt64(dataReader["Id"]);
                    supplierInvoice.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    supplierInvoice.SupplierId = Convert.ToInt32(dataReader["SupplierId"]);
                    supplierInvoice.InvoiceNo = Convert.ToString(dataReader["InvoiceNo"]);
                    supplierInvoice.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    supplierInvoice.ETA = Convert.ToDateTime(dataReader["ETA"]);
                    supplierInvoice.IsAirShipment = Convert.ToBoolean(dataReader["IsAirShipment"]);
                    supplierInvoice.PoNo = Convert.ToString(dataReader["PoNo"]);
                    supplierInvoice.ReferenceNo = Convert.ToString(dataReader["ReferenceNo"]);
                    supplierInvoice.Email = Convert.ToString(dataReader["Email"]);
                    supplierInvoice.ByCourier = Convert.ToBoolean(dataReader["ByCourier"]);
                    supplierInvoice.IsInvoiceUploaded = Convert.ToBoolean(dataReader["IsInvoiceUploaded"]);
                    supplierInvoice.IsPackingSlipUploaded = Convert.ToBoolean(dataReader["IsPackingSlipUploaded"]);
                    supplierInvoice.IsTenPlusUploaded = Convert.ToBoolean(dataReader["IsTenPlusUploaded"]);
                    supplierInvoice.IsBLUploaded = Convert.ToBoolean(dataReader["IsBLUploaded"]);
                    supplierInvoice.IsTCUploaded = Convert.ToBoolean(dataReader["IsTCUploaded"]);
                    supplierInvoice.InvoicePath = Convert.ToString(dataReader["InvoicePath"]);
                    supplierInvoice.PackingSlipPath = Convert.ToString(dataReader["PackingSlipPath"]);
                    supplierInvoice.TenPlusPath = Convert.ToString(dataReader["TenPlusPath"]);
                    supplierInvoice.BLPath = Convert.ToString(dataReader["BLPath"]);
                    supplierInvoice.IsInvoiceReceived = Convert.ToBoolean(dataReader["IsInvoiceReceived"]);
                    supplierInvoice.UploadedDate = Convert.ToDateTime(dataReader["UploadedDate"]);
                    supplierInvoice.ReceivedDate = Convert.ToDateTime(dataReader["ReceivedDate"]);
                    supplierInvoice.Barcode = DBNull.Value.Equals(dataReader["Barcode"]) ? string.Empty : Convert.ToString(dataReader["Barcode"]);
                }
                conn.Close();
            }


            List<SupplierInvoiceDetail> supplierInvoiceDetails = new List<SupplierInvoiceDetail>();
            commandText = string.Format($"SELECT  [Id] ,[InvoiceId] ,[SrNo] ,[PartId] ,[Qty] ,[Price] ,[Total] ,[AdjustedPOQty] ,[ExcessQty] ,[BoxNo],[Barcode]  FROM [dbo].[SupplierInvoiceDetails] where InvoiceId = '{ supplierInvoice.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var supplierInvoiceDetail = new SupplierInvoiceDetail();
                    supplierInvoiceDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                    supplierInvoiceDetail.InvoiceId = Convert.ToInt64(dataReader1["InvoiceId"]);
                    supplierInvoiceDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                    supplierInvoiceDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                    supplierInvoiceDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                    supplierInvoiceDetail.Price = Convert.ToDecimal(dataReader1["Price"]);
                    supplierInvoiceDetail.Total = Convert.ToDecimal(dataReader1["Total"]);
                    supplierInvoiceDetail.AdjustedPOQty = Convert.ToInt32(dataReader1["AdjustedPOQty"]);
                    supplierInvoiceDetail.ExcessQty = Convert.ToInt32(dataReader1["ExcessQty"]);
                    supplierInvoiceDetail.BoxNo = Convert.ToInt32(dataReader1["BoxNo"]);
                    supplierInvoiceDetail.Barcode = DBNull.Value.Equals(dataReader1["Barcode"]) ? string.Empty : Convert.ToString(dataReader1["Barcode"]);
                    supplierInvoiceDetails.Add(supplierInvoiceDetail);
                }
            }
            supplierInvoice.supplierInvoiceDetails = supplierInvoiceDetails;
            conn.Close();



            foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
            {
                List<SupplierInvoicePoDetails> supplierInvoicePoDetails = new List<SupplierInvoicePoDetails>();
                commandText = string.Format("SELECT  [Id] ,[PartId] ,[InvoiceId] ,[InvoiceDetailId] ,[POId] ,[PODetailId] ,[PONo] ,[Qty]  FROM [dbo].[SupplierInvoicePoDetails] " +
                    "where InvoiceDetailId = '{0}'", supplierInvoiceDetail.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var supplierInvoicePoDetail = new SupplierInvoicePoDetails();
                        supplierInvoicePoDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                        supplierInvoicePoDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                        supplierInvoicePoDetail.InvoiceId = Convert.ToInt64(dataReader1["InvoiceId"]);
                        supplierInvoicePoDetail.InvoiceDetailId = Convert.ToInt64(dataReader1["InvoiceDetailId"]);
                        supplierInvoicePoDetail.PoId = Convert.ToInt64(dataReader1["POId"]);
                        supplierInvoicePoDetail.PODetailId = Convert.ToInt64(dataReader1["PODetailId"]);
                        supplierInvoicePoDetail.PONo = Convert.ToString(dataReader1["PONo"]);
                        supplierInvoicePoDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);

                        supplierInvoicePoDetails.Add(supplierInvoicePoDetail);
                    }
                }
                supplierInvoiceDetail.supplierInvoicePoDetails = supplierInvoicePoDetails;
                conn.Close();
            }

            return supplierInvoice;
        }

        public async Task ReceiveSupplierInvoiceAsync(long supplierInvoiceId)
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
                    var sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]   SET [IsInvoiceReceived] = '{true}'   ,[ReceivedDate] = '{DateTime.Now}' WHERE Id = '{supplierInvoiceId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();


                    sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [IsBoxReceived] = '{true}'   WHERE [InvoiceId] = '{supplierInvoiceId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                    // Attempt to commit the transaction.
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public async Task ReceiveBoxInvoiceAsync(string barcode)
        {
            List<SupplierInvoiceDetail> supplierInvoiceDetails = new List<SupplierInvoiceDetail>();
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
                    var sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [IsBoxReceived] = '{true}'   WHERE [Barcode] = '{barcode}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                    // Attempt to commit the transaction.
                    transaction.Commit();

                    connection.Close();

                    var commandText = string.Format($"SELECT  *  FROM [dbo].[SupplierInvoiceDetails] where InvoiceId in (select invoiceid from [SupplierInvoiceDetails] where Barcode ='{barcode}')");

                    using (SqlCommand cmd1 = new SqlCommand(commandText, connection))
                    {
                        cmd1.CommandType = CommandType.Text;
                        connection.Open();
                        var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                        while (dataReader1.Read())
                        {
                            var supplierInvoiceDetail = new SupplierInvoiceDetail();
                            supplierInvoiceDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                            supplierInvoiceDetail.InvoiceId = Convert.ToInt64(dataReader1["InvoiceId"]);
                            supplierInvoiceDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                            supplierInvoiceDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                            supplierInvoiceDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                            supplierInvoiceDetail.Price = Convert.ToDecimal(dataReader1["Price"]);
                            supplierInvoiceDetail.Total = Convert.ToDecimal(dataReader1["Total"]);
                            supplierInvoiceDetail.AdjustedPOQty = Convert.ToInt32(dataReader1["AdjustedPOQty"]);
                            supplierInvoiceDetail.ExcessQty = Convert.ToInt32(dataReader1["ExcessQty"]);
                            supplierInvoiceDetail.BoxNo = Convert.ToInt32(dataReader1["BoxNo"]);
                            supplierInvoiceDetail.IsBoxReceived = DBNull.Value.Equals(dataReader1["IsBoxReceived"]) ? false : Convert.ToBoolean(dataReader1["IsBoxReceived"]);
                            supplierInvoiceDetails.Add(supplierInvoiceDetail);
                        }
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

            if (supplierInvoiceDetails.Count == supplierInvoiceDetails.Where(x => x.IsBoxReceived == true).Count())
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
                        var sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]   SET [IsInvoiceReceived] = '{true}'   ,[ReceivedDate] = '{DateTime.Now}' WHERE Id = '{supplierInvoiceDetails.FirstOrDefault().InvoiceId}'");
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

        }

        public Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice)
        {
            throw new NotImplementedException();
        }

        public async Task UploadFileAsync(int id, string docType, string path)
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
                    string sql = string.Empty;
                    switch (docType)
                    {
                        case "Invoice":
                            sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]   SET [IsInvoiceUploaded] = '{true}' ,[InvoicePath] = '{path}'  WHERE Id = '{id}'");
                            break;

                        case "PackingSlip":
                            sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]   SET [IsPackingSlipUploaded] = '{true}' ,[PackingSlipPath] = '{path}'  WHERE Id = '{id}'");
                            break;

                        case "TenPlus":
                            sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]   SET [IsTenPlusUploaded] = '{true}' ,[TenPlusPath] = '{path}'  WHERE Id = '{id}'");
                            break;

                        case "BL":
                            sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]   SET [IsBLUploaded] = '{true}' ,[BLPath] = '{path}'  WHERE Id = '{id}'");
                            break;
                    }
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
    }
}
