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
        private readonly IUserRepository userRepository;

        public PartRepository(ISqlHelper sqlHelper, IUserRepository userRepository)
        {
            _sqlHelper = sqlHelper;
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<Part>> GetAllPartsAsync(int companyId, int userId)
        {          
            List<Part> parts = new List<Part>();

            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] where CompanyId = '{0}' ", companyId);
            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partcustomerassignment PCA ON PCA.PartId = PM.id where CompanyId = '{companyId}' AND PCA.CustomerId IN ({companylist}) ");
            }
            if (userInfo.UserTypeId == 3)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],PM.[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partsupplierassignment PCA ON PCA.PartId = PM.id where CompanyId ='{companyId}' AND PCA.SupplierID IN ({companylist}) ");
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);
                    part.DefaultWarehouse = Convert.ToBoolean(dataReader["DefaultWarehouse"]);
                    part.WarehouseId = Convert.ToInt32(dataReader["WarehouseId"]);

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
                        if (companyId == 1)
                            part.SupplierPrice = partSupplierAssignment.UnitPrice;

                        partSupplierAssignments.Add(partSupplierAssignment);
                    }
                    dataReader1.Close();
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
                        if (companyId == 1)
                            part.CustomerPrice = partCustomerAssignment.Rate;
                        partCustomerAssignment.SurchargeExist = Convert.ToBoolean(dataReader1["SurchargeExist"]);
                        partCustomerAssignment.SurchargePerPound = Convert.ToDecimal(dataReader1["SurchargePerPound"]);

                        partCustomerAssignments.Add(partCustomerAssignment);
                    }
                    dataReader1.Close();
                }
                part.partCustomerAssignments = partCustomerAssignments;

                conn.Close();
            }

            
            foreach (Part part in parts)
            {                
                commandText = string.Format("SELECT sum(Qty - ShippedQty) as openqty from OrderDetail where partid = '{0}' and (IsClosed =0 OR IsClosed IS NULL) ", part.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        try
                        {
                            part.OpenOrderQty = Convert.ToInt32(dataReader1["openqty"]);
                        }
                        catch
                        {
                            part.OpenOrderQty = 0;
                        }

                    }
                    dataReader1.Close();
                }
                conn.Close();


                commandText = string.Format("SELECT sum(Qty - (InTransitQty + ReceivedQty )) as openqty from PoDetails where partid = '{0}' and (IsClosed =0 OR IsClosed IS NULL) ", part.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        try
                        {
                            part.SupplierOpenPoQty = Convert.ToInt32(dataReader1["openqty"]);
                        }
                        catch
                        {
                            part.SupplierOpenPoQty = 0;
                        }

                    }
                    dataReader1.Close();
                }

                commandText = string.Format("SELECT [id]  ,[PartId]  ,[SupplierPrice] ,[CustomerPrice] ,[Qty] FROM [StockPrice] where partid = '{0}' order by id", part.Id);
                part.stockPrices = new List<StockPrice>();
                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var stockPrice = new StockPrice();
                        stockPrice.Id = Convert.ToInt32(dataReader1["Id"]);
                        stockPrice.PartId = Convert.ToInt32(dataReader1["PartId"]);
                        stockPrice.SupplierPrice = Convert.ToDecimal(dataReader1["SupplierPrice"]);
                        stockPrice.CustomerPrice = Convert.ToDecimal(dataReader1["CustomerPrice"]);
                        stockPrice.Qty = Convert.ToInt32(dataReader1["Qty"]);                       

                        part.stockPrices.Add(stockPrice);

                    }
                    dataReader1.Close();
                }

                conn.Close();
            }

            return parts.OrderBy(x => x.Code);
        }


        public async Task<IEnumerable<Part>> GetAllPartsbyWarehouseAsync(int companyId, int userId, int warehouseId)
        {
            if(warehouseId == 0)
            {
                var result = await GetAllPartsAsync(companyId, userId);

                var commandText1 = "";
                var userInfo1 = await userRepository.GeUserbyIdAsync(userId);
                if (userInfo1.UserTypeId == 1)
                {
                    commandText1 = string.Format($" SELECT partid, sum(PWI.[QtyInHand]) [QtyInHand] FROM [part] INNER JOIN [PartWarehouseInventory] PWI ON PWI.PARTID = PART.ID where PART.CompanyId ='{companyId}'  group by partid");
                }
                if (userInfo1.UserTypeId == 2)
                {
                    string companylist = string.Join(",", userInfo1.CompanyIds);
                    commandText1 = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partcustomerassignment PCA ON PCA.PartId = PM.id where CompanyId = '{companyId}' AND PCA.CustomerId IN ({companylist}) ");
                }
                if (userInfo1.UserTypeId == 3)
                {
                    string companylist = string.Join(",", userInfo1.CompanyIds);
                    commandText1 = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],PM.[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partsupplierassignment PCA ON PCA.PartId = PM.id where CompanyId ='{companyId}' AND PCA.SupplierID IN ({companylist}) ");
                }

                SqlConnection conn1 = new SqlConnection(ConnectionSettings.ConnectionString);

                using (SqlCommand cmd = new SqlCommand(commandText1, conn1))
                {
                    cmd.CommandType = CommandType.Text;

                    conn1.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        foreach (Part part in result)
                        {
                            if (Convert.ToInt32(dataReader["partid"]) == part.Id)
                            {
                                part.QtyInHand = part.QtyInHand + part.OpeningQty - Convert.ToInt32(dataReader["QtyInHand"]);
                                part.OpeningQty = 0;
                            }
                        }
                       
                    }
                    conn1.Close();
                }

                return result;
            }
            List<Part> parts = new List<Part>();

            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format($" SELECT PART.[id],[Code],[Description],PART.[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],PWI.[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],PWI.[Location],[IntransitQty],PWI.[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],PWI.[WarehouseId] FROM [part] INNER JOIN [PartWarehouseInventory] PWI ON PWI.PARTID = PART.ID where PART.CompanyId = '{companyId}' AND PWI.[WarehouseId] = {warehouseId}");
            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partcustomerassignment PCA ON PCA.PartId = PM.id where CompanyId = '{companyId}' AND PCA.CustomerId IN ({companylist}) ");
            }
            if (userInfo.UserTypeId == 3)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],PM.[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partsupplierassignment PCA ON PCA.PartId = PM.id where CompanyId ='{companyId}' AND PCA.SupplierID IN ({companylist}) ");
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

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
                    part.OpeningQty = 0;
                    part.SafeQty = Convert.ToInt32(dataReader["SafeQty"]);
                    part.DrawingNo = Convert.ToString(dataReader["DrawingNo"]);
                    part.DrawingUploaded = Convert.ToBoolean(dataReader["DrawingUploaded"]);
                    part.DrawingFileName = Convert.ToString(dataReader["DrawingFileName"]);
                    part.IsActive = Convert.ToBoolean(dataReader["IsActive"]);
                    part.IsSample = Convert.ToBoolean(dataReader["IsSample"]);
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);
                    part.DefaultWarehouse = Convert.ToBoolean(dataReader["DefaultWarehouse"]);
                    part.WarehouseId = Convert.ToInt32(dataReader["WarehouseId"]);

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
                        part.SupplierPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                        partSupplierAssignments.Add(partSupplierAssignment);
                    }
                    dataReader1.Close();
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
                        part.CustomerPrice = Convert.ToDecimal(dataReader1["Rate"]);
                        partCustomerAssignments.Add(partCustomerAssignment);
                    }
                    dataReader1.Close();
                }
                part.partCustomerAssignments = partCustomerAssignments;

                conn.Close();
            }


            foreach (Part part in parts)
            {
                commandText = string.Format("SELECT sum(Qty - ShippedQty) as openqty from OrderDetail where partid = '{0}' and (IsClosed =0 OR IsClosed IS NULL) ", part.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        try
                        {
                            part.OpenOrderQty = Convert.ToInt32(dataReader1["openqty"]);
                        }
                        catch
                        {
                            part.OpenOrderQty = 0;
                        }

                    }
                    dataReader1.Close();
                }
                conn.Close();


                commandText = string.Format("SELECT sum(Qty - (InTransitQty + ReceivedQty )) as openqty from PoDetails where partid = '{0}' and (IsClosed =0 OR IsClosed IS NULL) ", part.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        try
                        {
                            part.SupplierOpenPoQty = Convert.ToInt32(dataReader1["openqty"]);
                        }
                        catch
                        {
                            part.SupplierOpenPoQty = 0;
                        }

                    }
                    dataReader1.Close();
                }

                commandText = string.Format("SELECT [id]  ,[PartId]  ,[SupplierPrice] ,[CustomerPrice] ,[Qty] FROM [StockPrice] where partid = '{0}' order by id", part.Id);
                part.stockPrices = new List<StockPrice>();
                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var stockPrice = new StockPrice();
                        stockPrice.Id = Convert.ToInt32(dataReader1["Id"]);
                        stockPrice.PartId = Convert.ToInt32(dataReader1["PartId"]);
                        stockPrice.SupplierPrice = Convert.ToDecimal(dataReader1["SupplierPrice"]);
                        stockPrice.CustomerPrice = Convert.ToDecimal(dataReader1["CustomerPrice"]);
                        stockPrice.Qty = Convert.ToInt32(dataReader1["Qty"]);

                        part.stockPrices.Add(stockPrice);

                    }
                    dataReader1.Close();
                }

                conn.Close();
            }

            return parts.OrderBy(x => x.Code);
        }

        //
        public async Task<IEnumerable<Part>> GetAllPartsCompactAsync(int companyId, int userId)
        {
            List<Part> parts = new List<Part>();

            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] where CompanyId = '{0}' ", companyId);
            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partcustomerassignment PCA ON PCA.PartId = PM.id where CompanyId = '{companyId}' AND PCA.CustomerId IN ({companylist}) ");
            }
            if (userInfo.UserTypeId == 3)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],PM.[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partsupplierassignment PCA ON PCA.PartId = PM.id where CompanyId ='{companyId}' AND PCA.SupplierID IN ({companylist}) ");
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);
                    part.DefaultWarehouse = Convert.ToBoolean(dataReader["DefaultWarehouse"]);
                    part.WarehouseId = Convert.ToInt32(dataReader["WarehouseId"]);

                    parts.Add(part);
                }
                conn.Close();
            }

            

            return parts.OrderBy(x => x.Code);
        }

        public async Task<IEnumerable<PartCompact>> GetAllPartsCompactAsync1(int companyId, int userId)
        {
            List<PartCompact> parts = new List<PartCompact>();

            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] where CompanyId = '{0}' ", companyId);
            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partcustomerassignment PCA ON PCA.PartId = PM.id where CompanyId = '{companyId}' AND PCA.CustomerId IN ({companylist}) ");
            }
            if (userInfo.UserTypeId == 3)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],PM.[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] PM INNER JOIN partsupplierassignment PCA ON PCA.PartId = PM.id where CompanyId ='{companyId}' AND PCA.SupplierID IN ({companylist}) ");
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new PartCompact();
                    part.Id = Convert.ToInt64(dataReader["Id"]);
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["OpeningQty"]) + Convert.ToInt32(dataReader["QtyInHand"]);
                    

                    parts.Add(part);
                }
                conn.Close();
            }



            return parts.OrderBy(x => x.Code);
        }


        //

        public async Task<IEnumerable<Part>> GetAllPartsByDateAsync(int companyId, int userId,DateTime dateTime)
        {
            List<Part> parts = new List<Part>();

            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],0,0,[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] FROM [part] where CompanyId = '{0}' ", companyId);
            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],0,0,[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] FROM [part] PM INNER JOIN partcustomerassignment PCA ON PCA.PartId = PM.id where CompanyId = '{companyId}' AND PCA.CustomerId IN ({companylist}) ");
            }
            if (userInfo.UserTypeId == 3)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT PM.[id],[Code],PM.[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty], [DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],0,0,[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] FROM [part] PM INNER JOIN partsupplierassignment PCA ON PCA.PartId = PM.id where CompanyId ='{companyId}' AND PCA.SupplierID IN ({companylist}) ");
            }

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = 0;
                    part.QtyInHand = 0;
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);

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
                    dataReader1.Close();
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
                    dataReader1.Close();
                }
                part.partCustomerAssignments = partCustomerAssignments;

                conn.Close();
            }


            foreach (Part part in parts)
            {
                commandText = string.Format($"SELECT SUM(Qty) Qty FROM [dbo].[SupplierInvoiceMaster] SIM  INNER JOIN [dbo].[SupplierInvoiceDetails] SID ON SIM.Id = SID.InvoiceId  where  ReceivedDate  < '{dateTime}' and IsInvoiceReceived = 1 AND SID.PartId = '{part.Id}' Group by Qty");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        try
                        {
                            part.QtyInHand = Convert.ToInt32(dataReader1["Qty"]);
                        }
                        catch
                        {
                            part.QtyInHand = 0;
                        }

                    }
                    dataReader1.Close();
                }
                conn.Close();

                commandText = string.Format($"SELECT SUM(QTY) QTY  FROM [dbo].[PackingSlipMaster] PSM  INNER JOIN [dbo].[PackingSlipDetails] PSD ON PSM.Id = PSD.PackingSlipId  where  ShippingDate  < '{dateTime}' and PSD.PartId =  '{part.Id}' Group by Qty,PSD.PartId");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        try
                        {
                            part.QtyInHand = part.QtyInHand - Convert.ToInt32(dataReader1["Qty"]);
                        }
                        catch
                        {
                            
                        }

                    }
                    dataReader1.Close();
                }
                conn.Close();


                commandText = string.Format("SELECT [id]  ,[PartId]  ,[SupplierPrice] ,[CustomerPrice] ,[Qty] FROM [StockPrice] where partid = '{0}' order by id", part.Id);
                part.stockPrices = new List<StockPrice>();
                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var stockPrice = new StockPrice();
                        stockPrice.Id = Convert.ToInt32(dataReader1["Id"]);
                        stockPrice.PartId = Convert.ToInt32(dataReader1["PartId"]);
                        stockPrice.SupplierPrice = Convert.ToDecimal(dataReader1["SupplierPrice"]);
                        stockPrice.CustomerPrice = Convert.ToDecimal(dataReader1["CustomerPrice"]);
                        stockPrice.Qty = Convert.ToInt32(dataReader1["Qty"]);

                        part.stockPrices.Add(stockPrice);

                    }
                    dataReader1.Close();
                }

                conn.Close();
            }

            return parts.OrderBy(x => x.Code);
        }

        public Part GetPart(long partId)
        {
            var part = new Part();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty] ,[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] FROM [part] where id = '{0}' ", partId);

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]) + Convert.ToInt32(dataReader["OpeningQty"]);
                    part.OpeningQty = Convert.ToInt32(dataReader["OpeningQty"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);
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

        public async Task<Part> GetPartAsync(long partId, SqlConnection conn, SqlTransaction transaction)
        {
            var part = new Part();

            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty] ,[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] where id = '{0}' ", partId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.OpeningQty = Convert.ToInt32(dataReader["OpeningQty"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);
                    part.DefaultWarehouse = Convert.ToBoolean(dataReader["DefaultWarehouse"]);
                    part.WarehouseId = Convert.ToInt32(dataReader["WarehouseId"]);
                }
                dataReader.Close();
            }


            List<PartSupplierAssignment> partSupplierAssignments = new List<PartSupplierAssignment>();
            commandText = string.Format("SELECT [id],[PartID],[SupplierID],[MapCode],[Description],[QtyInHand],[QtyInTransit],[TotalQty],[UnitPrice] FROM [partsupplierassignment]  where partid = '{0}'", part.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn, transaction))
            {
                cmd1.CommandType = CommandType.Text;

                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);

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
                dataReader1.Close();
            }


            List<PartCustomerAssignment> partCustomerAssignments = new List<PartCustomerAssignment>();
            commandText = string.Format("SELECT [id],[PartId] ,[CustomerId] ,[MapCode] ,[Description] ,[Weight] ,[Rate] ,[SurchargeExist] ,[SurchargePerPound]  FROM [partcustomerassignment]  where partid = '{0}'", part.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn, transaction))
            {
                cmd1.CommandType = CommandType.Text;

                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);

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
                dataReader1.Close();
            }


            commandText = string.Format("SELECT [id]  ,[PartId]  ,[SupplierPrice] ,[CustomerPrice] ,[Qty] FROM [StockPrice] where partid = '{0}' order by id", part.Id);
            part.stockPrices = new List<StockPrice>();
            using (SqlCommand cmd1 = new SqlCommand(commandText, conn, transaction))
            {
                cmd1.CommandType = CommandType.Text;

                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);

                while (dataReader1.Read())
                {
                    var stockPrice = new StockPrice();
                    stockPrice.Id = Convert.ToInt32(dataReader1["Id"]);
                    stockPrice.PartId = Convert.ToInt32(dataReader1["PartId"]);
                    stockPrice.SupplierPrice = Convert.ToDecimal(dataReader1["SupplierPrice"]);
                    stockPrice.CustomerPrice = Convert.ToDecimal(dataReader1["CustomerPrice"]);
                    stockPrice.Qty = Convert.ToInt32(dataReader1["Qty"]);

                    part.stockPrices.Add(stockPrice);

                }
                dataReader1.Close();
            }            

            return part;
        }

        public async Task<Part> GetPartAsync(long partId)
        {
            var part = new Part();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[IntransitQty],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty] ,[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed],[DefaultWarehouse],[WarehouseId] FROM [part] where id = '{0}' ", partId);

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.OpeningQty = Convert.ToInt32(dataReader["OpeningQty"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);
                    part.DefaultWarehouse = Convert.ToBoolean(dataReader["DefaultWarehouse"]);
                    part.WarehouseId = Convert.ToInt32(dataReader["WarehouseId"]);
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

            commandText = string.Format("SELECT [id]  ,[PartId]  ,[SupplierPrice] ,[CustomerPrice] ,[Qty] FROM [StockPrice] where partid = '{0}' order by id", part.Id);
            part.stockPrices = new List<StockPrice>();
            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var stockPrice = new StockPrice();
                    stockPrice.Id = Convert.ToInt32(dataReader1["Id"]);
                    stockPrice.PartId = Convert.ToInt32(dataReader1["PartId"]);
                    stockPrice.SupplierPrice = Convert.ToDecimal(dataReader1["SupplierPrice"]);
                    stockPrice.CustomerPrice = Convert.ToDecimal(dataReader1["CustomerPrice"]);
                    stockPrice.Qty = Convert.ToInt32(dataReader1["Qty"]);

                    part.stockPrices.Add(stockPrice);

                }
                dataReader1.Close();
            }

            conn.Close();

            return part;
        }

        public async Task<IEnumerable<PartInTransit>> GetPartInTransitDetailAsync(long partId, int companyId)
        {
            var parts = new List<PartInTransit>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT 	[Code] ,[Description] ,[InvoiceNo] ,[InvoiceDate] ,[ETA] ,SID.[PartId], sum(SID.[Qty]) [Qty] ,[Name] as SupplierName,  SID.InvoiceID as [InvoiceId] FROM [SupplierInvoiceDetails] SID  INNER JOIN [SupplierInvoiceMaster] SIM ON SID.InvoiceId = SIM.Id  INNER JOIN [part] P ON P.id = SID.PartId  INNER JOIN [supplier] S ON S.id = SIM.SupplierId  where (IsBoxReceived = 0 OR IsBoxReceived IS NULL) " +
                " AND SIM.CompanyId = '{0}' AND SID.PartId = '{1}' Group by SID.[PartId],[Code],[Description],[InvoiceNo],[InvoiceDate],[ETA],[Name], SID.InvoiceID ", companyId, partId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new PartInTransit();
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.InvoiceNo = Convert.ToString(dataReader["InvoiceNo"]);
                    part.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    part.ETA = Convert.ToDateTime(dataReader["ETA"]);
                    part.PartId = Convert.ToInt32(dataReader["PartId"]);
                    part.Qty = Convert.ToInt32(dataReader["Qty"]);
                    part.SupplierName = Convert.ToString(dataReader["SupplierName"]);
                    part.InvoiceId = Convert.ToInt32(dataReader["InvoiceId"]);

                    parts.Add(part);
                }
                dataReader.Close();
                conn.Close();
            }


            foreach (PartInTransit part in parts)
            {
                commandText = string.Format("SELECT Distinct PONo FROM [dbo].[SupplierInvoicePoDetails]  WHERE InvoiceId = '{0}' and PartId = '{1}' ", part.InvoiceId, part.PartId);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        part.PoNo = part.PoNo + "," + Convert.ToString(dataReader1["PONo"]);

                    }
                    dataReader1.Close();
                }
                conn.Close();
            }

            return parts;
        }

        public async Task<IEnumerable<PartInTransit>> GetPartLatestReceivedAsync(long partId, int companyId)
        {
            var parts = new List<PartInTransit>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT top 5 [Code] ,[Description] ,[InvoiceNo] ,[InvoiceDate],[ReceivedDate] ,[ETA] ,[PoNo] ,[PartId],Sum([Qty]) as QTY ,[Name] as SupplierName  " +
                 "FROM [SupplierInvoiceDetails] SID   INNER JOIN [SupplierInvoiceMaster] SIM ON SID.InvoiceId = SIM.Id  INNER JOIN [part] P ON P.id = SID.PartId INNER JOIN [supplier] S ON S.id = SIM.SupplierId   " +
                 "where  IsBoxReceived = 1 and SIM.CompanyId = '{0}' and Partid =  '{1}' group by [Code] ,[Description] ,[InvoiceNo] ,[InvoiceDate],[ReceivedDate] ,[ETA] ,[PoNo],[PartId],[Name] order by ReceivedDate desc", companyId, partId);


            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new PartInTransit();
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.InvoiceNo = Convert.ToString(dataReader["InvoiceNo"]);
                    part.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    part.ReceivedDate = Convert.ToDateTime(dataReader["ReceivedDate"]);
                    part.ETA = Convert.ToDateTime(dataReader["ETA"]);
                    part.PoNo = Convert.ToString(dataReader["PoNo"]);
                    part.PartId = Convert.ToInt32(dataReader["PartId"]);
                    part.Qty = Convert.ToInt32(dataReader["Qty"]);
                    part.SupplierName = Convert.ToString(dataReader["SupplierName"]);

                    parts.Add(part);
                }
                dataReader.Close();
                conn.Close();
            }

            return parts;
        }

        public async Task<IEnumerable<WarehouseInventory>> GetPartWarehouseInventoryAsync(long partId, int companyId)
        {
            var parts = new List<WarehouseInventory>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT PM.Code,PM.Description, PWI.[PartId] ,PWI.[WarehouseId],PWI.[QtyInHand],wm.Name as Warehousename FROM [PartWarehouseInventory] PWI INNER JOIN part PM ON PM.ID = PWI.PartId INNER JOIN Warehouse WM ON WM.id = PWI.WarehouseId WHERE PWI.Partid =  '{partId}' AND PWI.COMPANYID = '{companyId}'  ");


            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new WarehouseInventory();
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.PartId = Convert.ToInt32(dataReader["PartId"]);
                    part.WarehouseId = Convert.ToInt32(dataReader["WarehouseId"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.Warehousename = Convert.ToString(dataReader["Warehousename"]);                  

                    parts.Add(part);
                }
                dataReader.Close();
                conn.Close();
            }

            return parts;
        }

        public async Task<IEnumerable<PartOpenOrder>> GetPartOpenOrderDetailAsync(long partId, int companyId)
        {
            var parts = new List<PartOpenOrder>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT  [Name] as CustomerName,[Code],[Description],[PONo] ,[PODate] ,OD.[DueDate], Qty - ShippedQty as openqty  from OrderDetail OD INNER JOIN [OrderMaster] OM ON OD.OrderId = OM.Id INNER JOIN [part] P ON P.id = OD.PartId INNER JOIN [customer] c ON c.id = OM.CustomerId where partid = '{0}' and (OD.IsClosed =0 OR OD.IsClosed IS NULL)  AND OM.CompanyId = '{1}'", partId, companyId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new PartOpenOrder();

                    part.CustomerName = Convert.ToString(dataReader["CustomerName"]);
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.PONo = Convert.ToString(dataReader["PONo"]);
                    part.PODate = Convert.ToDateTime(dataReader["PODate"]);
                    part.DueDate = Convert.ToDateTime(dataReader["DueDate"]);
                    part.OpenQty = Convert.ToInt32(dataReader["openqty"]);

                    parts.Add(part);

                }
                dataReader.Close();
                conn.Close();
            }


            return parts;
        }

        public async Task<IEnumerable<SupplierOpenPO>> GetPartOpenPODetailAsync(long partId, int companyId)
        {
            var parts = new List<SupplierOpenPO>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT S.Name as SupplierName ,[PoId] ,[PartId] ,p.code , pm.PoNo, pm.PoDate, p.Description   ,[ReferenceNo]  ,[UnitPrice],PD.[DueDate],[Note],[Qty] - (pd.[InTransitQty] +  pd.[ReceivedQty]) as OpenQty ,[SrNo],pd.PartAcknowledgementDate,pd.[Qty]   FROM [dbo].[PoDetails] PD  INNER JOIN PoMaster PM ON PM.ID = PD.PoId  INNER JOIN part P ON P.id = PD.PartId INNER JOIN supplier S ON S.ID = PM.SupplierId   WHERE PartId = '{0}' AND (PD.IsClosed = 0 OR PD.IsClosed IS NULL) AND PM.CompanyId = '{1}'  order by pd.DueDate asc ", partId, companyId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new SupplierOpenPO();

                    part.SupplierName = Convert.ToString(dataReader["SupplierName"]);
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.ReferenceNo = Convert.ToString(dataReader["ReferenceNo"]);
                    part.DueDate = Convert.ToDateTime(dataReader["DueDate"]);                    
                    part.OrderedQty = Convert.ToInt32(dataReader["Qty"]);
                    part.OpenQty = Convert.ToInt32(dataReader["openqty"]);
                    part.SrNo = Convert.ToInt32(dataReader["SrNo"]);
                    part.PoNo = Convert.ToString(dataReader["PoNo"]);                   

                    if (dataReader["PartAcknowledgementDate"] != DBNull.Value)
                        part.AcknowledgeDate = Convert.ToDateTime(dataReader["PartAcknowledgementDate"]);
                    else
                        part.AcknowledgeDate = null;

                    parts.Add(part);

                }
                dataReader.Close();
                conn.Close();
            }


            return parts;
        }

        public async Task<IEnumerable<PartLatestShipment>> GetPartLatestShipmentAsync(long partId, int companyId)
        {
            var parts = new List<PartLatestShipment>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT TOP 5 NAME as CustomerName ,[code],[Description],[PackingSlipNo],[ShippingDate],[QTY]" +
                " FROM [PackingSlipMaster] PSM INNER JOIN [PackingSlipDetails] PSD ON PSM.ID = PSD.PackingSlipId INNER JOIN [part] P ON P.id = PSD.PartId INNER JOIN [customer] c ON c.id = PSM.CustomerId WHERE PARTID ='{0}' AND PSM.COMPANYID = '{1}' ORDER BY PSM.ID DESC", partId, companyId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new PartLatestShipment();

                    part.CustomerName = Convert.ToString(dataReader["CustomerName"]);
                    part.Code = Convert.ToString(dataReader["Code"]);
                    part.Description = Convert.ToString(dataReader["Description"]);
                    part.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);
                    part.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    part.Qty = Convert.ToInt32(dataReader["QTY"]);

                    parts.Add(part);
                }
                dataReader.Close();
                conn.Close();
            }


            return parts;
        }

        public async Task<IEnumerable<PartTotalShipment>> GetPartTotalShipmentAsync(long partId)
        {
            var parts = new List<PartTotalShipment>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT  ISNULL(SUM([Qty]),0) ShippedQty,ISNULL(SUM(ExcessQty),0) MonthlyExcessQty FROM  [dbo].[PackingSlipDetails] WHERE PARTID = {0};", partId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new PartTotalShipment();

                    part.ShippedQty = Convert.ToInt32(dataReader["ShippedQty"]);
                    part.MonthlyExcessQty = Convert.ToInt32(dataReader["MonthlyExcessQty"]);

                    parts.Add(part);
                }
                dataReader.Close();
                conn.Close();
            }


            return parts;
        }

        public async Task<IEnumerable<PartTotalInvoiceQty>> GetPartTotalInvoiceQtyAsync(long partId)
        {
            var parts = new List<PartTotalInvoiceQty>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT  ISNULL(SUM([Qty]),0) InvoiceQty FROM  [dbo].[InvoiceDetails] WHERE PARTID =  {0};", partId);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new PartTotalInvoiceQty();

                    part.InvoiceQty = Convert.ToInt32(dataReader["InvoiceQty"]);

                    parts.Add(part);
                }
                dataReader.Close();
                conn.Close();
            }


            return parts;
        }

        public async Task<IEnumerable<StockPrice>> GetStock(long partId, int companyId)
        {
            var stockPrices = new List<StockPrice>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT id,PartId,Qty,CustomerPrice,SupplierPrice FROM StockPrice WHERE PartId = '{partId}' and Qty > 0 order by id ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

                while (dataReader.Read())
                {
                    var stockPrice = new StockPrice();
                    stockPrice.Id = Convert.ToInt32(dataReader["Id"]);
                    stockPrice.PartId = Convert.ToInt32(dataReader["PartId"]);
                    stockPrice.Qty = Convert.ToInt32(dataReader["Qty"]);
                    stockPrice.CustomerPrice = Convert.ToDecimal(dataReader["CustomerPrice"]);
                    stockPrice.SupplierPrice = Convert.ToDecimal(dataReader["SupplierPrice"]);
                    stockPrices.Add(stockPrice);
                }
                dataReader.Close();
            }

            return stockPrices.OrderBy(x => x.Id);
        }

        public async Task<Part> GetPartByNameAsync(int companyId, string name)
        {
            var part = new Part();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] FROM [part] where companyId = '{0}'  and Code = '{1}'", companyId, name);

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.OpeningQty = Convert.ToInt32(dataReader["OpeningQty"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);

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
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[QtyInHand],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty],[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] FROM [part]   where id in (SELECT[PartID] FROM[partsupplierassignment]  where[SupplierID] = '{0}' and REPLACE(Mapcode,' ','') = '{1}')", supplierId, mapCode.Replace(" ", ""));

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.OpeningQty = Convert.ToInt32(dataReader["OpeningQty"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);
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

        public async Task<IEnumerable<Part>> GetPartBySupplierIdAsync(int supplierId)
        {
            List<Part> parts = new List<Part>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[QtyInHand],[IntransitQty],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty] ,[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] FROM [part]   where id in (SELECT[PartID] FROM[partsupplierassignment]  where[SupplierID] = '{0}')", supplierId);

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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]);
                    part.OpeningQty = Convert.ToInt32(dataReader["OpeningQty"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);

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

            return parts.OrderBy(x => x.Code);
        }

        public async Task<IEnumerable<Part>> GetPartByCustomerIdAsync(int customerId)
        {
            List<Part> parts = new List<Part>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format("SELECT [id],[Code],[Description],[CompanyId],[WeightInKg],[WeightInLb],[IsSample],[MinQty],[MaxQty],[OpeningQty],[SafeQty]," +
                 "[DrawingNo],[DrawingUploaded],[DrawingFileName],[IsActive],[Location],[QtyInHand],[IntransitQty],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty] ,[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] FROM [part]   where id in (SELECT[PartID] FROM[partcustomerassignment]  where[CustomerID] = '{0}')", customerId);
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
                    part.IsRepackage = Convert.ToBoolean(dataReader["IsRepackage"]);
                    part.Location = Convert.ToString(dataReader["Location"]);
                    part.IntransitQty = Convert.ToInt32(dataReader["IntransitQty"]);
                    part.QtyInHand = Convert.ToInt32(dataReader["QtyInHand"]) + Convert.ToInt32(dataReader["OpeningQty"]);
                    part.OpeningQty = Convert.ToInt32(dataReader["OpeningQty"]);
                    part.MonthlyForecastQty = Convert.ToInt32(dataReader["MonthlyForecastQty"]);
                    part.SupplierCode = Convert.ToString(dataReader["SupplierCode"]);
                    part.FuturePrice = Convert.ToDecimal(dataReader["FuturePrice"]);
                    part.CurrentPricingInEffectQty = Convert.ToInt32(dataReader["CurrentPricingInEffectQty"]);
                    part.MonthlyOpeningQty = Convert.ToInt32(dataReader["MonthlyOpeningQty"]);
                    part.MonthlyReturnQty = Convert.ToInt32(dataReader["MonthlyReturnQty"]);
                    part.MonthlyRejectQty = Convert.ToInt32(dataReader["MonthlyRejectQty"]);
                    part.IsDoublePricingAllowed = Convert.ToBoolean(dataReader["IsDoublePricingAllowed"]);

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

            return parts.OrderBy(x => x.Code);
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
                    if (part.DrawingNo == null)
                        part.DrawingNo = string.Empty;

                    if (part.Location == null)
                        part.Location = string.Empty;

                    if (part.DrawingFileName == null)
                        part.DrawingFileName = string.Empty;

                    string sql = string.Format($"INSERT INTO [dbo].[part]   ([Code]   ,[Description]   ,[CompanyId]   ,[WeightInKg]   ,[WeightInLb]   ,[IsSample]   ,[MinQty]   ,[MaxQty]   ,[OpeningQty],[SafeQty],[DrawingNo]   ,[DrawingUploaded]   ,[DrawingFileName]   ,[IsActive]   ,[Location],[MonthlyForecastQty],[SupplierCode],[IsRepackage],[FuturePrice],[CurrentPricingInEffectQty] ,[MonthlyOpeningQty] ,[MonthlyReturnQty] ,[MonthlyRejectQty],[IsDoublePricingAllowed] ,[DefaultWarehouse],[WarehouseId])     VALUES   ('{part.Code.Replace("'", "''")}'   ,'{part.Description.Replace("'", "''")}'   ,'{part.CompanyId}'   ,'{part.WeightInKg}'   ,'{part.WeightInLb}'   ,'{part.IsSample}'   ,'{part.MinQty}'   ,'{part.MaxQty}'   ,'{part.OpeningQty}'   ,'{part.SafeQty}' ,'{part.DrawingNo}'   ,'{part.DrawingUploaded}'   ,'{part.DrawingFileName.Replace("'", "''")}'   ,'{part.IsActive}'   ,'{part.Location.Replace("'", "''")}','{part.MonthlyForecastQty}','{part.SupplierCode}','{part.IsRepackage}','{part.FuturePrice}','{part.CurrentPricingInEffectQty}','{part.MonthlyOpeningQty}','{part.MonthlyReturnQty}','{part.MonthlyRejectQty}','{part.IsDoublePricingAllowed}','{part.DefaultWarehouse}','{part.WarehouseId}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;
                    var partId = command.ExecuteScalar();

                    foreach (PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
                    {
                        if (partSupplierAssignment.MapCode == null)
                            partSupplierAssignment.MapCode = string.Empty;

                        if (partSupplierAssignment.Description == null)
                            partSupplierAssignment.Description = string.Empty;

                        sql = string.Format($"INSERT INTO [dbo].[partsupplierassignment]   ([PartID]   ,[SupplierID]   ,[MapCode]   ,[Description]   ,[QtyInHand]   ,[QtyInTransit]   ,[TotalQty]   ,[UnitPrice])     VALUES   ('{partId}'   ,'{partSupplierAssignment.SupplierID}'   ,'{partSupplierAssignment.MapCode.Replace("'", "''")}'   ,'{partSupplierAssignment.Description.Replace("'", "''")}'   ,'{partSupplierAssignment.QtyInHand}'   ,'{partSupplierAssignment.QtyInTransit}'   ,'{partSupplierAssignment.TotalQty}'   ,'{partSupplierAssignment.UnitPrice}')");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    foreach (PartCustomerAssignment partCustomerAssignment in part.partCustomerAssignments)
                    {
                        if (partCustomerAssignment.MapCode == null)
                            partCustomerAssignment.MapCode = string.Empty;

                        if (partCustomerAssignment.Description == null)
                            partCustomerAssignment.Description = string.Empty;
                        sql = string.Format($"INSERT INTO [dbo].[partcustomerassignment]   ([PartId]   ,[CustomerId]   ,[MapCode]   ,[Description]   ,[Weight]   ,[Rate]   ,[SurchargeExist]   ,[SurchargePerPound] )     VALUES   ('{partId}'   ,'{partCustomerAssignment.CustomerId}'   ,'{partCustomerAssignment.MapCode.Replace("'", "''")}'   ,'{partCustomerAssignment.Description.Replace("'", "''")}'   ,'{partCustomerAssignment.Weight}'   ,'{partCustomerAssignment.Rate}'   ,'{partCustomerAssignment.SurchargeExist}'   ,'{partCustomerAssignment.SurchargePerPound}'   )");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    if (part.WarehouseId > 0)
                    {

                        sql = string.Format($"INSERT INTO [dbo].[PartWarehouseInventory]  ([PartId] ,[CompanyId] ,[WarehouseId],[Location] ,[OpeningQty] ,[QtyInHand])  VALUES    " +
                            $"('{partId}'   ,'{part.CompanyId}'   ,'{part.WarehouseId}'   ,'{part.Location.Replace("'", "''")}''   ,'{0}'   ,'{0}'   )");

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

        public async Task SetStockPriceAsync(int partId, List<StockPrice> stockPrices)
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
                    var sql = string.Empty;
                    
                    sql = string.Format($"delete from [StockPrice]  where partid = '{partId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (StockPrice stockPrice in stockPrices)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[StockPrice]  ([PartId] ,[SupplierPrice] ,[CustomerPrice] ,[Qty])  VALUES  ('{stockPrice.PartId}','{stockPrice.SupplierPrice}','{stockPrice.CustomerPrice}','{stockPrice.Qty}')");
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

        public async Task SetStockPriceAsync(List<StockPrice> stockPrices,int companyId)
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
                    var sql = string.Empty;
                    foreach (StockPrice stockPrice in stockPrices)
                    {
                        var part = await this.GetPartByNameAsync(companyId, stockPrice.PartCode);
                        stockPrice.PartId = Convert.ToInt32(part.Id);
                        sql = string.Format($"INSERT INTO [dbo].[StockPrice]  ([PartId] ,[SupplierPrice] ,[CustomerPrice] ,[Qty])  VALUES  ('{stockPrice.PartId}','{stockPrice.SupplierPrice}','{stockPrice.CustomerPrice}','{stockPrice.Qty}')");
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
            bool IsPartExistInWarehouse = false;

            if (part.WarehouseId > 0)
            {

                var commandText = string.Format($"SELECT [id]   FROM  [PartWarehouseInventory] where  CompanyId = '{part.CompanyId}' AND PartId = '{part.Id}' AND WarehouseId = '{part.WarehouseId}' ");

                SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        IsPartExistInWarehouse = true;
                    }
                    conn.Close();
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
                    if (part.DrawingNo == null)
                        part.DrawingNo = string.Empty;

                    if (part.Location == null)
                        part.Location = string.Empty;

                    if (part.DrawingFileName == null)
                        part.DrawingFileName = string.Empty;

                    var sql = string.Format("DELETE FROM [dbo].[partsupplierassignment]  WHERE partid = '{0}'", part.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format("DELETE FROM [dbo].[partcustomerassignment]  WHERE partid = '{0}'", part.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"UPDATE [dbo].[part]   SET [Code] = '{part.Code.Replace("'", "''")}' ,[Description] = '{part.Description.Replace("'", "''")}' ,[CompanyId] = '{part.CompanyId}' ,[WeightInKg] = '{part.WeightInKg}' ,[WeightInLb] = '{part.WeightInLb}' ,[IsSample] = '{part.IsSample}' ,[MinQty] = '{part.MinQty}' ,[MaxQty] = '{part.MaxQty}',[OpeningQty] = '{part.OpeningQty}' ,[SafeQty] = '{part.SafeQty}' ,[DrawingNo] = '{part.DrawingNo.Replace("'", "''")}' ,[DrawingUploaded] = '{part.DrawingUploaded}' ,[DrawingFileName] = '{part.DrawingFileName.Replace("'", "''")}' ,[IsActive] = '{part.IsActive}' ,[Location] = '{part.Location.Replace("'", "''")}',[MonthlyForecastQty] = '{part.MonthlyForecastQty}',[SupplierCode] = '{part.SupplierCode.Replace("'", "''")}',[IsRepackage] = '{part.IsRepackage}',[FuturePrice] = '{part.FuturePrice}' ,[CurrentPricingInEffectQty] = '{part.CurrentPricingInEffectQty}' ,[IsDoublePricingAllowed] = '{part.IsDoublePricingAllowed}',[DefaultWarehouse] = '{part.DefaultWarehouse}' ,[WarehouseId] = '{part.WarehouseId}'   WHERE id = '{part.Id}' ");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (PartSupplierAssignment partSupplierAssignment in part.partSupplierAssignments)
                    {
                        if (partSupplierAssignment.MapCode == null)
                            partSupplierAssignment.MapCode = string.Empty;

                        if (partSupplierAssignment.Description == null)
                            partSupplierAssignment.Description = string.Empty;
                        sql = string.Format($"INSERT INTO [dbo].[partsupplierassignment]   ([PartID]   ,[SupplierID]   ,[MapCode]   ,[Description]   ,[QtyInHand]   ,[QtyInTransit]   ,[TotalQty]   ,[UnitPrice])     VALUES   ('{part.Id}'   ,'{partSupplierAssignment.SupplierID}'   ,'{partSupplierAssignment.MapCode.Replace("'", "''")}'   ,'{partSupplierAssignment.Description.Replace("'", "''")}'   ,'{partSupplierAssignment.QtyInHand}'   ,'{partSupplierAssignment.QtyInTransit}'   ,'{partSupplierAssignment.TotalQty}'   ,'{partSupplierAssignment.UnitPrice}')");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    foreach (PartCustomerAssignment partCustomerAssignment in part.partCustomerAssignments)
                    {
                        if (partCustomerAssignment.MapCode == null)
                            partCustomerAssignment.MapCode = string.Empty;

                        if (partCustomerAssignment.Description == null)
                            partCustomerAssignment.Description = string.Empty;
                        sql = string.Format($"INSERT INTO [dbo].[partcustomerassignment]   ([PartId]   ,[CustomerId]   ,[MapCode]   ,[Description]   ,[Weight]   ,[Rate]   ,[SurchargeExist]   ,[SurchargePerPound]  )     VALUES   ('{part.Id}'   ,'{partCustomerAssignment.CustomerId}'   ,'{partCustomerAssignment.MapCode.Replace("'", "''")}'   ,'{partCustomerAssignment.Description.Replace("'", "''")}'   ,'{partCustomerAssignment.Weight}'   ,'{partCustomerAssignment.Rate}'   ,'{partCustomerAssignment.SurchargeExist}'   ,'{partCustomerAssignment.SurchargePerPound}' )");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }


                    if (part.WarehouseId > 0 && IsPartExistInWarehouse == false)
                    {
                        sql = string.Format($"DELETE FROM[PartWarehouseInventory] WHERE PARTID ='{part.Id}' AND[WarehouseId] = '{part.WarehouseId}' ");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        sql = string.Format($"INSERT INTO [dbo].[PartWarehouseInventory]  ([PartId] ,[CompanyId] ,[WarehouseId],[Location] ,[OpeningQty] ,[QtyInHand])  VALUES    " +
                            $"('{part.Id}'   ,'{part.CompanyId}'   ,'{part.WarehouseId}'   ,'{part.Location.Replace("'", "''")}'   ,'{0}'   ,'{0}'   )");                      command.CommandText = sql;
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
               
        public async Task UpdatePartCustomerPriceAsync(int companyId, string customer, string partcode, decimal price)
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

                    var sql = string.Format($"UPDATE pca SET pca.Rate = '{price}' FROM dbo.[partcustomerassignment] AS pca INNER JOIN dbo.customer AS cust    ON pca.CustomerId = cust.id INNER JOIN dbo.part AS part    ON pca.PartId = part.id WHERE part.Code= '{partcode}' AND part.CompanyId = '{companyId}' AND cust.Name = '{customer}'");

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

        public async Task UpdateOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty)
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

                    var sql = string.Format($"UPDATE [part]   SET  [OpeningQty] = '{openingQty}'  WHERE  Code= '{partcode}' AND CompanyId = '{companyId}'");

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

        public async Task UpdateMonthlyOpeningQtyByPartCodeAsync(int companyId, string partcode, int openingQty)
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

                    var sql = string.Format($"UPDATE [part]   SET  [MonthlyOpeningQty] = '{openingQty}'  WHERE  Code= '{partcode}' AND CompanyId = '{companyId}'");

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

        public async Task UpdateOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty)
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

                    var sql = string.Format($"UPDATE [part]   SET  [OpeningQty] = '{openingQty}'  WHERE  id= '{partId}' AND CompanyId = '{companyId}'");

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

        public async Task UpdateMonthlyOpeningQtyByPartIdAsync(int companyId, int partId, int openingQty)
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

                    var sql = string.Format($"UPDATE [part]   SET  [MonthlyOpeningQty] = '{openingQty}'  WHERE  id= '{partId}' AND CompanyId = '{companyId}'");

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

        public async Task UpdateMonthlyQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note)
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
                    var sql = string.Empty;
                    if (direction.ToLower() == BusinessConstants.DIRECTION.IN.ToString().ToLower())
                    {
                        sql = string.Format($"UPDATE [part]   SET  [MonthlyOpeningQty] = [MonthlyOpeningQty] + '{QtyInHand}'  WHERE  id= '{partId}' AND CompanyId = '{companyId}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();                        
                    }
                    else
                    {
                        sql = string.Format($"UPDATE [part]   SET  [MonthlyOpeningQty] = [MonthlyOpeningQty] - '{QtyInHand}'  WHERE  id= '{partId}' AND CompanyId = '{companyId}'");
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

        public async Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand)
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

                    var sql = string.Format($"UPDATE [part]   SET  [QtyInHand] = '{QtyInHand}'  WHERE  id= '{partId}' AND CompanyId = '{companyId}'");

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

        public async Task UpdateQtyInHandByPartIdAsync(int companyId, int partId, int QtyInHand, string direction, string note)
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
                    var sql = string.Empty;
                    if (direction.ToLower() == BusinessConstants.DIRECTION.IN.ToString().ToLower())
                    {
                        sql = string.Format($"UPDATE [part]   SET  [QtyInHand] = QtyInHand + '{QtyInHand}'  WHERE  id= '{partId}' AND CompanyId = '{companyId}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{partId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.ADJUSTMENT_PLUS)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.IN)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND)}'   ,'{note}'   ,'{QtyInHand}')");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        sql = string.Format($"UPDATE [part]   SET  [QtyInHand] = QtyInHand - '{QtyInHand}'  WHERE  id= '{partId}' AND CompanyId = '{companyId}'");
                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();

                        sql = string.Format($"INSERT INTO [dbo].[TransactionDetails]   ([PartId]   ,[TransactionTypeId]   ,[TransactionDate]   ,[DirectionTypeId]   ,[InventoryTypeId]   ,[ReferenceNo]   ,[Qty]) VALUES   ('{partId}'   ,'{ Convert.ToInt32(BusinessConstants.TRANSACTION_TYPE.ADJUSTMENT_MINUS)}'   ,'{DateTime.Now}'   ,'{Convert.ToInt32(BusinessConstants.DIRECTION.OUT)}'   ,'{Convert.ToInt32(BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND)}'   ,'{note}'   ,'{QtyInHand}')");
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

        public async Task UpdatePartSupplierPriceAsync(int companyId, string supplier, string partcode, decimal price)
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

                    var sql = string.Format($"UPDATE pca SET pca.UnitPrice = '{price}' FROM dbo.[partsupplierassignment] AS pca INNER JOIN dbo.supplier AS supp  ON pca.SupplierID = supp.id INNER JOIN dbo.part AS part    ON pca.PartId = part.id WHERE part.Code= '{partcode}' AND  part.companyId= '{companyId}' AND supp.Name = '{supplier}'");

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

        public async Task<IEnumerable<StockPrice>> GetAllPartsStocksAsync(int companyId)
        {
            List<StockPrice> stockPrices = new List<StockPrice>();

            var commandText = string.Format("SELECT SP.[id] ,[PartId] ,[SupplierPrice] ,[CustomerPrice] ,[Qty],PM.Code,PM.Description  FROM [dbo].[StockPrice] SP INNER JOIN part PM ON PM.id = SP.PartId WHERE PM.CompanyId = '{0}' ", companyId);

           SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var stockPrice = new StockPrice();
                    stockPrice.Id = Convert.ToInt32(dataReader["Id"]);
                    stockPrice.PartCode = Convert.ToString(dataReader["Code"]);
                    stockPrice.Description = Convert.ToString(dataReader["Description"]);                    
                    stockPrice.SupplierPrice = Convert.ToDecimal(dataReader["SupplierPrice"]);
                    stockPrice.CustomerPrice = Convert.ToDecimal(dataReader["CustomerPrice"]);
                    stockPrice.Qty = Convert.ToInt32(dataReader["Qty"]);

                    stockPrices.Add(stockPrice);
                }
                conn.Close();
            }

           
            return stockPrices.OrderBy(x => x.PartCode);
        }

        public async Task TransferInventoryInternallyAsync(PartTransfer partTransfer)
        {
            bool IsPartExistInWarehouse = false;

            var commandText = string.Format($"SELECT [id]   FROM  [PartWarehouseInventory] where  CompanyId = '{partTransfer.CompanyId}' AND PartId = '{partTransfer.PartId}' AND WarehouseId = '{partTransfer.ToWarehouseId}' ");

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    IsPartExistInWarehouse = true;
                }
                conn.Close();
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
                    var sql = string.Empty;
                    sql = string.Format($"UPDATE PartWarehouseInventory SET QtyInHand =  QtyInHand - '{partTransfer.Qty}' where partid= '{partTransfer.PartId}' AND CompanyId= '{partTransfer.CompanyId}' AND WarehouseId= '{partTransfer.FromWarehouseId}' ");

                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    if (IsPartExistInWarehouse)
                    {
                        sql = string.Format($"UPDATE PartWarehouseInventory SET QtyInHand =  QtyInHand + '{partTransfer.Qty}' where partid= '{partTransfer.PartId}' AND CompanyId= '{partTransfer.CompanyId}' AND WarehouseId= '{partTransfer.ToWarehouseId}' ");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        sql = string.Format($"INSERT INTO [dbo].[PartWarehouseInventory]  ([PartId] ,[CompanyId] ,[WarehouseId] ,[QtyInHand],[OpeningQty])  VALUES    " +
                                                   $"('{partTransfer.PartId}'   ,'{partTransfer.CompanyId}'   ,'{partTransfer.ToWarehouseId}'     ,'{partTransfer.Qty}' ,'{0}'    )");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }

                    sql = string.Format($"INSERT INTO [dbo].[PartWarehouseInventoryTransfer]   " +
                        $"([PartId]   ,[CompanyId]   ,[Qty]   ,[FromWarehouseId]   ,[ToWarehouseId]   ,[ActionTime]  )     VALUES   " +
                        $"('{partTransfer.PartId}'   ,'{partTransfer.CompanyId}'   ,'{partTransfer.Qty}'   ,'{partTransfer.FromWarehouseId}'   ,'{partTransfer.ToWarehouseId}', '{DateTime.Now}'     )");

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
