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
    public class OrderRepository : IOrderRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public OrderRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }       

        public async Task<IEnumerable<OrderMaster>> GetAllOrderMastersAsync(int companyId)
        {
            List<OrderMaster> orders = new List<OrderMaster>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format("SELECT [Id] ,[CompanyId] ,[CustomerId] ,[IsBlanketPO] ,[PONo] ,[PODate] ,[DueDate] ,[Remarks]  FROM [dbo].[OrderMaster] where CompanyId = '{0}' ", companyId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var order = new OrderMaster();
                    order.Id = Convert.ToInt64(dataReader["Id"]);
                    order.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    order.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    order.IsBlanketPO = Convert.ToBoolean(dataReader["IsBlanketPO"]);
                    order.PONo = Convert.ToString(dataReader["PONo"]);
                    order.PoDate = Convert.ToDateTime(dataReader["PoDate"]);
                    order.DueDate = Convert.ToDateTime(dataReader["DueDate"]);
                    order.Remarks = Convert.ToString(dataReader["Remarks"]);
                    
                    orders.Add(order);
                }
                conn.Close();
            }

            foreach (OrderMaster order in orders)
            {
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                commandText = string.Format("SELECT [Id] ,[OrderId] ,[PartId] ,[BlanketPOId] ,[BlanketPOAdjQty] ,[LineNumber] ,[Qty] ,[UnitPrice] " +
                    ",[DueDate] ,[Note],[ShippedQty]  FROM [dbo].[OrderDetail]  where orderid = '{0}'", order.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var orderDetail = new OrderDetail();
                        orderDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                        orderDetail.OrderId = Convert.ToInt64(dataReader1["OrderId"]);
                        orderDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                        orderDetail.BlanketPOId = Convert.ToInt64(dataReader1["BlanketPOId"]);
                        orderDetail.LineNumber = Convert.ToInt32(dataReader1["LineNumber"]);
                        orderDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                        orderDetail.ShippedQty = Convert.ToInt32(dataReader1["ShippedQty"]);
                        orderDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                        orderDetail.DueDate = Convert.ToDateTime(dataReader1["DueDate"]);
                        orderDetail.Note = Convert.ToString(dataReader1["Note"]);
                        orderDetails.Add(orderDetail);
                    }
                }
                order.OrderDetails = orderDetails;
                conn.Close();
            }

            return orders;
        }

        public async Task<OrderMaster> GetOrderMasterAsync(long orderId)
        {
            OrderMaster order = new OrderMaster();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format("SELECT [Id] ,[CompanyId] ,[CustomerId] ,[IsBlanketPO] ,[PONo] ,[PODate] ,[DueDate] ,[Remarks]  FROM [dbo].[OrderMaster] where Id = '{0}' ", orderId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    order.Id = Convert.ToInt64(dataReader["Id"]);
                    order.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    order.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    order.IsBlanketPO = Convert.ToBoolean(dataReader["IsBlanketPO"]);
                    order.PONo = Convert.ToString(dataReader["PONo"]);
                    order.PoDate = Convert.ToDateTime(dataReader["PoDate"]);
                    order.DueDate = Convert.ToDateTime(dataReader["DueDate"]);
                    order.Remarks = Convert.ToString(dataReader["Remarks"]);

                }
                conn.Close();
            }


            List<OrderDetail> orderDetails = new List<OrderDetail>();
            commandText = string.Format("SELECT [Id] ,[OrderId] ,[PartId] ,[BlanketPOId] ,[BlanketPOAdjQty] ,[LineNumber] ,[Qty] ,[UnitPrice] " +
                ",[DueDate] ,[Note],[ShippedQty]  FROM [dbo].[OrderDetail]  where orderid = '{0}'", order.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                    orderDetail.OrderId = Convert.ToInt64(dataReader1["OrderId"]);
                    orderDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                    orderDetail.BlanketPOId = Convert.ToInt64(dataReader1["BlanketPOId"]);
                    orderDetail.LineNumber = Convert.ToInt32(dataReader1["LineNumber"]);
                    orderDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                    orderDetail.ShippedQty = Convert.ToInt32(dataReader1["ShippedQty"]);
                    orderDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                    orderDetail.DueDate = Convert.ToDateTime(dataReader1["DueDate"]);
                    orderDetail.Note = Convert.ToString(dataReader1["Note"]);
                    orderDetails.Add(orderDetail);
                }
            }
            order.OrderDetails = orderDetails;
            conn.Close();
            return order;
        }

        public async Task AddOrderMasterAsync(OrderMaster order)
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
                    string sql = string.Format($"INSERT INTO [dbo].[OrderMaster]   ([CompanyId]   ,[CustomerId]   ,[IsBlanketPO]   ,[PONo]   ,[PODate]   ,[DueDate]   ,[Remarks]) VALUES   ('{order.CompanyId}'   ,'{order.CustomerId}'   ,'{order.IsBlanketPO}'   ,'{order.PONo}'   ,'{order.PoDate}'   ,'{order.DueDate}'   ,'{order.Remarks}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;
                    var id = command.ExecuteScalar();

                    foreach (OrderDetail orderDetail in order.OrderDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[OrderDetail]   ([OrderId]   ,[PartId]   ,[BlanketPOId]   ,[BlanketPOAdjQty]   ,[LineNumber]   ,[Qty]   ,[UnitPrice]   ,[DueDate]   ,[Note],[ShippedQty]) VALUES   ('{id}'   ,'{orderDetail.PartId}'   ,'{orderDetail.BlanketPOId}'   ,'{orderDetail.BlanketPOAdjQty}'   ,'{orderDetail.LineNumber}'   ,'{orderDetail.Qty}'   ,'{orderDetail.UnitPrice}'   ,'{orderDetail.DueDate}'   ,'{orderDetail.Note}','{0}')");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

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

        public async Task UpdateOrderMasterAsync(OrderMaster order)
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
                    var sql = string.Format("DELETE FROM [dbo].[OrderDetail]  WHERE orderid = '{0}'", order.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"UPDATE [dbo].[OrderMaster]   SET [CompanyId] = '{order.CompanyId}' ,[CustomerId] = '{order.CustomerId}' ,[IsBlanketPO] = '{order.IsBlanketPO}' ,[PONo] = '{order.PONo}' ,[PODate] = '{order.PoDate}' ,[DueDate] = '{order.DueDate}' ,[Remarks] = '{order.Remarks}'  WHERE id = '{order.Id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (OrderDetail orderDetail in order.OrderDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[OrderDetail]   ([OrderId]   ,[PartId]   ,[BlanketPOId]   ,[BlanketPOAdjQty]   ,[LineNumber]   ,[Qty]   ,[UnitPrice]   ,[DueDate]   ,[Note],[ShippedQty]) VALUES   ('{order.Id}'   ,'{orderDetail.PartId}'   ,'{orderDetail.BlanketPOId}'   ,'{orderDetail.BlanketPOAdjQty}'   ,'{orderDetail.LineNumber}'   ,'{orderDetail.Qty}'   ,'{orderDetail.UnitPrice}'   ,'{orderDetail.DueDate}'   ,'{orderDetail.Note}','{0}')");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

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

        public async Task DeleteOrderMasterAsync(long orderId)
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
                    var sql = string.Format("DELETE FROM [dbo].[OrderDetail]  WHERE orderid = '{0}'", orderId);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format("DELETE FROM [dbo].[OrderMaster]  WHERE id = '{0}'", orderId);
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

        public async Task UpdateOrderAsync(int id, string path)
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
                    string sql = string.Format($"UPDATE [dbo].[OrderMaster]   SET [Attachment] = '{path}'  WHERE Id = '{id}'");
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
