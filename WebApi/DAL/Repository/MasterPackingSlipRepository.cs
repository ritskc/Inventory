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
    public class MasterPackingSlipRepository : IMasterPackingSlipRepository
    {
        private readonly ISqlHelper sqlHelper;
        private readonly IEntityTrackerRepository entityTrackerRepository;
        private readonly IPackingSlipRepository packingSlipRepository;
        private readonly IUserRepository userRepository;
        private readonly ICustomerRepository customerRepository;

        public MasterPackingSlipRepository(ISqlHelper sqlHelper, IEntityTrackerRepository entityTrackerRepository, 
            IPackingSlipRepository packingSlipRepository, IUserRepository userRepository, ICustomerRepository customerRepository)
        {
            this.sqlHelper = sqlHelper;
            this.entityTrackerRepository = entityTrackerRepository;
            this.packingSlipRepository = packingSlipRepository;
            this.userRepository = userRepository;
            this.customerRepository = customerRepository;

        }
        public async Task<int> AddMasterPackingSlipAsync(MasterPackingSlip masterPackingSlip)
        {
            Int32 masterPackingSlipId = 0;
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
                    string sql = string.Format($"INSERT INTO [dbo].[MasterPackingSlipMaster]   ([CompanyId]   ,[CustomerId]   ,[MasterPackingSlipNo]   ,[UpdatedDate]   ,[Comment]   ,[IsPOSUploaded]   ,[POSPath]   ,[TrakingNumber]) VALUES   ('{masterPackingSlip.CompanyId}'   ,'{masterPackingSlip.CustomerId}'   ,'{masterPackingSlip.MasterPackingSlipNo}'   ,'{DateTime.Now}'   ,'{masterPackingSlip.Comment}'   ,'{false}'   ,'{""}'   ,'{""}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;

                    masterPackingSlipId = Convert.ToInt32(command.ExecuteScalar());
                    masterPackingSlip.Id = Convert.ToInt32(masterPackingSlipId);

                    foreach (PackingSlip packingSlip in masterPackingSlip.PackingSlips)
                    {
                        sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET  [IsPOSUploaded] = '{true}', [IsMasterPackingSlip] = '{true}',[MasterPackingSlipId] = '{masterPackingSlipId}'  WHERE id = '{packingSlip.Id}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }
                    await entityTrackerRepository.AddEntityAsync(masterPackingSlip.CompanyId, DateTime.Now, BusinessConstants.ENTITY_TRACKER_MASTER_PACKING_SLIP, command.Connection, command.Transaction, command);
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
            return masterPackingSlipId;
        }

        public async Task<bool> DeleteMasterPackingSlipAsync(long id)
        {
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
                    sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [IsPOSUploaded] ='{false}', [IsMasterPackingSlip] = '{false}',[MasterPackingSlipId] = '{0}'  WHERE MasterPackingSlipId = '{id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"DELETE FROM [dbo].[MasterPackingSlipMaster] WHERE id = '{id}'");
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

        public async Task<IEnumerable<MasterPackingSlip>> GetAllMasterPackingSlipsAsync(int companyId, int userId)
        {
            List<MasterPackingSlip> masterPackingSlips = new List<MasterPackingSlip>();

            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[MasterPackingSlipNo] " +
                  $",[UpdatedDate] ,[Comment] ,[IsPOSUploaded] ,[POSPath] ,[TrakingNumber]  FROM [dbo].[MasterPackingSlipMaster] where [CompanyId] = '{companyId}' ");

            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);

                commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[MasterPackingSlipNo] " +
                 $",[UpdatedDate] ,[Comment] ,[IsPOSUploaded] ,[POSPath] ,[TrakingNumber]  FROM [dbo].[MasterPackingSlipMaster] where [CompanyId] = '{companyId}' and  [CustomerId] in ({companylist}) ");
             }
            if (userInfo.UserTypeId == 3)
            {
                return masterPackingSlips;
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var masterPackingSlip = new MasterPackingSlip();
                    masterPackingSlip.PackingSlips = new List<PackingSlip>();

                    masterPackingSlip.Id = Convert.ToInt32(dataReader["Id"]);
                    masterPackingSlip.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    masterPackingSlip.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    masterPackingSlip.MasterPackingSlipNo = Convert.ToString(dataReader["MasterPackingSlipNo"]);
                    masterPackingSlip.UpdatedDate = Convert.ToDateTime(dataReader["UpdatedDate"]);
                    masterPackingSlip.Comment = Convert.ToString(dataReader["Comment"]);
                    masterPackingSlip.IsPOSUploaded = Convert.ToBoolean(dataReader["IsPOSUploaded"]);
                    masterPackingSlip.POSPath = Convert.ToString(dataReader["POSPath"]);
                    masterPackingSlip.TrakingNumber = Convert.ToString(dataReader["TrakingNumber"]);

                    masterPackingSlips.Add(masterPackingSlip);
                }
                dataReader.Close();
                conn.Close();
            }

                foreach(MasterPackingSlip masterPackingSlip in masterPackingSlips)
                {
                    commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                 $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                 $"[ShipmentInfoId] ,[InvoiceDate],[IsPOSUploaded],[POSPath],[TotalSurcharge],[IsMasterPackingSlip],[MasterPackingSlipId],[IsRepackage],[TrakingNumber],[IsShipmentVerified],[IsScanned],[AllowScanning]  FROM [dbo].[PackingSlipMaster] where [MasterPackingSlipId] = '{masterPackingSlip.Id}' ");

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

                        masterPackingSlip.PackingSlips.Add(packingSlip);
                    }
                    dataReader.Close();
                    conn.Close();

                }
            }
            return masterPackingSlips;
        }

        public MasterPackingSlip GetMasterPackingSlip(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<MasterPackingSlip> GetMasterPackingSlipAsync(long Id)
        {
            var masterPackingSlip = new MasterPackingSlip();
            masterPackingSlip.PackingSlips = new List<PackingSlip>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[MasterPackingSlipNo] " +
                $",[UpdatedDate] ,[Comment] ,[IsPOSUploaded] ,[POSPath] ,[TrakingNumber]  FROM [dbo].[MasterPackingSlipMaster] where Id = '{Id}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    masterPackingSlip.Id = Convert.ToInt32(dataReader["Id"]);
                    masterPackingSlip.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    masterPackingSlip.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    masterPackingSlip.MasterPackingSlipNo = Convert.ToString(dataReader["MasterPackingSlipNo"]);
                    masterPackingSlip.UpdatedDate = Convert.ToDateTime(dataReader["UpdatedDate"]);
                    masterPackingSlip.Comment = Convert.ToString(dataReader["Comment"]);
                    masterPackingSlip.IsPOSUploaded = Convert.ToBoolean(dataReader["IsPOSUploaded"]);
                    masterPackingSlip.POSPath = Convert.ToString(dataReader["POSPath"]);
                    masterPackingSlip.TrakingNumber = Convert.ToString(dataReader["TrakingNumber"]);                    
                }
                dataReader.Close();
                conn.Close();
            }

            commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                 $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                 $"[ShipmentInfoId] ,[InvoiceDate],[IsPOSUploaded],[POSPath],[TotalSurcharge],[IsMasterPackingSlip],[MasterPackingSlipId]  FROM [dbo].[PackingSlipMaster] where [MasterPackingSlipId] = '{masterPackingSlip.Id}' ");

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

                    masterPackingSlip.PackingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }

            return masterPackingSlip;
        }

        public async Task<bool> UpdateMasterPackingSlipAsync(MasterPackingSlip masterPackingSlip)
        {
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
                    sql = string.Format($"UPDATE [dbo].[MasterPackingSlipMaster]   SET [UpdatedDate] = '{DateTime.Now}',[Comment] = '{masterPackingSlip.Comment}'  WHERE Id = '{masterPackingSlip.Id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [IsPOSUploaded] ='{false}', [IsMasterPackingSlip] = '{false}',[MasterPackingSlipId] = '{0}'  WHERE MasterPackingSlipId = '{masterPackingSlip.Id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (PackingSlip packingSlip in masterPackingSlip.PackingSlips)
                    {
                        sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [IsPOSUploaded] ='{true}',  [IsMasterPackingSlip] = '{true}',[MasterPackingSlipId] = '{masterPackingSlip.Id}'  WHERE id = '{packingSlip.Id}' ");
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

        public async Task UpdatePOSAsync(int masterPackingSlipId, string path, string trackingNumber, string accessId)
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
                    string sql = string.Format($"UPDATE [dbo].[MasterPackingSlipMaster]   SET [IsPOSUploaded] = '{true}' ,[POSPath] = '{path}',[TrakingNumber]='{trackingNumber}' ,[AccessId]='{accessId}'  WHERE Id = '{masterPackingSlipId}'");
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

        public async Task<bool> AllowScanning(int masterPackingSlipId, int userId)
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
                    string sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [AllowScanning] = '{true}'   WHERE MasterPackingSlipId = '{masterPackingSlipId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"UPDATE [dbo].[MasterPackingSlipMaster]   SET [AllowScanning] = '{true}'   WHERE Id = '{masterPackingSlipId}'");
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

        //    var commandText = string.Format($"SELECT [Id],[CustomerId]  FROM [dbo].[MasterPackingSlipMaster] WHERE  [AccessId] = = '{id}' ");

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

            var commandText = string.Format($"SELECT [Id]  FROM [dbo].[MasterPackingSlipMaster] WHERE  [AccessId] ='{accessId}' ");

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
