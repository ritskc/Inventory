using DAL.IRepository;
using DAL.Models;
using DAL.Settings;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        public async Task AddTransactionAsync(TransactionDetail transactionDetail)
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
                    if (transactionDetail.TransactionTypeId == BusinessConstants.TRANSACTION_TYPE.UPLOAD_SUPPLIER_INVOICE)
                        sql = string.Format($"UPDATE [dbo].[part]   SET [IntransitQty] =  IntransitQty + '{transactionDetail.Qty}'  WHERE id = '{transactionDetail.PartId}' ");
                    else if (transactionDetail.TransactionTypeId == BusinessConstants.TRANSACTION_TYPE.RECEIVE_SUPPLIER_INVOICE)
                        sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand + '{transactionDetail.Qty}' ,[IntransitQty] =  IntransitQty -'{transactionDetail.Qty}'  WHERE id = '{transactionDetail.PartId}' ");
                    else if (transactionDetail.TransactionTypeId == BusinessConstants.TRANSACTION_TYPE.CUSTOMER_PACKINGSLIP)
                        sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand - '{transactionDetail.Qty}'  WHERE id = '{transactionDetail.PartId}' ");
                    else if (transactionDetail.TransactionTypeId == BusinessConstants.TRANSACTION_TYPE.ADJUSTMENT_MINUS)
                        sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand - '{transactionDetail.Qty}'  WHERE id = '{transactionDetail.PartId}' ");
                    else if (transactionDetail.TransactionTypeId == BusinessConstants.TRANSACTION_TYPE.ADJUSTMENT_PLUS)
                        sql = string.Format($"UPDATE [dbo].[part]   SET [QtyInHand] =  QtyInHand + '{transactionDetail.Qty}'  WHERE id = '{transactionDetail.PartId}' ");



                    command.CommandText = sql; 
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{transactionDetail.PartId}'   ,'{ Convert.ToInt32(transactionDetail.TransactionTypeId)}'   ,'{transactionDetail.TransactionDate}'   ,'{Convert.ToInt32(transactionDetail.DirectionId)}'   ,'{Convert.ToInt32(transactionDetail.InventoryType)}'   ,'{transactionDetail.ReferenceNo}'   ,'{transactionDetail.Qty}')");

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
    }
}
