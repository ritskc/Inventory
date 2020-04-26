using DAL.DBHelper;
using DAL.IRepository;
using DAL.Models;
using DAL.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class UserReportRepository:IUserReportRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public UserReportRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }       

        public async Task<IEnumerable<UserMenuReport>> GetDefaultReportsAsync()
        {
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            //List<UserPriviledgeDetail> userPriviledgeDetails = new List<UserPriviledgeDetail>();
            List<UserMenuReport> userMenus = new List<UserMenuReport>();
            var commandText = string.Format($"SELECT [Id]  ,[Menu] ,[IsReport] ,[url]  FROM [dbo].[UserMenu] where [IsReport] = 1 ");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = await cmd1.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var userMenu = new UserMenuReport();
                    userMenu.Id = Convert.ToInt32(dataReader1["Id"]);
                    userMenu.Report = Convert.ToString(dataReader1["Menu"]);

                    userMenu.UserReports = new List<UserReport>();
                    userMenus.Add(userMenu);
                }
                dataReader1.Close();
                conn.Close();
            }

            foreach (UserMenuReport userMenu in userMenus)
            {
                commandText = string.Format($"SELECT  [Id]  ,[ReportId] ,[ColumnName] ,[ColumnDisplayName]  ,[IsVisible]  FROM [UserDefaultReport] where ReportId = '{userMenu.Id}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = await cmd1.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var UserReportInfo = new UserReport();
                        UserReportInfo.Id = Convert.ToInt32(dataReader1["Id"]);
                        UserReportInfo.ReportId = Convert.ToInt32(dataReader1["ReportId"]);
                        UserReportInfo.ColumnName = Convert.ToString(dataReader1["ColumnName"]);
                        UserReportInfo.ColumnDisplayName = Convert.ToString(dataReader1["ColumnDisplayName"]);
                        UserReportInfo.IsVisible = Convert.ToBoolean(dataReader1["IsVisible"]);

                        userMenu.UserReports.Add(UserReportInfo);
                    }
                    dataReader1.Close();
                    conn.Close();
                }

            }
            return userMenus;
        }

        public async Task<IEnumerable<UserMenuReport>> GetReportsAsync()
        {
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            //List<UserPriviledgeDetail> userPriviledgeDetails = new List<UserPriviledgeDetail>();
            List<UserMenuReport> userMenus = new List<UserMenuReport>();
            var commandText = string.Format($"SELECT UM.ID,um.[Menu] ,[PriviledgeId]  FROM [dbo].[UserReport] UR inner join UserMenu UM ON UR.ReportId = um.Id GROUP BY UM.ID,UM.MENU,PriviledgeId");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = await cmd1.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var userMenu = new UserMenuReport();
                    userMenu.Id = Convert.ToInt32(dataReader1["Id"]);
                    userMenu.Report = Convert.ToString(dataReader1["Menu"]);
                    userMenu.PriviledgeId = Convert.ToInt32(dataReader1["PriviledgeId"]);
                    userMenu.UserReports = new List<UserReport>();
                    userMenus.Add(userMenu);
                }
                dataReader1.Close();
                conn.Close();
            }

            foreach (UserMenuReport userMenu in userMenus)
            {
                commandText = string.Format($"SELECT [Id] ,[ReportId] ,[PriviledgeId] ,[ColumnName] ,[ColumnDisplayName] ,[IsVisible]  FROM [dbo].[UserReport] where ReportId = '{userMenu.Id}' and PriviledgeId = '{userMenu.PriviledgeId}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = await cmd1.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var UserReportInfo = new UserReport();
                        UserReportInfo.Id = Convert.ToInt32(dataReader1["Id"]);
                        UserReportInfo.ReportId = Convert.ToInt32(dataReader1["ReportId"]);
                        UserReportInfo.ColumnName = Convert.ToString(dataReader1["ColumnName"]);
                        UserReportInfo.ColumnDisplayName = Convert.ToString(dataReader1["ColumnDisplayName"]);
                        UserReportInfo.IsVisible = Convert.ToBoolean(dataReader1["IsVisible"]);

                        userMenu.UserReports.Add(UserReportInfo);
                    }
                    dataReader1.Close();
                    conn.Close();
                }

            }
            return userMenus;
        }

        public async Task AddUserPriviledgeAsync(UserMenuReport userMenuReport)
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
                    sql = string.Format($"DELETE FROM [dbo].[UserReport] WHERE PriviledgeId = '{userMenuReport.PriviledgeId}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (UserReport userReport in userMenuReport.UserReports)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[UserReport] ([ReportId] ,[PriviledgeId] ,[ColumnName] ,[ColumnDisplayName] ,[IsVisible])  VALUES ('{userMenuReport.Id}', '{userMenuReport.PriviledgeId}','{userReport.ColumnName}', '{userReport.ColumnDisplayName}','{userReport.IsVisible}')");
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

        public async Task DeleteserPriviledgeAsync(int priviledgeId)
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
                    sql = string.Format($"DELETE FROM [dbo].[UserReport] WHERE PriviledgeId = '{priviledgeId}'");
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
