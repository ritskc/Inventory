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
        private readonly IUserRepository userRepository;

        public OrderRepository(ISqlHelper sqlHelper, IUserRepository userRepository)
        {
            _sqlHelper = sqlHelper;
            this.userRepository = userRepository;
        }       

        public async Task<IEnumerable<OrderMaster>> GetAllOrderMastersAsync(int companyId, int userId)
        {
            List<OrderMaster> orders = new List<OrderMaster>();

            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format("SELECT [Id] ,[CompanyId] ,[CustomerId] ,[IsBlanketPO] ,[PONo] ,[PODate] ,[DueDate] ,[Remarks],[IsClosed] ,[ClosingDate]  FROM [dbo].[OrderMaster] where CompanyId = '{0}' ", companyId);
            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);

                commandText = string.Format("SELECT [Id] ,[CompanyId] ,[CustomerId] ,[IsBlanketPO] ,[PONo] ,[PODate] ,[DueDate] ,[Remarks],[IsClosed] ,[ClosingDate]  FROM [dbo].[OrderMaster] where CompanyId = '{0}'  " +
                    "and  [CustomerId] in ({1}) ", companyId, companylist);               
            }
            if (userInfo.UserTypeId == 3)
            {
                return orders;
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);            

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
                    if (dataReader["IsClosed"] != DBNull.Value)
                        order.IsClosed = Convert.ToBoolean(dataReader["IsClosed"]);
                    else
                        order.IsClosed = false;
                    if (dataReader["ClosingDate"] != DBNull.Value)
                        order.ClosingDate = Convert.ToDateTime(dataReader["ClosingDate"]);
                    else
                        order.ClosingDate = null;
                    orders.Add(order);
                }
                dataReader.Close();
                conn.Close();
            }

            foreach (OrderMaster order in orders)
            {
                List<OrderDetail> orderDetails = new List<OrderDetail>();
                commandText = string.Format("SELECT [Id] ,[OrderId] ,[PartId] ,[BlanketPOId] ,[BlanketPOAdjQty] ,[LineNumber] ,[Qty] ,[UnitPrice] " +
                    ",[DueDate] ,[Note],[ShippedQty],[IsClosed] ,[ClosingDate],[SrNo],[IsForceClosed]  FROM [dbo].[OrderDetail]  where orderid = '{0}'", order.Id);

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
                        if (dataReader1["IsClosed"] != DBNull.Value)
                            orderDetail.IsClosed = Convert.ToBoolean(dataReader1["IsClosed"]);
                        else
                            orderDetail.IsClosed = false;
                        if (dataReader1["ClosingDate"] != DBNull.Value)
                            orderDetail.ClosingDate = Convert.ToDateTime(dataReader1["ClosingDate"]);
                        else
                            orderDetail.ClosingDate = null;

                        orderDetail.IsForceClosed = Convert.ToBoolean(dataReader1["IsForceClosed"]);

                        if (dataReader1["SrNo"] != DBNull.Value)
                            orderDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                        else
                            orderDetail.SrNo = 0;

                        orderDetails.Add(orderDetail);
                    }
                    dataReader1.Close();
                }                
                order.OrderDetails = orderDetails;
                conn.Close();
            }

            return orders.OrderBy(x=>x.PoDate);
        }

        public async Task<OrderMaster> GetOrderMasterAsync(long orderId)
        {
            OrderMaster order = new OrderMaster();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = string.Format("SELECT [Id] ,[CompanyId] ,[CustomerId] ,[IsBlanketPO] ,[PONo] ,[PODate] ,[DueDate] ,[Remarks],[IsClosed] ,[ClosingDate]  FROM [dbo].[OrderMaster] where Id = '{0}' ", orderId);

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
                    if (dataReader["IsClosed"] != DBNull.Value)
                        order.IsClosed = Convert.ToBoolean(dataReader["IsClosed"]);
                    else
                        order.IsClosed = false;
                    if (dataReader["ClosingDate"] != DBNull.Value)
                        order.ClosingDate = Convert.ToDateTime(dataReader["ClosingDate"]);
                    else
                        order.ClosingDate = null;
                }
                dataReader.Close();
                conn.Close();
            }


            List<OrderDetail> orderDetails = new List<OrderDetail>();
            commandText = string.Format("SELECT [Id] ,[OrderId] ,[PartId] ,[BlanketPOId] ,[BlanketPOAdjQty] ,[LineNumber] ,[Qty] ,[UnitPrice] " +
                ",[DueDate] ,[Note],[ShippedQty],[IsClosed] ,[ClosingDate],[SrNo],[IsForceClosed]  FROM [dbo].[OrderDetail]  where orderid = '{0}'", order.Id);

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
                    if (dataReader1["IsClosed"] != DBNull.Value)
                        orderDetail.IsClosed = Convert.ToBoolean(dataReader1["IsClosed"]);
                    else
                        orderDetail.IsClosed = false;
                    if (dataReader1["ClosingDate"] != DBNull.Value)
                        orderDetail.ClosingDate = Convert.ToDateTime(dataReader1["ClosingDate"]);
                    else
                        orderDetail.ClosingDate = null;

                    if (dataReader1["SrNo"] != DBNull.Value)
                        orderDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                    else
                        orderDetail.SrNo = 0;
                    orderDetail.IsForceClosed = Convert.ToBoolean(dataReader1["IsForceClosed"]);
                    orderDetails.Add(orderDetail);
                }
                dataReader1.Close();
            }
            order.OrderDetails = orderDetails;
            conn.Close();
            return order;
        }

        public async Task<OrderMaster> GetOrderMasterAsync(long orderId, SqlConnection conn, SqlTransaction transaction)
        {
            OrderMaster order = new OrderMaster();
            var commandText = string.Format("SELECT [Id] ,[CompanyId] ,[CustomerId] ,[IsBlanketPO] ,[PONo] ,[PODate] ,[DueDate] ,[Remarks],[IsClosed] ,[ClosingDate]  FROM [dbo].[OrderMaster] where Id = '{0}' ", orderId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn,transaction))
            {
                cmd.CommandType = CommandType.Text;              

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

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
                    if (dataReader["IsClosed"] != DBNull.Value)
                        order.IsClosed = Convert.ToBoolean(dataReader["IsClosed"]);
                    else
                        order.IsClosed =false;
                    if (dataReader["ClosingDate"] != DBNull.Value)
                        order.ClosingDate = Convert.ToDateTime(dataReader["ClosingDate"]);
                    else
                        order.ClosingDate = null;

                }
                dataReader.Close();
            }


            List<OrderDetail> orderDetails = new List<OrderDetail>();
            commandText = string.Format("SELECT [Id] ,[OrderId] ,[PartId] ,[BlanketPOId] ,[BlanketPOAdjQty] ,[LineNumber] ,[Qty] ,[UnitPrice] " +
                ",[DueDate] ,[Note],[ShippedQty],[IsClosed] ,[ClosingDate],[SrNo],[IsForceClosed]  FROM [dbo].[OrderDetail]  where orderid = '{0}'", order.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn,transaction))
            {
                cmd1.CommandType = CommandType.Text;               
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);

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
                    if (dataReader1["IsClosed"] != DBNull.Value)
                        orderDetail.IsClosed = Convert.ToBoolean(dataReader1["IsClosed"]);
                    else
                        orderDetail.IsClosed = false;
                    if (dataReader1["ClosingDate"] != DBNull.Value)
                        orderDetail.ClosingDate = Convert.ToDateTime(dataReader1["ClosingDate"]);
                    else
                        orderDetail.ClosingDate = null;

                    if (dataReader1["SrNo"] != DBNull.Value)
                        orderDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                    else
                        orderDetail.SrNo = 0;
                    orderDetail.IsForceClosed = Convert.ToBoolean(dataReader1["IsForceClosed"]);

                    orderDetails.Add(orderDetail);
                }
                dataReader1.Close();
            }
            order.OrderDetails = orderDetails;
            
            return order;
        }

        public async Task<long> AddOrderMasterAsync(OrderMaster order)
        {
            long orderId = 0;
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
                    string sql = string.Format($"INSERT INTO [dbo].[OrderMaster]   ([CompanyId]   ,[CustomerId]   ,[IsBlanketPO]   ,[PONo]   ,[PODate]   ,[DueDate]   ,[Remarks],[IsClosed]) VALUES   ('{order.CompanyId}'   ,'{order.CustomerId}'   ,'{order.IsBlanketPO}'   ,'{order.PONo}'   ,'{order.PoDate}'   ,'{order.DueDate}'   ,'{order.Remarks}','{false}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;

                    var id = command.ExecuteScalar();
                    orderId = Convert.ToInt64(id.ToString());

                    foreach (OrderDetail orderDetail in order.OrderDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[OrderDetail]   ([OrderId]   ,[PartId]   ,[BlanketPOId]   ,[BlanketPOAdjQty]   ,[LineNumber]   ,[Qty]   ,[UnitPrice]   ,[DueDate]   ,[Note],[ShippedQty],[SrNo],[IsForceClosed],[IsClosed]) VALUES   ('{id}'   ,'{orderDetail.PartId}'   ,'{orderDetail.BlanketPOId}'   ,'{orderDetail.BlanketPOAdjQty}'   ,'{orderDetail.LineNumber}'   ,'{orderDetail.Qty}'   ,'{orderDetail.UnitPrice}'   ,'{orderDetail.DueDate}'   ,'{orderDetail.Note}','{0}','{orderDetail.SrNo}','{false}','{false}')");

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
                return orderId;
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
                transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted, "SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    //var sql = string.Format("DELETE FROM [dbo].[OrderDetail]  WHERE orderid = '{0}'", order.Id);
                    //command.CommandText = sql;
                    //await command.ExecuteNonQueryAsync();
                    var orderResult = await GetOrderMasterAsync(order.Id, command.Connection, command.Transaction);
                    foreach (OrderDetail orderDetail in orderResult.OrderDetails)
                    {
                        var deleteLine = order.OrderDetails.Where(x => x.Id == orderDetail.Id).FirstOrDefault();
                        if(deleteLine == null)
                        {
                            var sql1 = string.Format($"DELETE [dbo].[OrderDetail]   WHERE id = '{orderDetail.Id}' ");
                            command.CommandText = sql1;
                            await command.ExecuteNonQueryAsync();
                        }
                    }

                    var sql = string.Format($"UPDATE [dbo].[OrderMaster]   SET [CompanyId] = '{order.CompanyId}' ,[CustomerId] = '{order.CustomerId}' ,[IsBlanketPO] = '{order.IsBlanketPO}' ,[PONo] = '{order.PONo}' ,[PODate] = '{order.PoDate}' ,[DueDate] = '{order.DueDate}' ,[Remarks] = '{order.Remarks}'  WHERE id = '{order.Id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (OrderDetail orderDetail in order.OrderDetails)
                    {
                        if (orderDetail.Id == 0)
                        {
                            if (orderDetail.IsForceClosed)
                                sql = string.Format($"INSERT INTO [dbo].[OrderDetail]   ([OrderId]   ,[PartId]   ,[BlanketPOId]   ,[BlanketPOAdjQty]   ,[LineNumber]   ,[Qty]   ,[UnitPrice]   ,[DueDate]   ,[Note],[ShippedQty],[SrNo],[IsForceClosed],[IsClosed],[ClosingDate]) VALUES   ('{order.Id}'   ,'{orderDetail.PartId}'   ,'{orderDetail.BlanketPOId}'   ,'{orderDetail.BlanketPOAdjQty}'   ,'{orderDetail.LineNumber}'   ,'{orderDetail.Qty}'   ,'{orderDetail.UnitPrice}'   ,'{orderDetail.DueDate}'   ,'{orderDetail.Note}','{0}','{orderDetail.SrNo}','{true}','{true}','{DateTime.Now}')");
                            else
                                sql = string.Format($"INSERT INTO [dbo].[OrderDetail]   ([OrderId]   ,[PartId]   ,[BlanketPOId]   ,[BlanketPOAdjQty]   ,[LineNumber]   ,[Qty]   ,[UnitPrice]   ,[DueDate]   ,[Note],[ShippedQty],[SrNo],[IsForceClosed],[IsClosed]) VALUES   ('{order.Id}'   ,'{orderDetail.PartId}'   ,'{orderDetail.BlanketPOId}'   ,'{orderDetail.BlanketPOAdjQty}'   ,'{orderDetail.LineNumber}'   ,'{orderDetail.Qty}'   ,'{orderDetail.UnitPrice}'   ,'{orderDetail.DueDate}'   ,'{orderDetail.Note}','{0}','{orderDetail.SrNo}','{false}','{false}')");

                        }
                        else
                        {
                            if (orderDetail.IsForceClosed || (orderDetail.Qty == orderDetail.ShippedQty))
                            {
                                sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET  [LineNumber] = '{orderDetail.LineNumber}' ,[Qty] = '{orderDetail.Qty}'  ,[UnitPrice] = '{orderDetail.UnitPrice}', [DueDate] = '{orderDetail.DueDate}' ,[Note] = '{orderDetail.Note}',[IsClosed] = '{true}',[ClosingDate] ='{DateTime.Now}' ,[SrNo] ='{orderDetail.SrNo}',[IsForceClosed] = '{orderDetail.IsForceClosed }' WHERE id = '{orderDetail.Id}'");
                            }
                            else
                            {
                                if (orderDetail.Qty == orderDetail.ShippedQty)
                                    orderDetail.IsClosed = true;
                                else
                                    orderDetail.IsClosed = false;
                                sql = string.Format($"UPDATE [dbo].[OrderDetail]   SET  [LineNumber] = '{orderDetail.LineNumber}' ,[Qty] = '{orderDetail.Qty}'  ,[UnitPrice] = '{orderDetail.UnitPrice}', [DueDate] = '{orderDetail.DueDate}' ,[Note] = '{orderDetail.Note}',[SrNo] ='{orderDetail.SrNo}',[IsForceClosed] = '{orderDetail.IsForceClosed }',[IsClosed] = '{orderDetail.IsClosed}' WHERE id = '{orderDetail.Id}'");
                            }
                        }

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    orderResult = await GetOrderMasterAsync(order.Id, command.Connection, command.Transaction);
                    var openPO = orderResult.OrderDetails.Where(x => x.OrderId == order.Id && x.IsClosed == false).FirstOrDefault();

                    if (openPO == null)
                    {
                        sql = string.Format($"UPDATE [dbo].[OrderMaster]   SET [IsClosed] = '{true}' ,[ClosingDate] = '{DateTime.Now}'  WHERE id = '{order.Id}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                        //await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
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
