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
    public class PoRepository : IPoRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public PoRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<IEnumerable<Po>> GetAllPosAsync(int companyId)
        {
            List<Po> pos = new List<Po>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[PoNo] ,[PoDate] ,[EmailIds] ,[Remarks] ,[IsClosed] ,[ClosingDate] ,[IsAcknowledged] ,[AcknowledgementDate] ,[PaymentTerms] ,[DeliveryTerms],[DueDate]  FROM [dbo].[PoMaster] where CompanyId = '{companyId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var po = new Po();
                    po.Id = Convert.ToInt64(dataReader["Id"]);
                    po.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    po.SupplierId = Convert.ToInt32(dataReader["SupplierId"]);
                    po.PoNo = Convert.ToString(dataReader["PoNo"]);
                    po.PoDate = Convert.ToDateTime(dataReader["PoDate"]);
                    po.EmailIds = Convert.ToString(dataReader["EmailIds"]);
                    po.Remarks = Convert.ToString(dataReader["Remarks"]);
                    po.IsClosed = Convert.ToBoolean(dataReader["IsClosed"]);
                    if (dataReader["ClosingDate"] != DBNull.Value)
                        po.ClosingDate = Convert.ToDateTime(dataReader["ClosingDate"]);
                    else
                        po.ClosingDate = null;

                    if (dataReader["DueDate"] != DBNull.Value)
                        po.DueDate = Convert.ToDateTime(dataReader["DueDate"]);
                    else
                        po.DueDate = null;

                    po.IsAcknowledged = Convert.ToString(dataReader["IsAcknowledged"]);

                    if (dataReader["AcknowledgementDate"] != DBNull.Value)
                        po.AcknowledgementDate = Convert.ToDateTime(dataReader["AcknowledgementDate"]);
                    else
                        po.AcknowledgementDate = null;
                    po.PaymentTerms = Convert.ToString(dataReader["PaymentTerms"]);
                    po.DeliveryTerms = Convert.ToString(dataReader["DeliveryTerms"]);

                    pos.Add(po);
                }
                dataReader.Close();
                conn.Close();
            }

            foreach (Po po in pos)
            {
                List<PoDetail> poDetails = new List<PoDetail>();
                commandText = string.Format($"SELECT [Id] ,[PoId] ,[PartId] ,[ReferenceNo] ,[Qty] ,[UnitPrice] ,[DueDate] ,[Note] ,[AckQty] ,[InTransitQty] ,[ReceivedQty] ,[IsClosed] ,[ClosingDate]  FROM [dbo].[PoDetails] where poid = '{ po.Id}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var poDetail = new PoDetail();
                        poDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                        poDetail.PoId = Convert.ToInt64(dataReader1["PoId"]);
                        poDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                        poDetail.ReferenceNo = Convert.ToString(dataReader1["ReferenceNo"]);
                        poDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                        poDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                        poDetail.DueDate = Convert.ToDateTime(dataReader1["DueDate"]);
                        poDetail.Note = Convert.ToString(dataReader1["Note"]);
                        poDetail.AckQty = Convert.ToInt32(dataReader1["AckQty"]);
                        poDetail.InTransitQty = Convert.ToInt32(dataReader1["InTransitQty"]);
                        poDetail.ReceivedQty = Convert.ToInt32(dataReader1["ReceivedQty"]);
                        poDetail.IsClosed = Convert.ToBoolean(dataReader1["IsClosed"]);
                        if (dataReader1["ClosingDate"] != DBNull.Value)
                            po.ClosingDate = Convert.ToDateTime(dataReader1["ClosingDate"]);
                        else
                            po.ClosingDate = null;

                        poDetails.Add(poDetail);
                    }
                    dataReader1.Close();
                }
                po.poDetails = poDetails;
                conn.Close();
            }

            foreach (Po po in pos)
            {
                List<PoTerm> poTerms = new List<PoTerm>();
                commandText = string.Format("SELECT [Id] ,[PoId] ,[SequenceNo] ,[Term]  FROM [dbo].[PoTerms] where poid = '{0}'", po.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var poTerm = new PoTerm();
                        poTerm.Id = Convert.ToInt64(dataReader1["Id"]);
                        poTerm.PoId = Convert.ToInt64(dataReader1["PoId"]);
                        poTerm.SequenceNo = Convert.ToInt32(dataReader1["SequenceNo"]);
                        poTerm.Term = Convert.ToString(dataReader1["Term"]);                       

                        poTerms.Add(poTerm);
                    }
                }
                po.poTerms = poTerms;
                conn.Close();
            }

            return pos.OrderBy(x=>x.DueDate);
        }

        public async Task<Po> GetPoAsync(long poId)
        {
            var po = new Po();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[PoNo] ,[PoDate] ,[EmailIds] ,[Remarks] ,[IsClosed] ,[ClosingDate] ,[IsAcknowledged] ,[AcknowledgementDate] ,[PaymentTerms] ,[DeliveryTerms],[DueDate]  FROM [dbo].[PoMaster] where Id = '{poId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {

                    po.Id = Convert.ToInt64(dataReader["Id"]);
                    po.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    po.SupplierId = Convert.ToInt32(dataReader["SupplierId"]);
                    po.PoNo = Convert.ToString(dataReader["PoNo"]);
                    po.PoDate = Convert.ToDateTime(dataReader["PoDate"]);
                    po.EmailIds = Convert.ToString(dataReader["EmailIds"]);
                    po.Remarks = Convert.ToString(dataReader["Remarks"]);
                    po.IsClosed = Convert.ToBoolean(dataReader["IsClosed"]);
                    if (dataReader["ClosingDate"] != DBNull.Value)
                        po.ClosingDate = Convert.ToDateTime(dataReader["ClosingDate"]);
                    else
                        po.ClosingDate = null;

                    if (dataReader["DueDate"] != DBNull.Value)
                        po.DueDate = Convert.ToDateTime(dataReader["DueDate"]);
                    else
                        po.DueDate = null;


                    po.IsAcknowledged = Convert.ToString(dataReader["IsAcknowledged"]);
                   
                    if (dataReader["AcknowledgementDate"] != DBNull.Value)
                        po.AcknowledgementDate = Convert.ToDateTime(dataReader["AcknowledgementDate"]);
                    else
                        po.AcknowledgementDate = null;
                    po.PaymentTerms = Convert.ToString(dataReader["PaymentTerms"]);
                    po.DeliveryTerms = Convert.ToString(dataReader["DeliveryTerms"]);

                }
                dataReader.Close();
                conn.Close();
            }


            List<PoDetail> poDetails = new List<PoDetail>();
            commandText = string.Format($"SELECT [Id] ,[PoId] ,[PartId] ,[ReferenceNo] ,[Qty] ,[UnitPrice] ,[DueDate] ,[Note] ,[AckQty] ,[InTransitQty] ,[ReceivedQty] ,[IsClosed] ,[ClosingDate]  FROM [dbo].[PoDetails] where poid = '{ po.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var poDetail = new PoDetail();
                    poDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                    poDetail.PoId = Convert.ToInt64(dataReader1["PoId"]);
                    poDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                    poDetail.ReferenceNo = Convert.ToString(dataReader1["ReferenceNo"]);
                    poDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                    poDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                    poDetail.DueDate = Convert.ToDateTime(dataReader1["DueDate"]);
                    poDetail.Note = Convert.ToString(dataReader1["Note"]);
                    poDetail.AckQty = Convert.ToInt32(dataReader1["AckQty"]);
                    poDetail.InTransitQty = Convert.ToInt32(dataReader1["InTransitQty"]);
                    poDetail.ReceivedQty = Convert.ToInt32(dataReader1["ReceivedQty"]);
                    poDetail.IsClosed = Convert.ToBoolean(dataReader1["IsClosed"]);               

                    if (dataReader1["ClosingDate"] != DBNull.Value)
                        po.ClosingDate = Convert.ToDateTime(dataReader1["ClosingDate"]);
                    else
                        po.ClosingDate = null;

                    poDetails.Add(poDetail);
                }
                dataReader1.Close();
                conn.Close();
            }
            po.poDetails = poDetails;
            
            
            List<PoTerm> poTerms = new List<PoTerm>();
            commandText = string.Format("SELECT [Id] ,[PoId] ,[SequenceNo] ,[Term]  FROM [dbo].[PoTerms] where poid = '{0}'", po.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var poTerm = new PoTerm();
                    poTerm.Id = Convert.ToInt64(dataReader1["Id"]);
                    poTerm.PoId = Convert.ToInt64(dataReader1["PoId"]);
                    poTerm.SequenceNo = Convert.ToInt32(dataReader1["SequenceNo"]);
                    poTerm.Term = Convert.ToString(dataReader1["Term"]);

                    poTerms.Add(poTerm);
                }
                dataReader1.Close();
                conn.Close();
            }
            po.poTerms = poTerms;
           
        

            return po;
        }

        public async Task<Po> GetPoAsync(long poId, SqlConnection conn,SqlTransaction transaction)
        {
            var po = new Po();           

            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[PoNo] ,[PoDate] ,[EmailIds] ,[Remarks] ,[IsClosed] ,[ClosingDate] ,[IsAcknowledged] ,[AcknowledgementDate] ,[PaymentTerms] ,[DeliveryTerms],[DueDate]  FROM [dbo].[PoMaster] where Id = '{poId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn,transaction))
            {
                cmd.CommandType = CommandType.Text;                

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                while (dataReader.Read())
                {

                    po.Id = Convert.ToInt64(dataReader["Id"]);
                    po.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    po.SupplierId = Convert.ToInt32(dataReader["SupplierId"]);
                    po.PoNo = Convert.ToString(dataReader["PoNo"]);
                    po.PoDate = Convert.ToDateTime(dataReader["PoDate"]);
                    po.EmailIds = Convert.ToString(dataReader["EmailIds"]);
                    po.Remarks = Convert.ToString(dataReader["Remarks"]);
                    po.IsClosed = Convert.ToBoolean(dataReader["IsClosed"]);
                    if (dataReader["ClosingDate"] != DBNull.Value)
                        po.ClosingDate = Convert.ToDateTime(dataReader["ClosingDate"]);
                    else
                        po.ClosingDate = null;

                    if (dataReader["DueDate"] != DBNull.Value)
                        po.DueDate = Convert.ToDateTime(dataReader["DueDate"]);
                    else
                        po.DueDate = null;

                    po.IsAcknowledged = Convert.ToString(dataReader["IsAcknowledged"]);

                    if (dataReader["AcknowledgementDate"] != DBNull.Value)
                        po.AcknowledgementDate = Convert.ToDateTime(dataReader["AcknowledgementDate"]);
                    else
                        po.AcknowledgementDate = null;
                    po.PaymentTerms = Convert.ToString(dataReader["PaymentTerms"]);
                    po.DeliveryTerms = Convert.ToString(dataReader["DeliveryTerms"]);

                }

                dataReader.Close();
            }


            List<PoDetail> poDetails = new List<PoDetail>();
            commandText = string.Format($"SELECT [Id] ,[PoId] ,[PartId] ,[ReferenceNo] ,[Qty] ,[UnitPrice] ,[DueDate] ,[Note] ,[AckQty] ,[InTransitQty] ,[ReceivedQty] ,[IsClosed] ,[ClosingDate]  FROM [dbo].[PoDetails] where poid = '{ po.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn, transaction))
            {
                cmd1.CommandType = CommandType.Text;                
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);

                while (dataReader1.Read())
                {
                    var poDetail = new PoDetail();
                    poDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                    poDetail.PoId = Convert.ToInt64(dataReader1["PoId"]);
                    poDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                    poDetail.ReferenceNo = Convert.ToString(dataReader1["ReferenceNo"]);
                    poDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                    poDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                    poDetail.DueDate = Convert.ToDateTime(dataReader1["DueDate"]);
                    poDetail.Note = Convert.ToString(dataReader1["Note"]);
                    poDetail.AckQty = Convert.ToInt32(dataReader1["AckQty"]);
                    poDetail.InTransitQty = Convert.ToInt32(dataReader1["InTransitQty"]);
                    poDetail.ReceivedQty = Convert.ToInt32(dataReader1["ReceivedQty"]);
                    poDetail.IsClosed = Convert.ToBoolean(dataReader1["IsClosed"]);

                    if (dataReader1["ClosingDate"] != DBNull.Value)
                        po.ClosingDate = Convert.ToDateTime(dataReader1["ClosingDate"]);
                    else
                        po.ClosingDate = null;

                    poDetails.Add(poDetail);
                }

                dataReader1.Close();
            }
            po.poDetails = poDetails;


            List<PoTerm> poTerms = new List<PoTerm>();
            commandText = string.Format("SELECT [Id] ,[PoId] ,[SequenceNo] ,[Term]  FROM [dbo].[PoTerms] where poid = '{0}'", po.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn, transaction))
            {
                cmd1.CommandType = CommandType.Text;                
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);

                while (dataReader1.Read())
                {
                    var poTerm = new PoTerm();
                    poTerm.Id = Convert.ToInt64(dataReader1["Id"]);
                    poTerm.PoId = Convert.ToInt64(dataReader1["PoId"]);
                    poTerm.SequenceNo = Convert.ToInt32(dataReader1["SequenceNo"]);
                    poTerm.Term = Convert.ToString(dataReader1["Term"]);

                    poTerms.Add(poTerm);
                }

                dataReader1.Close();
            }
            po.poTerms = poTerms;

            return po;
        }

        public async Task AddPoAsync(Po po)
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
                    sql = string.Format($"INSERT INTO [dbo].[PoMaster]   ([CompanyId]   ,[SupplierId]   ,[PoNo]   ,[PoDate]   ,[EmailIds]   ,[Remarks]   ,[IsClosed]  ,[IsAcknowledged]   ,[PaymentTerms]   ,[DeliveryTerms],[DueDate]) VALUES   ('{po.CompanyId}'   ,'{po.SupplierId}'   ,'{po.PoNo}'   ,'{po.PoDate}'   ,'{po.EmailIds}'   ,'{po.Remarks}'   ,'{po.IsClosed}'   ,'{po.IsAcknowledged}'  ,'{po.PaymentTerms}'   ,'{po.DeliveryTerms}' ,'{po.DueDate}')");

                    sql = sql + " Select Scope_Identity()";

                    command.CommandText = sql;
                    var poId = command.ExecuteScalar();


                    //var poId = _sqlHelper.ExecuteScalar(ConnectionSettings.ConnectionString, sql, CommandType.Text);


                    foreach (PoDetail poDetail in po.poDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[PoDetails]   ([PoId]   ,[PartId]   ,[ReferenceNo]   ,[Qty]   ,[UnitPrice]   ,[DueDate]   ,[Note]   ,[AckQty]   ,[InTransitQty]   ,[ReceivedQty]   ,[IsClosed]  ) VALUES   ('{poId}'   ,'{poDetail.PartId}'   ,'{poDetail.ReferenceNo}'   ,'{poDetail.Qty}'   ,'{poDetail.UnitPrice}'   ,'{poDetail.DueDate}'   ,'{poDetail.Note}'   ,'{poDetail.AckQty}'   ,'{poDetail.InTransitQty}'   ,'{poDetail.ReceivedQty}'   ,'{poDetail.IsClosed}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    foreach (PoTerm poTerm in po.poTerms)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[PoTerms]   ([PoId]   ,[SequenceNo]   ,[Term]) VALUES   ('{poId}'   ,'{poTerm.SequenceNo}'   ,'{poTerm.Term}')");
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

        public async Task UpdatePoAsync(Po po)
        {
            //start
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
                    sql = string.Format("DELETE FROM [dbo].[PoTerms]  WHERE poid = '{0}'", po.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                    
                    sql = string.Format("DELETE FROM [dbo].[PoDetails]  WHERE poid = '{0}'", po.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"UPDATE [dbo].[PoMaster]   SET [CompanyId] = '{po.CompanyId}' ,[SupplierId] = '{po.SupplierId}' ,[PoNo] = '{po.PoNo}' ,[PoDate] = '{po.PoDate}' ,[EmailIds] = '{po.EmailIds}' ,[Remarks] = '{po.Remarks}' ,[IsClosed] = '{po.IsClosed}' ,[IsAcknowledged] = '{po.IsAcknowledged}' ,[PaymentTerms] = '{po.PaymentTerms}' ,[DeliveryTerms] = '{po.DeliveryTerms}',[DueDate] = '{po.DueDate}' WHERE id = '{po.Id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (PoDetail poDetail in po.poDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[PoDetails]   ([PoId]   ,[PartId]   ,[ReferenceNo]   ,[Qty]   ,[UnitPrice]   ,[DueDate]   ,[Note]   ,[AckQty]   ,[InTransitQty]   ,[ReceivedQty]   ,[IsClosed] ) VALUES   ('{po.Id}'   ,'{poDetail.PartId}'   ,'{poDetail.ReferenceNo}'   ,'{poDetail.Qty}'   ,'{poDetail.UnitPrice}'   ,'{poDetail.DueDate}'   ,'{poDetail.Note}'   ,'{poDetail.AckQty}'   ,'{poDetail.InTransitQty}'   ,'{poDetail.ReceivedQty}'   ,'{poDetail.IsClosed}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    foreach (PoTerm poTerm in po.poTerms)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[PoTerms]   ([PoId]   ,[SequenceNo]   ,[Term]) VALUES   ('{po.Id}'   ,'{poTerm.SequenceNo}'   ,'{poTerm.Term}')");
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
            //end           
        }

        public async Task<int> DeletePoAsync(long id)
        {
            int result = 0;
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
                    sql = string.Format("DELETE FROM [dbo].[PoTerms]  WHERE poid = '{0}'", id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();                    

                    sql = string.Format("DELETE FROM [dbo].[PoDetails]  WHERE poid = '{0}'", id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();                    

                    sql = string.Format("DELETE FROM [dbo].[PoMaster]  WHERE id = '{0}'", id);
                    command.CommandText = sql;
                    result = await command.ExecuteNonQueryAsync();                                       

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return result;
        }
    }
}
