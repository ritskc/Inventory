using DAL.DBHelper;
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
    public class UserActivityReportRepository : IUserActivityReportRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public UserActivityReportRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task AddActivityAsync(UserActivityReport userActivityReport)
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
                    var sql = string.Format($"INSERT INTO [dbo].[UserActivityReport]   ([UserId], [Module]  ,[Action] ,[Reference] ,[Description],[ActionTime]) VALUES   " +
                        $"('{userActivityReport.UserId}'   ,'{userActivityReport.Module}'   ,'{userActivityReport.Action}'   ,'{userActivityReport.Reference}','{userActivityReport.Description}','{DateTime.Now}')");

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

        public async Task AddActivityAsync(UserActivityReport userActivityReport, SqlConnection connection, SqlTransaction transaction, SqlCommand command)
        {            
            command.Connection = connection;
            command.Transaction = transaction;

            var sql = string.Format($"INSERT INTO [dbo].[UserActivityReport]   ([UserId], [Module]  ,[Action] ,[Reference] ,[Description],[ActionTime]) VALUES   " +
                       $"('{userActivityReport.UserId}'   ,'{userActivityReport.Module}'   ,'{userActivityReport.Action}'   ,'{userActivityReport.Reference}','{userActivityReport.Description}','{DateTime.Now}')");
                command.CommandText = sql;
                await command.ExecuteNonQueryAsync();
            }
        }

}
