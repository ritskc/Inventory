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
                    Enum.TryParse(Convert.ToString(dataReader["Entity"]), out EntityType entityType);
                    entityTracker.Entity = entityType;
                    entityTracker.AvailableNo = Convert.ToInt32(dataReader["AvailableNo"]);

                    entityTrackers.Add(entityTracker);
                }
                conn.Close();
            }
            return entityTrackers;
        }

       

        public async Task<EntityTracker> GetEntityTrackerAsync(int companyId, string FinYear)
        {
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
                    Enum.TryParse(Convert.ToString(dataReader["Entity"]), out EntityType entityType);
                    entityTracker.Entity = entityType;
                    entityTracker.AvailableNo = Convert.ToInt32(dataReader["AvailableNo"]);
                }
                conn.Close();
            }
            return entityTracker;
        }       
    }
}
