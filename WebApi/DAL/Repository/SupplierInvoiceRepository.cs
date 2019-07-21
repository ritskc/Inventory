using DAL.DBHelper;
using DAL.IRepository;
using DAL.Models;
using DAL.Settings;
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

        public SupplierInvoiceRepository(ISqlHelper sqlHelper, IPoRepository poRepository)
        {
            _sqlHelper = sqlHelper;
            _poRepository = poRepository;
        }
        public async Task AddSupplierInvoiceAsync(SupplierInvoice supplierInvoice)
        {
            //throw new NotImplementedException();
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
                    string sql = string.Format($"INSERT INTO [dbo].[SupplierInvoiceMaster]   ([CompanyId]   ,[SupplierId]   ,[InvoiceNo]   ,[InvoiceDate]   ,[ETA]   ,[IsAirShipment]   ,[PoNo]   ,[ReferenceNo]   ,[Email]   ,[ByCourier]   ,[IsInvoiceUploaded]   ,[IsPackingSlipUploaded]   ,[IsTenPlusUploaded]   ,[IsBLUploaded]   ,[IsTCUploaded]   ,[InvoicePath]   ,[PackingSlipPath]   ,[TenPlusPath]   ,[BLPath]   ,[IsInvoiceReceived]   ,[UploadedDate]   ,[ReceivedDate]) VALUES   ('{supplierInvoice.CompanyId}'   ,'{supplierInvoice.SupplierId}'   ,'{supplierInvoice.InvoiceNo}'   ,'{supplierInvoice.InvoiceDate}'   ,'{supplierInvoice.ETA}'   ,'{supplierInvoice.IsAirShipment}'   ,'{supplierInvoice.PoNo}'   ,'{supplierInvoice.ReferenceNo}'   ,'{supplierInvoice.Email}'   ,'{supplierInvoice.ByCourier}'   ,'{supplierInvoice.IsInvoiceUploaded}'   ,'{supplierInvoice.IsPackingSlipUploaded}'   ,'{supplierInvoice.IsTenPlusUploaded}'   ,'{supplierInvoice.IsBLUploaded}'   ,'{supplierInvoice.IsTCUploaded}'   ,'{supplierInvoice.InvoicePath}'   ,'{supplierInvoice.PackingSlipPath}'   ,'{supplierInvoice.TenPlusPath}'   ,'{supplierInvoice.BLPath}'   ,'{supplierInvoice.IsInvoiceReceived}'   ,'{supplierInvoice.UploadedDate}'   ,'{supplierInvoice.ReceivedDate}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;
                    var invoiceId = command.ExecuteScalar();

                    foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[SupplierInvoiceDetails]   ([InvoiceId]   ,[SrNo]   ,[PartId]   ,[Qty]   ,[Price]   ,[Total]   ,[AdjustedPOQty]   ,[ExcessQty]   ,[BoxNo]) VALUES   ('{invoiceId}'   ,'{supplierInvoiceDetail.SrNo}'   ,'{supplierInvoiceDetail.PartId}'   ,'{supplierInvoiceDetail.Qty}'   ,'{supplierInvoiceDetail.Price}'   ,'{supplierInvoiceDetail.Total}'   ,'{supplierInvoiceDetail.AdjustedPOQty}'   ,'{supplierInvoiceDetail.ExcessQty}'   ,'{supplierInvoiceDetail.BoxNo}')");

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

                            if (poDetail.AckQty >= supplierInvoicePoDetails.Qty + poDetail.InTransitQty + poDetail.ReceivedQty)
                            {
                                sql = string.Format($"UPDATE [dbo].[PoDetails]   SET [InTransitQty] = '{supplierInvoicePoDetails.Qty}' ,[IsClosed] = '{true}' ,[ClosingDate] = '{DateTime.Now}'  WHERE id = '{supplierInvoicePoDetails.PODetailId}' ");
                            }
                            else
                            {
                                sql = string.Format($"UPDATE [dbo].[PoDetails]   SET [InTransitQty] = '{supplierInvoicePoDetails.Qty}'  WHERE id = '{supplierInvoicePoDetails.PODetailId}' ");
                            }
                            await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

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

                    foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                    {

                        foreach (SupplierInvoicePoDetails supplierInvoicePoDetails in supplierInvoiceDetail.supplierInvoicePoDetails)
                        {
                            var poResult = await _poRepository.GetPoAsync(supplierInvoicePoDetails.PoId);

                            var poDetail = poResult.poDetails.Where(x => x.IsClosed == false).FirstOrDefault();

                            if (poDetail == null)
                            {
                                string sql = string.Format($"UPDATE [dbo].[PoMaster]   SET [IsClosed] = '{true}' ,[ClosingDate] = '{DateTime.Now}'  WHERE id = '{poResult.Id}' ");

                                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                            }


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

        public Task<int> DeleteSupplierInvoiceAsync(long supplierInvoiceId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SupplierInvoice>> GetAllSupplierInvoicesAsync(int companyId)
        {
            List<SupplierInvoice> supplierInvoices = new List<SupplierInvoice>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,[IsAirShipment] ,[PoNo] ,[ReferenceNo] ,[Email] " +
                $",[ByCourier] ,[IsInvoiceUploaded] ,[IsPackingSlipUploaded] ,[IsTenPlusUploaded] ,[IsBLUploaded] ,[IsTCUploaded] ," +
                $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate]  FROM [SupplierInvoiceMaster] where CompanyId = '{companyId}' ");

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
                    supplierInvoices.Add(supplierInvoice);
                }
                conn.Close();
            }

            foreach (SupplierInvoice supplierInvoice in supplierInvoices)
            {
                List<SupplierInvoiceDetail> supplierInvoiceDetails = new List<SupplierInvoiceDetail>();
                commandText = string.Format($"SELECT  [Id] ,[InvoiceId] ,[SrNo] ,[PartId] ,[Qty] ,[Price] ,[Total] ,[AdjustedPOQty] ,[ExcessQty] ,[BoxNo]  FROM [dbo].[SupplierInvoiceDetails] where InvoiceId = '{ supplierInvoice.Id}'");

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
                $"[InvoicePath] ,[PackingSlipPath] ,[TenPlusPath] ,[BLPath] ,[IsInvoiceReceived] ,[UploadedDate] ,[ReceivedDate]  FROM [SupplierInvoiceMaster] where Id = '{supplierInvoiceId}' ");

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
                }
                conn.Close();
            }


            List<SupplierInvoiceDetail> supplierInvoiceDetails = new List<SupplierInvoiceDetail>();
            commandText = string.Format($"SELECT  [Id] ,[InvoiceId] ,[SrNo] ,[PartId] ,[Qty] ,[Price] ,[Total] ,[AdjustedPOQty] ,[ExcessQty] ,[BoxNo]  FROM [dbo].[SupplierInvoiceDetails] where InvoiceId = '{ supplierInvoice.Id}'");

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

        public Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice)
        {
            throw new NotImplementedException();
        }
    }
}
