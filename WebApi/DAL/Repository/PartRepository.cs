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


            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand] FROM [part] where CompanyId = '{0}' ", companyId);

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
                    part.OpeningQty = Convert.ToInt32(dataReader["OpeningQty"]);
                    part.SafeQty = Convert.ToInt32(dataReader["SafeQty"]);
                    part.DrawingNo = Convert.ToString(dataReader["DrawingNo"]);
                    part.DrawingUploaded = Convert.ToBoolean(dataReader["DrawingUploaded"]);
                    part.DrawingFileName = Convert.ToString(dataReader["DrawingFileName"]);
                    part.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    part.IsSample = Convert.ToBoolean(dataReader["IsSample"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);

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
                commandText = string.Format("SELECT [id],[PartId] ,[CustomerId] ,[MapCode] ,[Description] ,[Weight] ,[Rate] ,[SurchargeExist] ,[SurchargePerPound]  FROM [partcustomerassignment]  where partid = '{0}'", part.Id);

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

                        partCustomerAssignments.Add(partCustomerAssignment);
                    }
                }
                part.partCustomerAssignments = partCustomerAssignments;
                conn.Close();
            }

            return parts;
        }

        public Part GetPart(long partId)
        {
            var part = new Part();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand] FROM [part] where id = '{0}' ", partId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

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
                    part.IsSample = Convert.ToBoolean(dataReader["IsSample"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);

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
            commandText = string.Format("SELECT [id],[PartId] ,[CustomerId] ,[MapCode] ,[Description] ,[Weight] ,[Rate] ,[SurchargeExist] ,[SurchargePerPound]  FROM [partcustomerassignment]  where partid = '{0}'", part.Id);

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

                    partCustomerAssignments.Add(partCustomerAssignment);
                }

                part.partCustomerAssignments = partCustomerAssignments;
                conn.Close();
            }

            return part;
        }

        public async Task<Part> GetPartAsync(long partId)
        {
            var part = new Part();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand] FROM [part] where id = '{0}' ", partId);

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
                    part.IsSample = Convert.ToBoolean(dataReader["IsSample"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
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
            commandText = string.Format("SELECT [id],[PartId] ,[CustomerId] ,[MapCode] ,[Description] ,[Weight] ,[Rate] ,[SurchargeExist] ,[SurchargePerPound]  FROM [partcustomerassignment]  where partid = '{0}'", part.Id);

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

                    partCustomerAssignments.Add(partCustomerAssignment);
                }

                part.partCustomerAssignments = partCustomerAssignments;
                conn.Close();
            }

            return part;
        }

        public async Task<Part> GetPartByNameAsync(int companyId,string name)
        {
            var part = new Part();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location] FROM [part] where companyId = '{0}'  and Code = '{1}'", companyId,name);

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
                    part.IsSample = Convert.ToBoolean(dataReader["IsSample"]);
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
            commandText = string.Format("SELECT [id],[PartId] ,[CustomerId] ,[MapCode] ,[Description] ,[Weight] ,[Rate] ,[SurchargeExist] ,[SurchargePerPound]  FROM [partcustomerassignment]  where partid = '{0}'", part.Id);

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

                    partCustomerAssignments.Add(partCustomerAssignment);
                }

                part.partCustomerAssignments = partCustomerAssignments;
                conn.Close();
            }

            return part;
        }



        public async Task<Part> GetPartByMapCodeAsync(int? supplierId, string mapCode)
        {
            var part = new Part();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            
            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location] FROM [part]   where id in (SELECT[PartID] FROM[partsupplierassignment]  where[SupplierID] = '{0}' and REPLACE(Mapcode,' ','') = '{1}')", supplierId, mapCode.Replace(" ",""));

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
                    part.IsSample = Convert.ToBoolean(dataReader["IsSample"]);
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
            commandText = string.Format("SELECT [id],[PartId] ,[CustomerId] ,[MapCode] ,[Description] ,[Weight] ,[Rate] ,[SurchargeExist] ,[SurchargePerPound]  FROM [partcustomerassignment]  where partid = '{0}'", part.Id);

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

                    partCustomerAssignments.Add(partCustomerAssignment);
                }

                part.partCustomerAssignments = partCustomerAssignments;
                conn.Close();
            }

            return part;
        }



        public async Task AddPartAsync(Part part)
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
                    string sql = string.Format($"INSERT INTO [dbo].[part]   ([Code]   ,[Description]   ,[CompanyId]   ,[WeightInKg]   ,[WeightInLb]   ,[IsSample]   ,[MinQty]   ,[MaxQty]   ,[OpeningQty],[SafeQty],[DrawingNo]   ,[DrawingUploaded]   ,[DrawingFileName]   ,[IsActive]   ,[Location])     VALUES   ('{part.Code}'   ,'{part.Description}'   ,'{part.CompanyId}'   ,'{part.WeightInKg}'   ,'{part.WeightInLb}'   ,'{part.IsSample}'   ,'{part.MinQty}'   ,'{part.MaxQty}'   ,'{part.OpeningQty}'   ,'{part.SafeQty}' ,'{part.DrawingNo}'   ,'{part.DrawingUploaded}'   ,'{part.DrawingFileName}'   ,'{part.IsActive}'   ,'{part.Location}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;
                    var partId = command.ExecuteScalar();

                    foreach (PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[partsupplierassignment]   ([PartID]   ,[SupplierID]   ,[MapCode]   ,[Description]   ,[QtyInHand]   ,[QtyInTransit]   ,[TotalQty]   ,[UnitPrice])     VALUES   ('{partId}'   ,'{partSupplierAssignment.SupplierID}'   ,'{partSupplierAssignment.MapCode}'   ,'{partSupplierAssignment.Description}'   ,'{partSupplierAssignment.QtyInHand}'   ,'{partSupplierAssignment.QtyInTransit}'   ,'{partSupplierAssignment.TotalQty}'   ,'{partSupplierAssignment.UnitPrice}')");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    foreach (PartCustomerAssignment partCustomerAssignment in part.partCustomerAssignments)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[partcustomerassignment]   ([PartId]   ,[CustomerId]   ,[MapCode]   ,[Description]   ,[Weight]   ,[Rate]   ,[SurchargeExist]   ,[SurchargePerPound] )     VALUES   ('{partId}'   ,'{partCustomerAssignment.CustomerId}'   ,'{partCustomerAssignment.MapCode}'   ,'{partCustomerAssignment.Description}'   ,'{partCustomerAssignment.Weight}'   ,'{partCustomerAssignment.Rate}'   ,'{partCustomerAssignment.SurchargeExist}'   ,'{partCustomerAssignment.SurchargePerPound}'   )");

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

        public async Task UpdatePartAsync(Part part)
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
                    var sql = string.Format("DELETE FROM [dbo].[partsupplierassignment]  WHERE partid = '{0}'", part.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format("DELETE FROM [dbo].[partcustomerassignment]  WHERE partid = '{0}'", part.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"UPDATE [dbo].[part]   SET [Code] = '{part.Code}' ,[Description] = '{part.Description}' ,[CompanyId] = '{part.CompanyId}' ,[WeightInKg] = '{part.WeightInKg}' ,[WeightInLb] = '{part.WeightInLb}' ,[IsSample] = '{part.IsSample}' ,[MinQty] = '{part.MinQty}' ,[MaxQty] = '{part.MaxQty}',[OpeningQty] = '{part.OpeningQty}' ,[SafeQty] = '{part.SafeQty}' ,[DrawingNo] = '{part.DrawingNo}' ,[DrawingUploaded] = '{part.DrawingUploaded}' ,[DrawingFileName] = '{part.DrawingFileName}' ,[IsActive] = '{part.IsActive}' ,[Location] = '{part.Location}'  WHERE id = '{part.Id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[partsupplierassignment]   ([PartID]   ,[SupplierID]   ,[MapCode]   ,[Description]   ,[QtyInHand]   ,[QtyInTransit]   ,[TotalQty]   ,[UnitPrice])     VALUES   ('{part.Id}'   ,'{partSupplierAssignment.SupplierID}'   ,'{partSupplierAssignment.MapCode}'   ,'{partSupplierAssignment.Description}'   ,'{partSupplierAssignment.QtyInHand}'   ,'{partSupplierAssignment.QtyInTransit}'   ,'{partSupplierAssignment.TotalQty}'   ,'{partSupplierAssignment.UnitPrice}')");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    foreach (PartCustomerAssignment partCustomerAssignment in part.partCustomerAssignments)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[partcustomerassignment]   ([PartId]   ,[CustomerId]   ,[MapCode]   ,[Description]   ,[Weight]   ,[Rate]   ,[SurchargeExist]   ,[SurchargePerPound]  )     VALUES   ('{part.Id}'   ,'{partCustomerAssignment.CustomerId}'   ,'{partCustomerAssignment.MapCode}'   ,'{partCustomerAssignment.Description}'   ,'{partCustomerAssignment.Weight}'   ,'{partCustomerAssignment.Rate}'   ,'{partCustomerAssignment.SurchargeExist}'   ,'{partCustomerAssignment.SurchargePerPound}' )");

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


        public async Task UpdatePartCustomerPriceAsync(string customer, string partcode, decimal price)
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

                    var sql = string.Format($"UPDATE pca SET pca.Rate = '{price}' FROM dbo.[partcustomerassignment] AS pca INNER JOIN dbo.customer AS cust    ON pca.CustomerId = cust.id INNER JOIN dbo.part AS part    ON pca.PartId = part.id WHERE part.Code= '{partcode}' AND cust.Name = '{customer}'");

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

        public async Task UpdatePartSupplierPriceAsync(string supplier, string partcode, decimal price)
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

                    var sql = string.Format($"UPDATE pca SET pca.UnitPrice = '{price}' FROM dbo.[partsupplierassignment] AS pca INNER JOIN dbo.supplier AS supp  ON pca.SupplierID = supp.id INNER JOIN dbo.part AS part    ON pca.PartId = part.id WHERE part.Code= '{partcode}' AND supp.Name = '{supplier}'");

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

        public async Task DeletePartAsync(long id)
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
                    var sql = string.Format("DELETE FROM [dbo].[partsupplierassignment]  WHERE partid = '{0}'", id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format("DELETE FROM [dbo].[partcustomerassignment]  WHERE partid = '{0}'", id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format("DELETE FROM [dbo].[part]  WHERE id = '{0}'", id);
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
