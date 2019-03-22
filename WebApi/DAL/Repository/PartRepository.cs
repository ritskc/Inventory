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
    public class PartRepository : IPartRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public PartRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<IEnumerable<Part>> GetAllPartsAsync(int companyId)
        {
            List<Part> parts = new List<Part>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location] FROM [part] where CompanyId = '{0}' ", companyId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new Part();
                    part.Id = Convert.ToInt64(dataReader["Id"]);
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    part.WeightInKg = Convert.ToDecimal(dataReader["WeightInKg"]);
                    part.WeightInLb = Convert.ToDecimal(dataReader["WeightInLb"]);
                    part.MinQty = Convert.ToInt32(dataReader["MinQty"]);
                    part.MaxQty = Convert.ToInt32(dataReader["MaxQty"]);
                    part.DrawingNo = Convert.ToString(dataReader["DrawingNo"]);
                    part.DrawingUploaded = Convert.ToBoolean(dataReader["DrawingUploaded"]);
                    part.DrawingFileName = Convert.ToString(dataReader["DrawingFileName"]);
                    part.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    part.Location = Convert.ToString(dataReader["Location"]);

                    parts.Add(part);
                }
                conn.Close();
            }

            foreach (Part part in parts)
            {
                List<PartSupplierAssignment> partSupplierAssignments = new List<PartSupplierAssignment>();
                commandText = string.Format("SELECT [id],[PartID],[SupplierID],[MapCode],[Description],[QtyInHand],[QtyInTransit],[TotalQty],[UnitPrice] FROM [partsupplierassignment]  where partid = '{0}'", part.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var partSupplierAssignment = new PartSupplierAssignment();
                        partSupplierAssignment.Id = Convert.ToInt64(dataReader1["Id"]);
                        partSupplierAssignment.PartID = Convert.ToInt64(dataReader1["PartID"]);
                        partSupplierAssignment.SupplierID = Convert.ToInt32(dataReader1["SupplierID"]);
                        partSupplierAssignment.MapCode = Convert.ToString(dataReader1["MapCode"]);
                        partSupplierAssignment.Description = Convert.ToString(dataReader1["Description"]);
                        partSupplierAssignment.QtyInHand = Convert.ToInt32(dataReader1["QtyInHand"]);
                        partSupplierAssignment.QtyInTransit = Convert.ToInt32(dataReader1["QtyInTransit"]);
                        partSupplierAssignment.TotalQty = Convert.ToInt32(dataReader1["TotalQty"]);
                        partSupplierAssignment.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);

                        partSupplierAssignments.Add(partSupplierAssignment);
                    }
                }
                part.partSupplierAssignments = partSupplierAssignments;
                conn.Close();
            }

            foreach (Part part in parts)
            {
                List<PartCustomerAssignment> partCustomerAssignments = new List<PartCustomerAssignment>();
                commandText = string.Format("SELECT [id],[PartId] ,[CustomerId] ,[MapCode] ,[Description] ,[Weight] ,[Rate] ,[SurchargeExist] ,[SurchargePerPound] ,[OpeningQty]  FROM [partcustomerassignment]  where partid = '{0}'", part.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var partCustomerAssignment = new PartCustomerAssignment();
                        partCustomerAssignment.Id = Convert.ToInt64(dataReader1["Id"]);
                        partCustomerAssignment.PartId = Convert.ToInt64(dataReader1["PartId"]);
                        partCustomerAssignment.CustomerId = Convert.ToInt32(dataReader1["CustomerId"]);
                        partCustomerAssignment.MapCode = Convert.ToString(dataReader1["MapCode"]);
                        partCustomerAssignment.Description = Convert.ToString(dataReader1["Description"]);
                        partCustomerAssignment.Weight = Convert.ToDecimal(dataReader1["Weight"]);
                        partCustomerAssignment.Rate = Convert.ToDecimal(dataReader1["Rate"]);
                        partCustomerAssignment.SurchargeExist = Convert.ToBoolean(dataReader1["SurchargeExist"]);
                        partCustomerAssignment.SurchargePerPound = Convert.ToDecimal(dataReader1["SurchargePerPound"]);
                        partCustomerAssignment.OpeningQty = Convert.ToInt32(dataReader1["OpeningQty"]);

                        partCustomerAssignments.Add(partCustomerAssignment);
                    }
                }
                part.partCustomerAssignments = partCustomerAssignments;
                conn.Close();
            }

            return parts;
        }

        public async Task<Part> GetPartAsync(long partId)
        {
            var part = new Part();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location] FROM [part] where id = '{0}' ", partId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {

                    part.Id = Convert.ToInt64(dataReader["Id"]);
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    part.WeightInKg = Convert.ToDecimal(dataReader["WeightInKg"]);
                    part.WeightInLb = Convert.ToDecimal(dataReader["WeightInLb"]);
                    part.MinQty = Convert.ToInt32(dataReader["MinQty"]);
                    part.MaxQty = Convert.ToInt32(dataReader["MaxQty"]);
                    part.DrawingNo = Convert.ToString(dataReader["DrawingNo"]);
                    part.DrawingUploaded = Convert.ToBoolean(dataReader["DrawingUploaded"]);
                    part.DrawingFileName = Convert.ToString(dataReader["DrawingFileName"]);
                    part.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    part.Location = Convert.ToString(dataReader["Location"]);

                }
                conn.Close();
            }


            List<PartSupplierAssignment> partSupplierAssignments = new List<PartSupplierAssignment>();
            commandText = string.Format("SELECT [id],[PartID],[SupplierID],[MapCode],[Description],[QtyInHand],[QtyInTransit],[TotalQty],[UnitPrice] FROM [partsupplierassignment]  where partid = '{0}'", part.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var partSupplierAssignment = new PartSupplierAssignment();
                    partSupplierAssignment.Id = Convert.ToInt64(dataReader1["Id"]);
                    partSupplierAssignment.PartID = Convert.ToInt64(dataReader1["PartID"]);
                    partSupplierAssignment.SupplierID = Convert.ToInt32(dataReader1["SupplierID"]);
                    partSupplierAssignment.MapCode = Convert.ToString(dataReader1["MapCode"]);
                    partSupplierAssignment.Description = Convert.ToString(dataReader1["Description"]);
                    partSupplierAssignment.QtyInHand = Convert.ToInt32(dataReader1["QtyInHand"]);
                    partSupplierAssignment.QtyInTransit = Convert.ToInt32(dataReader1["QtyInTransit"]);
                    partSupplierAssignment.TotalQty = Convert.ToInt32(dataReader1["TotalQty"]);
                    partSupplierAssignment.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);

                    partSupplierAssignments.Add(partSupplierAssignment);
                }

                part.partSupplierAssignments = partSupplierAssignments;
                conn.Close();
            }


            List<PartCustomerAssignment> partCustomerAssignments = new List<PartCustomerAssignment>();
            commandText = string.Format("SELECT [id],[PartId] ,[CustomerId] ,[MapCode] ,[Description] ,[Weight] ,[Rate] ,[SurchargeExist] ,[SurchargePerPound] ,[OpeningQty]  FROM [partcustomerassignment]  where partid = '{0}'", part.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var partCustomerAssignment = new PartCustomerAssignment();
                    partCustomerAssignment.Id = Convert.ToInt64(dataReader1["Id"]);
                    partCustomerAssignment.PartId = Convert.ToInt64(dataReader1["PartId"]);
                    partCustomerAssignment.CustomerId = Convert.ToInt32(dataReader1["CustomerId"]);
                    partCustomerAssignment.MapCode = Convert.ToString(dataReader1["MapCode"]);
                    partCustomerAssignment.Description = Convert.ToString(dataReader1["Description"]);
                    partCustomerAssignment.Weight = Convert.ToDecimal(dataReader1["Weight"]);
                    partCustomerAssignment.Rate = Convert.ToDecimal(dataReader1["Rate"]);
                    partCustomerAssignment.SurchargeExist = Convert.ToBoolean(dataReader1["TotaSurchargeExistlQty"]);
                    partCustomerAssignment.SurchargePerPound = Convert.ToDecimal(dataReader1["SurchargePerPound"]);
                    partCustomerAssignment.OpeningQty = Convert.ToInt32(dataReader1["OpeningQty"]);

                    partCustomerAssignments.Add(partCustomerAssignment);
                }

                part.partCustomerAssignments = partCustomerAssignments;
                conn.Close();
            }

            return part;
        }

        public async Task AddPartAsync(Part part)
        {
            string sql = string.Format($"INSERT INTO [dbo].[part]   ([Code]   ,[Description]   ,[CompanyId]   ,[WeightInKg]   ,[WeightInLb]   ,[IsSample]   ,[MinQty]   ,[MaxQty]   ,[DrawingNo]   ,[DrawingUploaded]   ,[DrawingFileName]   ,[IsActive]   ,[Location])     VALUES   ('{part.Code}'   ,'{part.Description}'   ,'{part.CompanyId}'   ,'{part.WeightInKg}'   ,'{part.WeightInLb}'   ,'{part.IsSample}'   ,'{part.MinQty}'   ,'{part.MaxQty}'   ,'{part.DrawingNo}'   ,'{part.DrawingUploaded}'   ,'{part.DrawingFileName}'   ,'{part.IsActive}'   ,'{part.Location}')");

            sql = sql + " Select Scope_Identity()";


            var partId = _sqlHelper.ExecuteScalar(ConnectionSettings.ConnectionString, sql, CommandType.Text);


            foreach (PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
            {
                sql = string.Format($"INSERT INTO [dbo].[partsupplierassignment]   ([PartID]   ,[SupplierID]   ,[MapCode]   ,[Description]   ,[QtyInHand]   ,[QtyInTransit]   ,[TotalQty]   ,[UnitPrice])     VALUES   ('{partId}'   ,'{partSupplierAssignment.SupplierID}'   ,'{partSupplierAssignment.MapCode}'   ,'{partSupplierAssignment.Description}'   ,'{partSupplierAssignment.QtyInHand}'   ,'{partSupplierAssignment.QtyInTransit}'   ,'{partSupplierAssignment.TotalQty}'   ,'{partSupplierAssignment.UnitPrice}')");

                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
            }

            foreach (PartCustomerAssignment partCustomerAssignment in part.partCustomerAssignments)
            {
                sql = string.Format($"INSERT INTO [dbo].[partcustomerassignment]   ([PartId]   ,[CustomerId]   ,[MapCode]   ,[Description]   ,[Weight]   ,[Rate]   ,[SurchargeExist]   ,[SurchargePerPound]   ,[OpeningQty])     VALUES   ('{part.Id}'   ,'{partCustomerAssignment.CustomerId}'   ,'{partCustomerAssignment.MapCode}'   ,'{partCustomerAssignment.Description}'   ,'{partCustomerAssignment.Weight}'   ,'{partCustomerAssignment.Rate}'   ,'{partCustomerAssignment.SurchargeExist}'   ,'{partCustomerAssignment.SurchargePerPound}'   ,'{partCustomerAssignment.OpeningQty}')");

                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
            }
        }

        public async Task UpdatePartAsync(Part part)
        {
            var sql = string.Format("DELETE FROM [dbo].[partsupplierassignment]  WHERE partid = '{0}'", part.Id);

            _sqlHelper.ExecuteNonQuery(ConnectionSettings.ConnectionString, sql, CommandType.Text);

            sql = string.Format("DELETE FROM [dbo].[partcustomerassignment]  WHERE partid = '{0}'", part.Id);

            _sqlHelper.ExecuteNonQuery(ConnectionSettings.ConnectionString, sql, CommandType.Text);
            
            sql = string.Format($"UPDATE [dbo].[part]   SET [Code] = '{part.Code}' ,[Description] = '{part.Description}' ,[CompanyId] = '{part.CompanyId}' ,[WeightInKg] = '{part.WeightInKg}' ,[WeightInLb] = '{part.WeightInLb}' ,[IsSample] = '{part.IsSample}' ,[MinQty] = '{part.MinQty}' ,[MaxQty] = '{part.MaxQty}' ,[DrawingNo] = '{part.DrawingNo}' ,[DrawingUploaded] = '{part.DrawingUploaded}' ,[DrawingFileName] = '{part.DrawingFileName}' ,[IsActive] = '{part.IsActive}' ,[Location] = '{part.Location}'  WHERE id = '{part.Id}' ");
            await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

            foreach (PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
            {
                sql = string.Format($"INSERT INTO [dbo].[partsupplierassignment]   ([PartID]   ,[SupplierID]   ,[MapCode]   ,[Description]   ,[QtyInHand]   ,[QtyInTransit]   ,[TotalQty]   ,[UnitPrice])     VALUES   ('{part.Id}'   ,'{partSupplierAssignment.SupplierID}'   ,'{partSupplierAssignment.MapCode}'   ,'{partSupplierAssignment.Description}'   ,'{partSupplierAssignment.QtyInHand}'   ,'{partSupplierAssignment.QtyInTransit}'   ,'{partSupplierAssignment.TotalQty}'   ,'{partSupplierAssignment.UnitPrice}')");

                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
            }

            foreach (PartCustomerAssignment partCustomerAssignment in part.partCustomerAssignments)
            {
                sql = string.Format($"INSERT INTO [dbo].[partcustomerassignment]   ([PartId]   ,[CustomerId]   ,[MapCode]   ,[Description]   ,[Weight]   ,[Rate]   ,[SurchargeExist]   ,[SurchargePerPound]   ,[OpeningQty])     VALUES   ('{part.Id}'   ,'{partCustomerAssignment.CustomerId}'   ,'{partCustomerAssignment.MapCode}'   ,'{partCustomerAssignment.Description}'   ,'{partCustomerAssignment.Weight}'   ,'{partCustomerAssignment.Rate}'   ,'{partCustomerAssignment.SurchargeExist}'   ,'{partCustomerAssignment.SurchargePerPound}'   ,'{partCustomerAssignment.OpeningQty}')");

                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
            }
        }

        public async Task<int> DeletePartAsync(long id)
        {
            var sql = string.Format("DELETE FROM [dbo].[partsupplierassignment]  WHERE partid = '{0}'", id);

            _sqlHelper.ExecuteNonQuery(ConnectionSettings.ConnectionString, sql, CommandType.Text);

            sql = string.Format("DELETE FROM [dbo].[partcustomerassignment]  WHERE partid = '{0}'", id);

            _sqlHelper.ExecuteNonQuery(ConnectionSettings.ConnectionString, sql, CommandType.Text);

            sql = string.Format("DELETE FROM [dbo].[part]  WHERE id = '{0}'", id);

            _sqlHelper.ExecuteNonQuery(ConnectionSettings.ConnectionString, sql, CommandType.Text);

            return await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
        }
    }
}
