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
    public class PackingSlipRepository : IPackingSlipRepository
    {
        private readonly ISqlHelper _sqlHelper;
        private readonly IOrderRepository _orderRepository;

        public PackingSlipRepository(ISqlHelper sqlHelper, IOrderRepository orderRepository)
        {
            this._sqlHelper = sqlHelper;
            this._orderRepository = orderRepository;
        }
        public async Task<Int32> AddPackingSlipAsync(PackingSlip packingSlip)
        {
            Int32 packingSlipId;
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
                    string sql = string.Format($"INSERT INTO [dbo].[PackingSlipMaster]   ([CompanyId]   ,[CustomerId]   ,[PackingSlipNo]   ,[ShippingDate]   ,[ShipVia]   ,[Crates]   ,[Boxes]   ,[GrossWeight]   ,[ShippingCharge]   ,[CustomCharge]   ,[SubTotal]   ,[Total]   ,[IsInvoiceCreated]   ,[IsPaymentReceived]   ,[FOB]   ,[Terms]   ,[ShipmentInfoId]   ,[InvoiceDate]) VALUES   ('{packingSlip.CompanyId}'   ,'{packingSlip.CustomerId}'   ,'{packingSlip.PackingSlipNo}'   ,'{packingSlip.ShippingDate}'   ,'{packingSlip.ShipVia}'   ,'{packingSlip.Crates}'   ,'{packingSlip.Boxes}'   ,'{packingSlip.GrossWeight}'   ,'{packingSlip.ShippingCharge}'   ,'{packingSlip.CustomCharge}'   ,'{packingSlip.SubTotal}'   ,'{packingSlip.Total}'   ,'{false}'   ,'{packingSlip.IsPaymentReceived}'   ,'{packingSlip.FOB}'   ,'{packingSlip.Terms}'   ,'{packingSlip.ShipmentInfoId}'   ,'{null}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;
                    packingSlipId = Convert.ToInt32( command.ExecuteScalar());
                    packingSlip.Id =  Convert.ToInt32(packingSlipId);

                    foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[PackingSlipDetails]   ([PackingSlipId]   ,[IsBlankOrder]   ,[OrderNo]   ,[OrderId]   ,[OrderDetailId]   ,[PartId]   ,[Qty]   ,[Boxes]   ,[InBasket]   ,[UnitPrice]   ,[Price]   ,[Surcharge]   ,[SurchargePerPound]   ,[SurchargePerUnit]   ,[TotalSurcharge]   ,[ExcessQty]) VALUES   ('{packingSlipId}'   ,'{packingSlipDetail.IsBlankOrder}'   ,'{packingSlipDetail.OrderNo}'   ,'{packingSlipDetail.OrderId}'   ,'{packingSlipDetail.OrderDetailId}'   ,'{packingSlipDetail.PartId}'   ,'{packingSlipDetail.Qty}'   ,'{packingSlipDetail.Boxes}'   ,'{packingSlipDetail.InBasket}'   ,'{packingSlipDetail.UnitPrice}'   ,'{packingSlipDetail.Price}'   ,'{packingSlipDetail.Surcharge}'   ,'{packingSlipDetail.SurchargePerPound}'   ,'{packingSlipDetail.SurchargePerUnit}'   ,'{packingSlipDetail.TotalSurcharge}'   ,'{packingSlipDetail.ExcessQty}')");

                        sql = sql + " Select Scope_Identity()";
                        command.CommandText = sql;
                        packingSlipDetail.Id = Convert.ToInt32(command.ExecuteScalar());

                        if (!packingSlipDetail.IsBlankOrder)
                        {
                            var orderResult = await _orderRepository.GetOrderMasterAsync(packingSlipDetail.OrderId);

                            if (orderResult != null)
                            {
                                var orderDetail = orderResult.OrderDetails.Where(x => x.Id == packingSlipDetail.OrderDetailId).FirstOrDefault();

                                if (orderDetail != null)
                                {
                                    sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET [ShippedQty] = '{orderDetail.ShippedQty + packingSlipDetail.Qty - packingSlipDetail.ExcessQty}'  WHERE id = '{orderDetail.Id}' ");
                                    await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                                }
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
            return packingSlipId;
        }

        public async  Task CreateInvoiceAsync(PackingSlip packingSlip)
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
                    string sql = string.Format($"UPDATE [dbo].[PackingSlipMaster]   SET [ShippingCharge] = '{packingSlip.ShippingCharge}' ,[CustomCharge] = '{packingSlip.CustomCharge}' ,[SubTotal] = '{packingSlip.SubTotal}' ,[Total] = '{packingSlip.Total}' ,[IsInvoiceCreated] = '{true}' ,[InvoiceDate] = '{DateTime.Now}' WHERE Id = '{packingSlip.Id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (PackingSlipDetails packingSlipDetail in packingSlip.PackingSlipDetails)
                    {
                        sql = string.Format($"UPDATE [dbo].[PackingSlipDetails]   SET [UnitPrice] = '{packingSlipDetail.UnitPrice}' ,[Price] = '{packingSlipDetail.Price}' ,[Surcharge] = '{packingSlipDetail.Surcharge}' ,[SurchargePerPound] = '{packingSlipDetail.SurchargePerPound}' ,[SurchargePerUnit] = '{packingSlipDetail.SurchargePerUnit}' ,[TotalSurcharge] = '{packingSlipDetail.TotalSurcharge}' WHERE Id = '{packingSlipDetail.Id}'");
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
        }

        public Task<int> DeleteSupplierInvoiceAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PackingSlip>> GetAllPackingSlipsAsync(int companyId)
        {
            List<PackingSlip> packingSlips = new List<PackingSlip>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate]  FROM [dbo].[PackingSlipMaster] where CompanyId = '{companyId}' ");

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
                    
                    packingSlips.Add(packingSlip);
                }
                conn.Close();
            }

            foreach (PackingSlip packingSlip in packingSlips)
            {
                List<PackingSlipDetails> packingSlipDetails = new List<PackingSlipDetails>();
                commandText = string.Format($"SELECT [Id] ,[PackingSlipId] ,[IsBlankOrder] ,[OrderNo] ,[OrderId] ,[OrderDetailId] ,[PartId] ,[Qty] ," +
                    $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}'");

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
                        
                        packingSlipDetails.Add(packingSlipDetail);
                    }
                }
                packingSlip.PackingSlipDetails = packingSlipDetails;
                conn.Close();
            }            

            return packingSlips;
        }

        public async Task<PackingSlip> GetPackingSlipAsync(long id)
        {
            var packingSlip = new PackingSlip();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate]  FROM [dbo].[PackingSlipMaster] where Id = '{id}' ");

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
                }
                conn.Close();
            }

            List<PackingSlipDetails> packingSlipDetails = new List<PackingSlipDetails>();
            commandText = string.Format($"SELECT [Id] ,[PackingSlipId] ,[IsBlankOrder] ,[OrderNo] ,[OrderId] ,[OrderDetailId] ,[PartId] ,[Qty] ," +
                $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}'");

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

                    packingSlipDetails.Add(packingSlipDetail);
                }
            }
            packingSlip.PackingSlipDetails = packingSlipDetails;
            conn.Close();

            return packingSlip;
        }

        public PackingSlip GetPackingSlip(long id)
        {
            var packingSlip = new PackingSlip();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate]  FROM [dbo].[PackingSlipMaster] where Id = '{id}' ");

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
                }
                conn.Close();
            }

            List<PackingSlipDetails> packingSlipDetails = new List<PackingSlipDetails>();
            commandText = string.Format($"SELECT [Id] ,[PackingSlipId] ,[IsBlankOrder] ,[OrderNo] ,[OrderId] ,[OrderDetailId] ,[PartId] ,[Qty] ," +
                $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}'");

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

                    packingSlipDetails.Add(packingSlipDetail);
                }
            }
            packingSlip.PackingSlipDetails = packingSlipDetails;
            conn.Close();

            return packingSlip;
        }

        public Task UpdatePackingSlipAsync(PackingSlip packingSlip)
        {
            throw new NotImplementedException();
        }
    }
}
