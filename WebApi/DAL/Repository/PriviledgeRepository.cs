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
    public class PriviledgeRepository : IPriviledgeRepository
    {
        private readonly ISqlHelper _sqlHelper;        

        public PriviledgeRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;            
        }

        public async Task AddUserPriviledgeAsync(UserPriviledge userPriviledge)
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
                    sql = string.Format($"INSERT INTO [dbo].[UserPriviledgeMaster] ([Name],[Description]) VALUES  ('{userPriviledge.Name}'   ,'{userPriviledge.Description}')");
                    sql = sql + " Select Scope_Identity()";

                    command.CommandText = sql;
                    var userPriviledgeId = command.ExecuteScalar();
                    
                    foreach (UserPriviledgeDetail userPriviledgeDetail in userPriviledge.UserPriviledgeDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[UserPriviledgeDetails] ([UserPriviledgeId],[UserMenuActionId],[IsPermitted]) VALUES  ('{userPriviledgeId}','{userPriviledgeDetail.UserMenuActionId}','{userPriviledgeDetail.IsPermitted}')");
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

        public async Task DeleteUserPriviledgeAsync(int id)
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
                    sql = string.Format("DELETE FROM [dbo].[UserPriviledgeDetails]  WHERE UserPriviledgeId = '{0}'", id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format("DELETE FROM [dbo].[UserPriviledgeMaster]  WHERE id = '{0}'", id);
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

        public async Task<IEnumerable<UserPriviledge>> GetAllPriviledgeAsync()
        {
            var userPriviledges = new List<UserPriviledge>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[Name] ,[Description] FROM [dbo].[UserPriviledgeMaster] ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var userPriviledge = new UserPriviledge();
                    userPriviledge.Id = Convert.ToInt32(dataReader["Id"]);
                    userPriviledge.Name = Convert.ToString(dataReader["Name"]);
                    userPriviledge.Description = Convert.ToString(dataReader["Description"]);

                    userPriviledges.Add(userPriviledge);
                }
                dataReader.Close();
                conn.Close();
            }

            foreach (UserPriviledge userPriviledge in userPriviledges)
            {
                List<UserPriviledgeDetail> userPriviledgeDetails = new List<UserPriviledgeDetail>();
                commandText = string.Format($"SELECT UPD.[Id] ,UPD.[UserPriviledgeId] ,UPD.[UserMenuActionId] ,UPD.[IsPermitted],UMAA.MenuId,UM.Menu,UMAA.ActionId,UA.Action FROM [dbo].[UserPriviledgeDetails] UPD INNER JOIN UserMenuActionAssignment UMAA ON UMAA.Id = UPD.UserMenuActionId INNER JOIN UserMenu UM ON UM.Id = UMAA.MenuId INNER JOIN UserAction UA ON UA.ID = UMAA.ActionId  where UPD.UserPriviledgeId = '{userPriviledge.Id}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var userPriviledgeDetail = new UserPriviledgeDetail();
                        userPriviledgeDetail.Id = Convert.ToInt32(dataReader1["Id"]);
                        userPriviledgeDetail.UserPriviledgeId = Convert.ToInt32(dataReader1["UserPriviledgeId"]);
                        userPriviledgeDetail.UserMenuActionId = Convert.ToInt32(dataReader1["UserMenuActionId"]);

                        userPriviledgeDetail.IsPermitted = Convert.ToBoolean(dataReader1["IsPermitted"]);

                        userPriviledgeDetail.MenuId = Convert.ToInt32(dataReader1["MenuId"]);
                        userPriviledgeDetail.Menu = Convert.ToString(dataReader1["Menu"]);

                        userPriviledgeDetail.ActionId = Convert.ToInt32(dataReader1["ActionId"]);
                        userPriviledgeDetail.Action = Convert.ToString(dataReader1["Action"]);

                        userPriviledgeDetails.Add(userPriviledgeDetail);
                    }
                    dataReader1.Close();
                    conn.Close();
                }
                userPriviledge.UserPriviledgeDetails = userPriviledgeDetails;
            }
            return userPriviledges;
        }

        public async Task<UserPriviledge> GetPriviledgeAsync(int id)
        {
            var userPriviledge = new UserPriviledge();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[Name] ,[Description] FROM [dbo].[UserPriviledgeMaster] where Id = '{id}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {

                    userPriviledge.Id = Convert.ToInt32(dataReader["Id"]);
                    userPriviledge.Name = Convert.ToString(dataReader["Name"]);
                    userPriviledge.Description = Convert.ToString(dataReader["Description"]);          

                }
                dataReader.Close();
                conn.Close();
            }


            List<UserPriviledgeDetail> userPriviledgeDetails = new List<UserPriviledgeDetail>();
            commandText = string.Format($"SELECT UPD.[Id] ,UPD.[UserPriviledgeId] ,UPD.[UserMenuActionId] ,UPD.[IsPermitted],UMAA.MenuId,UM.Menu,UMAA.ActionId,UA.Action FROM [dbo].[UserPriviledgeDetails] UPD INNER JOIN UserMenuActionAssignment UMAA ON UMAA.Id = UPD.UserMenuActionId INNER JOIN UserMenu UM ON UM.Id = UMAA.MenuId INNER JOIN UserAction UA ON UA.ID = UMAA.ActionId  where UPD.UserPriviledgeId = '{id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var userPriviledgeDetail = new UserPriviledgeDetail();
                    userPriviledgeDetail.Id = Convert.ToInt32(dataReader1["Id"]);
                    userPriviledgeDetail.UserPriviledgeId = Convert.ToInt32(dataReader1["UserPriviledgeId"]);
                    userPriviledgeDetail.UserMenuActionId = Convert.ToInt32(dataReader1["UserMenuActionId"]);

                    userPriviledgeDetail.IsPermitted = Convert.ToBoolean(dataReader1["IsPermitted"]);

                    userPriviledgeDetail.MenuId = Convert.ToInt32(dataReader1["MenuId"]);
                    userPriviledgeDetail.Menu = Convert.ToString(dataReader1["Menu"]);

                    userPriviledgeDetail.ActionId = Convert.ToInt32(dataReader1["ActionId"]);
                    userPriviledgeDetail.Action = Convert.ToString(dataReader1["Action"]);                    

                    userPriviledgeDetails.Add(userPriviledgeDetail);
                }
                dataReader1.Close();
                conn.Close();
            }
            userPriviledge.UserPriviledgeDetails = userPriviledgeDetails;
            return userPriviledge;
        }

        public async Task<UserPriviledge> GetFormattedPriviledgeAsync(int id)
        {
            var userPriviledge = new UserPriviledge();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[Name] ,[Description] FROM [dbo].[UserPriviledgeMaster] where Id = '{id}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {

                    userPriviledge.Id = Convert.ToInt32(dataReader["Id"]);
                    userPriviledge.Name = Convert.ToString(dataReader["Name"]);
                    userPriviledge.Description = Convert.ToString(dataReader["Description"]);

                }
                dataReader.Close();
                conn.Close();
            }


            //List<UserPriviledgeDetail> userPriviledgeDetails = new List<UserPriviledgeDetail>();
            List<UserMenu> userMenus = new List<UserMenu>();
            commandText = string.Format($"SELECT UPD.[Id] ,UPD.[UserPriviledgeId] ,UPD.[UserMenuActionId] ,UPD.[IsPermitted] ,UPD.[IsLanding],UMAA.MenuId,UM.Menu,UMAA.ActionId,UA.Action,UM.IsReport,UM.url FROM [dbo].[UserPriviledgeDetails] UPD INNER JOIN UserMenuActionAssignment UMAA ON UMAA.Id = UPD.UserMenuActionId INNER JOIN UserMenu UM ON UM.Id = UMAA.MenuId INNER JOIN UserAction UA ON UA.ID = UMAA.ActionId  where UPD.UserPriviledgeId = '{id}' and UA.Id = 1 and UPD.IsPermitted = 1");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var userMenu = new UserMenu();
                    userMenu.UserActions = new List<UserAction>();
                    userMenu.UserReports = new List<UserReport>();
                    userMenu.MenuId = Convert.ToInt32(dataReader1["MenuId"]);
                    userMenu.Menu = Convert.ToString(dataReader1["Menu"]);
                    userMenu.Url = Convert.ToString(dataReader1["Url"]);
                    userMenu.IsViewPermitted = Convert.ToBoolean(dataReader1["IsPermitted"]); 
                    userMenu.IsReport = Convert.ToBoolean(dataReader1["IsReport"]);
                    userMenu.IsLanding = Convert.ToBoolean(dataReader1["IsLanding"]);
                    userMenus.Add(userMenu);
                }
                dataReader1.Close();
                conn.Close();
            }

            foreach(UserMenu userMenu in userMenus)
            {

                commandText = string.Format($"SELECT UPD.[Id] ,UPD.[UserPriviledgeId] ,UPD.[UserMenuActionId] ,UPD.[IsPermitted],UMAA.MenuId,UM.Menu,UMAA.ActionId,UA.Action FROM [dbo].[UserPriviledgeDetails] UPD INNER JOIN UserMenuActionAssignment UMAA ON UMAA.Id = UPD.UserMenuActionId INNER JOIN UserMenu UM ON UM.Id = UMAA.MenuId INNER JOIN UserAction UA ON UA.ID = UMAA.ActionId  where UPD.UserPriviledgeId = '{id}' and UM.Id = '{userMenu.MenuId}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {                        
                        var userAction = new UserAction();
                        userAction.Id = Convert.ToInt32(dataReader1["Id"]);
                        userAction.UserPriviledgeId = Convert.ToInt32(dataReader1["UserPriviledgeId"]);
                        userAction.UserMenuActionId = Convert.ToInt32(dataReader1["UserMenuActionId"]);
                        userAction.IsPermitted = Convert.ToBoolean(dataReader1["IsPermitted"]);
                        userAction.ActionId = Convert.ToInt32(dataReader1["ActionId"]);
                        userAction.Action = Convert.ToString(dataReader1["Action"]);

                        userMenu.UserActions.Add(userAction);
                    }
                    dataReader1.Close();
                    conn.Close();
                }
            }

            foreach (UserMenu userMenu in userMenus)
            {
                if (userMenu.IsReport)
                {
                    bool hasData = false;
                    commandText = string.Format($"SELECT  [Id]  ,[ReportId] ,[PriviledgeId]  ,[ColumnName] ,[ColumnDisplayName]  ,[IsVisible]  FROM [UserReport] where PriviledgeId = '{id}' and ReportId = '{userMenu.MenuId}'");

                    using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                    {
                        cmd1.CommandType = CommandType.Text;
                        conn.Open();
                        var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                        while (dataReader1.Read())
                        {
                            hasData = true;
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

                    if (!hasData)
                    {
                        commandText = string.Format($"SELECT  [Id]  ,[ReportId] ,[ColumnName] ,[ColumnDisplayName]  ,[IsVisible]  FROM [UserDefaultReport] where ReportId = '{userMenu.MenuId}'");

                        using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                        {
                            cmd1.CommandType = CommandType.Text;
                            conn.Open();
                            var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

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
                    
                }
            }
            userPriviledge.UserMenus = userMenus;
            return userPriviledge;
        }

        public async Task<IEnumerable<UserPriviledgeDetail>> GetRawPriviledgeAsync()
        {

            var userPriviledgeDetails = new List<UserPriviledgeDetail>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT UMAS.[Id]  ,UMAS.[MenuId],UM.MENU ,UMAS.[ActionId], UA.Action  ,UMAS.[IsApplicable],0 as [IsPermitted] FROM [UserMenuActionAssignment] UMAS INNER JOIN USERMENU UM ON UMAS.MenuId = UM.ID INNER JOIN USERACTION UA ON UMAS.ActionId = UA.ID order by UM.MENU,UMAS.[ActionId]");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var userPriviledgeDetail = new UserPriviledgeDetail();

                    userPriviledgeDetail.Id = Convert.ToInt32(dataReader["Id"]);
                    userPriviledgeDetail.UserMenuActionId = Convert.ToInt32(dataReader["Id"]);
                    userPriviledgeDetail.MenuId = Convert.ToInt32(dataReader["MenuId"]);
                    userPriviledgeDetail.Menu = Convert.ToString(dataReader["MENU"]);
                    userPriviledgeDetail.ActionId = Convert.ToInt32(dataReader["ActionId"]);
                    userPriviledgeDetail.Action = Convert.ToString(dataReader["Action"]);
                    userPriviledgeDetail.IsApplicable = Convert.ToBoolean(dataReader["IsApplicable"]);
                    userPriviledgeDetail.IsPermitted = Convert.ToBoolean(dataReader["IsPermitted"]);

                    userPriviledgeDetails.Add(userPriviledgeDetail);
                }
                dataReader.Close();
                conn.Close();
            }

            userPriviledgeDetails.OrderBy(x => x.MenuId);          

            return userPriviledgeDetails;
        }

        public async Task UpdateUserPriviledgeAsync(UserPriviledge userPriviledge)
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
                    sql = string.Format("DELETE FROM [dbo].[UserPriviledgeDetails]  WHERE UserPriviledgeId = '{0}'", userPriviledge.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"UPDATE [dbo].[UserPriviledgeMaster] SET [Name] = '{userPriviledge.Name}'  ,[Description] = '{userPriviledge.Description}' WHERE id =  '{userPriviledge.Id}' ");

                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (UserPriviledgeDetail userPriviledgeDetail in userPriviledge.UserPriviledgeDetails)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[UserPriviledgeDetails] ([UserPriviledgeId],[UserMenuActionId],[IsPermitted]) VALUES  ('{userPriviledge.Id}','{userPriviledgeDetail.UserMenuActionId}','{userPriviledgeDetail.IsPermitted}')");
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
    }
}
