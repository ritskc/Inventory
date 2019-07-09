using DAL.DBHelper;
using DAL.IRepository;
using DAL.Models;
using DAL.Settings;
using DAL.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class EntityTrackerRepository : IEntityTrackerRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public EntityTrackerRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public Task AddEntityTrackerAsyncAsync(EntityTracker entityTracker)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EntityTracker>> GetAllEntityTrackerAsync(int companyId)
        {
            List<EntityTracker> entityTrackers = new List<EntityTracker>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT  [CompanyId] ,[FinYear] ,[Entity] ,[AvailableNo] FROM [dbo].[EntityTracker] where CompanyId = '{companyId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var entityTracker = new EntityTracker();
                    entityTracker.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    entityTracker.FinYear = Convert.ToString(dataReader["FinYear"]);
                    //Enum.TryParse(Convert.ToString(dataReader["Entity"]), out EntityType entityType);
                    //entityTracker.Entity = entityType;
                    //entityTracker.AvailableNo = Convert.ToInt32(dataReader["AvailableNo"]);

                    entityTrackers.Add(entityTracker);
                }
                conn.Close();
            }
            return entityTrackers;
        }



        public async Task<EntityTracker> GetEntityTrackerAsync(int companyId, string FinYear)
        {
            var finyear = DateTimeUtil.GetIndianFinancialYear(DateTime.Now);
            var entityTracker = new EntityTracker();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT  [CompanyId] ,[FinYear] ,[Entity] ,[AvailableNo] FROM [dbo].[EntityTracker] where CompanyId = '{companyId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    entityTracker.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    entityTracker.FinYear = Convert.ToString(dataReader["FinYear"]);
                    //Enum.TryParse(Convert.ToString(dataReader["Entity"]), out EntityType entityType);
                    //entityTracker.Entity = entityType;
                    //entityTracker.AvailableNo = Convert.ToInt32(dataReader["AvailableNo"]);
                }
                conn.Close();
            }
            return entityTracker;
        }

        public async Task<EntityTracker> GetEntityAsync(int companyId, DateTime dateTime, string entity)
        {
            EntityTracker entityTracker = null;
            var finYear = string.Empty;
            if (entity.ToLower() == BusinessConstants.ENTITY_TRACKER_PO.ToLower())
            {
                finYear = DateTimeUtil.GetIndianFinancialYear(dateTime);
            }
            else if (entity.ToLower() == BusinessConstants.ENTITY_TRACKER_PACKING_SLIP.ToLower())
            {
                finYear = DateTimeUtil.GetUSAFinancialYear(dateTime);
            }
            else
                return null;

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT  [CompanyId] ,[FinYear] ,[Entity] ,[AvailableNo] FROM [dbo].[EntityTracker] where CompanyId = '{companyId}'  and FinYear = '{finYear}' and Entity = '{entity}'");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    entityTracker = new EntityTracker();
                    entityTracker.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    entityTracker.FinYear = Convert.ToString(dataReader["FinYear"]);
                    entityTracker.Entity = entity;
                    string entityNumber;
                    if(Convert.ToInt32(dataReader["AvailableNo"]) < 9)
                    {
                        entityNumber = "00" + Convert.ToInt32(dataReader["AvailableNo"]);
                    }
                    else if(Convert.ToInt32(dataReader["AvailableNo"]) > 9 && Convert.ToInt32(dataReader["AvailableNo"]) < 100)
                    {
                        entityNumber = "0" + Convert.ToInt32(dataReader["AvailableNo"]);
                    }
                    else
                    {
                        entityNumber = Convert.ToInt32(dataReader["AvailableNo"]).ToString();
                    }
                    entityTracker.EntityNo = entityTracker.FinYear + "-" + entityNumber;
                }
                conn.Close();
            }

            if (entityTracker == null)
            {
                entityTracker = new EntityTracker();
                entityTracker.CompanyId = companyId;
                entityTracker.FinYear = finYear;
                entityTracker.Entity = entity;
                entityTracker.EntityNo = finYear + "-" + "001";
            }
            return entityTracker;
        }

        private EntityTracker GetEntity(int companyId, DateTime dateTime, string entity)
        {
            EntityTracker entityTracker = null;
            var finYear = string.Empty;
            if (entity.ToLower() == BusinessConstants.ENTITY_TRACKER_PO.ToLower())
            {
                finYear = DateTimeUtil.GetIndianFinancialYear(dateTime);
            }
            else if (entity.ToLower() == BusinessConstants.ENTITY_TRACKER_PACKING_SLIP.ToLower())
            {
                finYear = DateTimeUtil.GetUSAFinancialYear(dateTime);
            }
            else
                return null;

            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT  [CompanyId] ,[FinYear] ,[Entity] ,[AvailableNo] FROM [dbo].[EntityTracker] where CompanyId = '{companyId}'  and FinYear = '{finYear}' and Entity = '{entity}'");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    entityTracker = new EntityTracker();
                    entityTracker.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    entityTracker.FinYear = Convert.ToString(dataReader["FinYear"]);
                    if (entity.ToLower() == BusinessConstants.ENTITY_TRACKER_PO.ToLower())
                    {
                        entity = BusinessConstants.ENTITY_TRACKER_PO;                        
                    }
                    else if (entity.ToLower() == BusinessConstants.ENTITY_TRACKER_PACKING_SLIP.ToLower())
                    {
                        entity = BusinessConstants.ENTITY_TRACKER_PACKING_SLIP;                        
                    }
                    entityTracker.Entity = entity;
                    entityTracker.EntityNo = entityTracker.FinYear + "-" + Convert.ToInt32(dataReader["AvailableNo"]);
                }
                conn.Close();
            }
            return entityTracker;
        }

        public async Task AddEntityAsync(int companyId, DateTime dateTime, string entity)
        {
            var entityTracker = GetEntity(companyId, dateTime, entity);

            var finYear = string.Empty;
            if (entity.ToLower() == BusinessConstants.ENTITY_TRACKER_PO.ToLower())
            {
                entity = BusinessConstants.ENTITY_TRACKER_PO;
                finYear = DateTimeUtil.GetIndianFinancialYear(dateTime);
            }
            else if (entity.ToLower() == BusinessConstants.ENTITY_TRACKER_PACKING_SLIP.ToLower())
            {
                entity = BusinessConstants.ENTITY_TRACKER_PACKING_SLIP;
                finYear = DateTimeUtil.GetUSAFinancialYear(dateTime);
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
                    if (entityTracker == null)
                    {
                        var sql = string.Format($"INSERT INTO [dbo].[EntityTracker]   ([CompanyId]   ,[FinYear]   ,[Entity]   ,[AvailableNo]) VALUES   ('{companyId}'   ,'{finYear}'   ,'{entity}'   ,'{2}')");

                        command.CommandText = sql;
                        await command.ExecuteNonQueryAsync();
                    }
                    else
                    {
                        var sql = string.Format($"UPDATE [dbo].[EntityTracker]   SET  [AvailableNo] = '{Convert.ToInt32(entityTracker.EntityNo.Replace(entityTracker.FinYear, "").Replace("-", "")) + 1}' where CompanyId = '{companyId}'  and FinYear = '{finYear}' and Entity = '{entity}'");
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
    }
}
