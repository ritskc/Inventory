﻿using DAL.DBHelper;
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
        private readonly IUserRepository userRepository;

        public SupplierInvoiceRepository(ISqlHelper sqlHelper, IPoRepository poRepository, 
            ITransactionRepository transactionRepository,IUserRepository userRepository)
        {
            _sqlHelper = sqlHelper;
            _poRepository = poRepository;
            _transactionRepository = transactionRepository;
            this.userRepository = userRepository;
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
                        if(supplierInvoice.DontImpactPO)
                        {
                            supplierInvoiceDetail.AdjustedPOQty = 0;
                            supplierInvoiceDetail.ExcessQty = supplierInvoiceDetail.Qty;

                        }
                        sql = string.Format($"INSERT INTO [dbo].[SupplierInvoiceDetails]   ([InvoiceId]   ,[SrNo]   ,[PartId]   ,[Qty]   ,[Price]   ,[Total]   ,[AdjustedPOQty]   ,[ExcessQty]   ,[BoxNo],[IsBoxReceived],[AdjustedInvoiceQty]) VALUES   ('{invoiceId}'   ,'{supplierInvoiceDetail.SrNo}'   ,'{supplierInvoiceDetail.PartId}'   ,'{supplierInvoiceDetail.Qty}'   ,'{supplierInvoiceDetail.Price}'   ,'{supplierInvoiceDetail.Total}'   ,'{supplierInvoiceDetail.AdjustedPOQty}'   ,'{supplierInvoiceDetail.ExcessQty}'   ,'{supplierInvoiceDetail.BoxNo}','{false}','{0}')");

                        sql = sql + " Select Scope_Identity()";
                        command.CommandText = sql;
                        supplierInvoiceDetail.Id = Convert.ToInt32(command.ExecuteScalar());
                        if (!(supplierInvoice.DontImpactPO))
                        {
                            foreach (SupplierInvoicePoDetails supplierInvoicePoDetails in supplierInvoiceDetail.supplierInvoicePoDetails)
                            {
                            supplierInvoicePoDetails.InvoiceDetailId = supplierInvoiceDetail.Id;

                            sql = string.Format($"INSERT INTO [dbo].[SupplierInvoicePoDetails]   ([PartId],[InvoiceId],[InvoiceDetailId],[PoId],[PODetailId],[PONo],[Qty],[UnitPrice]) VALUES   ('{supplierInvoicePoDetails.PartId}', '{invoiceId}','{supplierInvoicePoDetails.InvoiceDetailId}','{supplierInvoicePoDetails.PoId}','{supplierInvoicePoDetails.PODetailId}'   ,'{supplierInvoicePoDetails.PONo}'   ,'{supplierInvoicePoDetails.Qty}', '{supplierInvoicePoDetails.UnitPrice}')");

                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                           
                                var poResult = await _poRepository.GetPoAsync(supplierInvoicePoDetails.PoId, command.Connection, command.Transaction);

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

                                poResult = await _poRepository.GetPoAsync(supplierInvoicePoDetails.PoId, command.Connection, command.Transaction);
                                var openPO = poResult.poDetails.Where(x => x.PoId == supplierInvoicePoDetails.PoId && x.IsClosed == false).FirstOrDefault();

                                if (openPO == null)
                                {
                                    sql = string.Format($"UPDATE [dbo].[PoMaster]   SET [IsClosed] = '{true}' ,[ClosingDate] = '{DateTime.Now}'  WHERE id = '{supplierInvoicePoDetails.PoId}' ");
                                    command.CommandText = sql;
                                    await command.ExecuteNonQueryAsync();
                                    //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                                }
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
                finally
                {
                    connection.Close();
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

        public async Task<IEnumerable<SupplierInvoice>> GetAllSupplierInvoicesAsync(int companyId,int userId)
        {
            List<SupplierInvoice> supplierInvoices = new List<SupplierInvoice>();

            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            var commandText = "";
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
               $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
               $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate],[Barcode]  FROM [SupplierInvoiceMaster] where CompanyId = '{companyId}' ");

            }
            if (userInfo.UserTypeId == 2)
            {
                return supplierInvoices;
            }
            if (userInfo.UserTypeId == 3)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
               $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
               $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate],[Barcode]  FROM [SupplierInvoiceMaster] where CompanyId = '{companyId}' and  [SupplierId] in ({companylist})");
                
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

           
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

        public async Task<IEnumerable<SupplierInvoice>> GetIntransitSupplierInvoicesAsync(int companyId)
        {
            List<SupplierInvoice> supplierInvoices = new List<SupplierInvoice>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
                $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
                $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate],[Barcode]  FROM [SupplierInvoiceMaster] where CompanyId = '{companyId}' and ([IsInvoiceReceived] = 0 OR [IsInvoiceReceived] IS NULL)");

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

        public async Task<IEnumerable<SupplierInvoice>> GetIntransitSupplierInvoicesByPartIdAsync(int companyId,int partId)
        {
            List<SupplierInvoice> supplierInvoices = new List<SupplierInvoice>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
                $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
                $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate],[Barcode]  FROM [SupplierInvoiceMaster] where CompanyId = '{companyId}' and ([IsInvoiceReceived] = 0 OR [IsInvoiceReceived] IS NULL) AND " +
                $"ID IN (SELECT InvoiceId FROM SupplierInvoiceDetails WHERE PartId = '{partId}' and (IsBoxReceived!=1 OR IsBoxReceived IS NULL))");

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
                commandText = string.Format($"SELECT  [Id] ,[InvoiceId] ,[SrNo] ,[PartId] ,[Qty] ,[Price] ,[Total] ,[AdjustedPOQty] ,[ExcessQty] ,[BoxNo],[Barcode]  FROM [dbo].[SupplierInvoiceDetails] where InvoiceId = '{ supplierInvoice.Id}' and PartId = '{ partId}'");

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

           return supplierInvoices;
        }

        public async Task<IEnumerable<SupplierOpenInvoice>> GetOpenSupplierInvoicesByPartIdAsync(int companyId, int partId)
        {
            List<SupplierOpenInvoice> supplierInvoices = new List<SupplierOpenInvoice>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT  [InvoiceId],[PartId],InvoiceNo ,sum(Qty-AdjustedInvoiceQty) OpenQty FROM [dbo].[SupplierInvoiceDetails] SID INNER JOIN SupplierInvoiceMaster SIM ON SIM.ID = SID.InvoiceId where PartId = '{partId}' and companyid = '{companyId}'and ( Qty > AdjustedInvoiceQty) group by InvoiceId,partid,INVOICENO");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var supplierInvoice = new SupplierOpenInvoice();
                    supplierInvoice.InvoiceId = Convert.ToInt32(dataReader["InvoiceId"]);
                    supplierInvoice.PartId = Convert.ToInt32(dataReader["PartId"]);
                    supplierInvoice.InvoiceNo = Convert.ToString(dataReader["InvoiceNo"]);
                    supplierInvoice.OpenQty = Convert.ToInt32(dataReader["OpenQty"]);

                    supplierInvoices.Add(supplierInvoice);
                }
                conn.Close();
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
       
        public async Task<SupplierInvoice> GetSupplierInvoiceAsync(long supplierInvoiceId, SqlConnection conn,SqlTransaction transaction)
        {
            SupplierInvoice supplierInvoice = new SupplierInvoice();
            //SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
                $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
                $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate],[Barcode]  FROM [SupplierInvoiceMaster] where Id = '{supplierInvoiceId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn,transaction))
            {
                cmd.CommandType = CommandType.Text;              

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

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
                dataReader.Close();
            }


            List<SupplierInvoiceDetail> supplierInvoiceDetails = new List<SupplierInvoiceDetail>();
            commandText = string.Format($"SELECT  [Id] ,[InvoiceId] ,[SrNo] ,[PartId] ,[Qty] ,[Price] ,[Total] ,[AdjustedPOQty] ,[ExcessQty] ,[BoxNo],[Barcode],[AdjustedInvoiceQty]  FROM [dbo].[SupplierInvoiceDetails] where InvoiceId = '{ supplierInvoice.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn,transaction))
            {
                cmd1.CommandType = CommandType.Text;
                
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);
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
                    supplierInvoiceDetail.AdjustedInvoiceQty = Convert.ToInt32(dataReader1["AdjustedInvoiceQty"]);
                    supplierInvoiceDetail.Barcode = DBNull.Value.Equals(dataReader1["Barcode"]) ? string.Empty : Convert.ToString(dataReader1["Barcode"]);
                    supplierInvoiceDetails.Add(supplierInvoiceDetail);
                }
                dataReader1.Close();
            }
            supplierInvoice.supplierInvoiceDetails = supplierInvoiceDetails;
            



            foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
            {
                List<SupplierInvoicePoDetails> supplierInvoicePoDetails = new List<SupplierInvoicePoDetails>();
                commandText = string.Format("SELECT  [Id] ,[PartId] ,[InvoiceId] ,[InvoiceDetailId] ,[POId] ,[PODetailId] ,[PONo] ,[Qty]  FROM [dbo].[SupplierInvoicePoDetails] " +
                    "where InvoiceDetailId = '{0}'", supplierInvoiceDetail.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn,transaction))
                {
                    cmd1.CommandType = CommandType.Text;                    
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);

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
                    dataReader1.Close();
                }
                supplierInvoiceDetail.supplierInvoicePoDetails = supplierInvoicePoDetails;
                
            }

            return supplierInvoice;
        }
        
        public async Task<SupplierInvoice> GetSupplierInvoiceAsync(string invoiceNo)
        {
            SupplierInvoice supplierInvoice = new SupplierInvoice();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
                $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
                $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate],[Barcode]  FROM [SupplierInvoiceMaster] where InvoiceNo = '{invoiceNo}' ");

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
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

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

                    var supplierInvoice = await GetSupplierInvoiceAsync(supplierInvoiceId, command.Connection, command.Transaction);

                    foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                    {                      

                        foreach (SupplierInvoicePoDetails supplierInvoicePoDetails in supplierInvoiceDetail.supplierInvoicePoDetails)
                        {
                            sql = string.Format($"UPDATE [dbo].[PoDetails]   SET [InTransitQty] = InTransitQty - '{supplierInvoicePoDetails.Qty}', [ReceivedQty] = ReceivedQty + '{supplierInvoicePoDetails.Qty}'  WHERE id = '{supplierInvoicePoDetails.PODetailId}' ");

                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }

                        sql = string.Format($"UPDATE [dbo].[part]   SET [IntransitQty] =  IntransitQty - '{supplierInvoiceDetail.Qty}',[QtyInHand] =  QtyInHand + '{supplierInvoiceDetail.Qty}'  WHERE id = '{supplierInvoiceDetail.PartId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{supplierInvoiceDetail.PartId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.RECEIVE_SUPPLIER_INVOICE)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.IN)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND)}'   ,'{supplierInvoiceId.ToString()}'   ,'{supplierInvoiceDetail.Qty}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);                        
                    }

                    var stocks = await GetStockQtyAsync(supplierInvoiceId, command.Connection, command.Transaction);

                    foreach(StockPrice stockPrice in stocks)
                    {
                        if(IfStockExist(stockPrice, command.Connection, command.Transaction))
                        {
                            sql = string.Format($"UPDATE [StockPrice] SET [Qty] = [Qty] + '{stockPrice.Qty}' WHERE PartId = '{stockPrice.PartId}' AND SupplierPrice = '{stockPrice.SupplierPrice}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }
                        else
                        {
                            sql = string.Format($"INSERT INTO [dbo].[StockPrice]  ([PartId] ,[SupplierPrice] ,[CustomerPrice] ,[Qty])  VALUES  ('{stockPrice.PartId}','{stockPrice.SupplierPrice}','{stockPrice.CustomerPrice}','{stockPrice.Qty}')");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

        }

        private bool IfStockExist(StockPrice stockPrice, SqlConnection conn, SqlTransaction transaction)
        {
            bool IfRecordExist = false;
            var commandText = string.Format($"SELECT CustomerPrice FROM StockPrice WHERE PartId = '{stockPrice.PartId}' AND SupplierPrice = '{stockPrice.SupplierPrice}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = cmd.ExecuteReader(CommandBehavior.Default);

                while (dataReader.Read())
                {
                    IfRecordExist = true;
                }
                dataReader.Close();
            }

            return IfRecordExist;
        }

        public async Task<List<StockPrice>> GetStockQtyAsync(long invoiceId, SqlConnection conn, SqlTransaction transaction)
        {
            var stockPrices = new List<StockPrice>();

            var commandText = string.Format($"SELECT [PartId],Sum([Qty]) QTY,[UnitPrice] FROM [SupplierInvoicePoDetails]  where InvoiceId = '{invoiceId}' group by PartId,UnitPrice ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                while (dataReader.Read())
                {
                    var stockPrice = new StockPrice();
                    stockPrice.PartId = Convert.ToInt32(dataReader["PartId"]);
                    stockPrice.Qty = Convert.ToInt32(dataReader["Qty"]);
                    stockPrice.SupplierPrice = Convert.ToDecimal(dataReader["UnitPrice"]);
                    stockPrices.Add(stockPrice);
                }

                dataReader.Close();
            }

            commandText = string.Format($"SELECT [PartId],sum ([ExcessQty]) QTY FROM [SupplierInvoiceDetails]  where InvoiceId = '{invoiceId}' and [ExcessQty] > 0 group by PartId ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                while (dataReader.Read())
                {
                    var stockPrice = new StockPrice();
                    stockPrice.PartId = Convert.ToInt32(dataReader["PartId"]);
                    stockPrice.Qty = Convert.ToInt32(dataReader["Qty"]);
                    stockPrice.SupplierPrice = 0;
                    stockPrices.Add(stockPrice);
                }

                dataReader.Close();
            }

            foreach (StockPrice stockPrice in stockPrices)
            {
                if (stockPrice.SupplierPrice == 0)
                {
                    commandText = string.Format($"SELECT TOP 1 UnitPrice FROM partsupplierassignment WHERE PartID = '{stockPrice.PartId}' ");

                    using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                        while (dataReader.Read())
                        {
                            stockPrice.SupplierPrice = Convert.ToDecimal(dataReader["UnitPrice"]);
                        }

                        dataReader.Close();
                    }
                }
            }

            foreach (StockPrice stockPrice in stockPrices)
            {
                commandText = string.Format($"SELECT CustomerPrice FROM StockPrice WHERE PartId = '{stockPrice.PartId}' AND SupplierPrice = '{stockPrice.SupplierPrice}' ");

                using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
                {
                    cmd.CommandType = CommandType.Text;

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                    while (dataReader.Read())
                    {                      
                        stockPrice.CustomerPrice = Convert.ToDecimal(dataReader["CustomerPrice"]);                       
                    }
                    dataReader.Close();
                }
            }

            foreach (StockPrice stockPrice in stockPrices)
            {
                if (stockPrice.CustomerPrice == 0)
                {
                    commandText = string.Format($"SELECT TOP 1 Rate FROM partcustomerassignment WHERE PartID  = '{stockPrice.PartId}' ");

                    using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
                    {
                        cmd.CommandType = CommandType.Text;

                        var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                        while (dataReader.Read())
                        {
                            stockPrice.CustomerPrice = Convert.ToDecimal(dataReader["Rate"]);
                        }

                        dataReader.Close();
                    }
                }
            }

            return stockPrices;
        }

        public async Task UnReceiveSupplierInvoiceAsync(long supplierInvoiceId)
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

                try
                {
                    var sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]   SET [IsInvoiceReceived] = '{false}' WHERE Id = '{supplierInvoiceId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();


                    sql = string.Format($"UPDATE [dbo].[SupplierInvoiceDetails]   SET [IsBoxReceived] = '{false}'  WHERE [InvoiceId] = '{supplierInvoiceId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    var stocks = await GetStockQtyAsync(supplierInvoiceId, command.Connection, command.Transaction);

                    foreach (StockPrice stockPrice in stocks)
                    {
                        if (IfStockExist(stockPrice, command.Connection, command.Transaction))
                        {
                            sql = string.Format($"UPDATE [StockPrice] SET [Qty] = [Qty] - '{stockPrice.Qty}' WHERE PartId = '{stockPrice.PartId}' AND SupplierPrice = '{stockPrice.SupplierPrice}' ");
                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }                        
                    }

                    var supplierInvoice = await GetSupplierInvoiceAsync(supplierInvoiceId, command.Connection, command.Transaction);

                    foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                    {

                        foreach (SupplierInvoicePoDetails supplierInvoicePoDetails in supplierInvoiceDetail.supplierInvoicePoDetails)
                        {
                            sql = string.Format($"UPDATE [dbo].[PoDetails]   SET [InTransitQty] = InTransitQty + '{supplierInvoicePoDetails.Qty}', [ReceivedQty] = ReceivedQty - '{supplierInvoicePoDetails.Qty}'  WHERE id = '{supplierInvoicePoDetails.PODetailId}' ");

                            command.CommandText = sql;
                            await command.ExecuteNonQueryAsync();
                        }

                        sql = string.Format($"UPDATE [dbo].[part]   SET [IntransitQty] =  IntransitQty + '{supplierInvoiceDetail.Qty}',[QtyInHand] =  QtyInHand - '{supplierInvoiceDetail.Qty}'  WHERE id = '{supplierInvoiceDetail.PartId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{supplierInvoiceDetail.PartId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.UNRECEIVE_SUPPLIER_INVOICE)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.OUT)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND)}'   ,'{supplierInvoiceId.ToString()}'   ,'{supplierInvoiceDetail.Qty}')");
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

        public async Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice)
        {            
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
                    var sql = string.Format($"UPDATE [dbo].[SupplierInvoiceMaster]  SET [InvoiceDate] =  '{supplierInvoice.InvoiceDate}' , [ETA] =  '{supplierInvoice.ETA}'  WHERE id = '{supplierInvoice.Id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
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

            ;
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
