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
    public class UserRepository : IUserRepository
    {
        private readonly IPriviledgeRepository priviledgeRepository;
        public UserRepository(IPriviledgeRepository priviledgeRepository)
        {
            this.priviledgeRepository = priviledgeRepository;

        }
        public async Task AddUserAsync(User user)
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
                    string sql = string.Format($"INSERT INTO [dbo].[User]   ([UserName]   ,[Password]   ,[FirstName]   ,[LastName]   ,[Email]   ,[PriviledgeId]   ,[UserTypeId],[IsSuperAdmin] ) VALUES   ('{user.UserName}'   ,'{user.Password}'   ,'{user.FirstName}'   ,'{user.LastName}'   ,'{user.Email}'   ,'{user.PriviledgeId}'   ,'{user.UserTypeId}','{user.IsSuperAdmin}')");

                    sql = sql + " Select Scope_Identity()";
                    command.CommandText = sql;
                    var userId = await command.ExecuteScalarAsync();
                    

                    foreach (Int32 companyId in user.CompanyIds)
                    {                       

                        sql = string.Format($"INSERT INTO [dbo].[UserCompanyAssignment]   ([UserId]   ,[CompanyId])     VALUES   ('{userId}'   ,'{companyId}')");
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

        public async Task DeleteUserAsync(long id)
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
                    var sql = string.Format($"DELETE FROM [dbo].[UserCompanyAssignment] WHERE UserId = '{id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format("DELETE FROM [dbo].[User] WHERE  WHERE id = '{0}'", id);
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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = new List<User>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format("SELECT [Id] ,[UserName] ,[Password] ,[FirstName] ,[LastName] ,[Email] ,[PriviledgeId] ," +
                "[UserTypeId] ,[IsSuperAdmin] FROM [User]  WITH(NOLOCK)");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var user = new User();
                    user.Id = Convert.ToInt32(dataReader["Id"]);
                    user.UserName = Convert.ToString(dataReader["UserName"]).ToLower();
                    user.FirstName = Convert.ToString(dataReader["FirstName"]).ToLower();
                    user.Password = Convert.ToString(dataReader["Password"]).ToLower();
                    user.LastName = Convert.ToString(dataReader["LastName"]);
                    user.Email = Convert.ToString(dataReader["Email"]);
                    user.PriviledgeId = Convert.ToInt32(dataReader["PriviledgeId"]);
                    user.UserTypeId = Convert.ToInt32(dataReader["UserTypeId"]);
                    user.IsSuperAdmin = Convert.ToBoolean(dataReader["IsSuperAdmin"]);
                    users.Add(user);
                }
                dataReader.Close();
                conn.Close();
            }
            foreach (User user in users)
            {
                if (!user.IsSuperAdmin)
                {
                    //user.userPriviledge = await this.priviledgeRepository.GetPriviledgeAsync(user.PriviledgeId);                   

                    user.userPriviledge = await this.priviledgeRepository.GetFormattedPriviledgeAsync(user.PriviledgeId);

                    user.CompanyIds = new List<int>();
                    commandText = string.Format($"SELECT [Id],[UserId],[CompanyId]  FROM [UserCompanyAssignment]  WITH(NOLOCK) WHERE UserId ='{user.Id}'");

                    using (SqlCommand cmd = new SqlCommand(commandText, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        conn.Open();

                        var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                        while (dataReader.Read())
                        {

                            user.CompanyIds.Add(Convert.ToInt32(dataReader["CompanyId"]));
                        }
                        dataReader.Close();
                        conn.Close();
                    }
                }
            }            

            //foreach (User user in users)
            //{
            //    List<Report> reports = new List<Report>();
            //    commandText = string.Format("SELECT distinct  R.Name as ReportName, URP.[PriviledgeId] , URP.[ReportId] FROM [UserReportPriviledges] URP INNER JOIN[USER] U ON U.PriviledgeId = URP.PriviledgeId " +
            //        "INNER JOIN[REPORT] R ON R.Id = URP.ReportId WHERE URP.PriviledgeId = '{0}'", user.PriviledgeId.ToString());

            //    using (SqlCommand cmd3 = new SqlCommand(commandText, conn))
            //    {
            //        cmd3.CommandType = CommandType.Text;
            //        conn.Open();
            //        var dataReader3 = cmd3.ExecuteReader(CommandBehavior.CloseConnection);

            //        while (dataReader3.Read())
            //        {
            //            var report = new Report();
            //            report.Id = Convert.ToInt32(dataReader3["ReportId"]);
            //            //report.PriviledgeId = Convert.ToInt32(dataReader3["PriviledgeId"]);
            //            report.Name = Convert.ToString(dataReader3["ReportName"]);

            //            reports.Add(report);
            //        }
            //        user.Reports = reports;
            //        conn.Close();
            //    }
            //}


            //foreach (User user in users)
            //{
            //    List<UserReportPriviledge> userReportPriviledges = new List<UserReportPriviledge>();
            //    commandText = string.Format("SELECT URP.[Id], R.Name as ReportName, URP.[PriviledgeId] , URP.[ReportId],[ReportColumnId] , RC.ColumnName, URP.[ColumnName] AS 'DISPLAYNAME' ,[View] ,[Edit],[Sort],[SortOrder] ," +
            //        "[ColumnTypeId] ,[ColumnSequence] FROM [UserReportPriviledges] URP INNER JOIN[USER] U ON U.PriviledgeId = URP.PriviledgeId INNER JOIN[REPORT] R ON R.Id = URP.ReportId  " +
            //        "INNER JOIN ReportColumns RC ON RC.Id = URP.ReportColumnId WHERE U.PriviledgeId = '{0}'", user.PriviledgeId);

            //    using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            //    {
            //        cmd1.CommandType = CommandType.Text;
            //        conn.Open();
            //        var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

            //        while (dataReader1.Read())
            //        {
            //            var userReportPriviledge = new UserReportPriviledge();
            //            userReportPriviledge.Id = Convert.ToInt32(dataReader1["Id"]);
            //            //userReportPriviledge.PriviledgeId = Convert.ToInt32(dataReader1["PriviledgeId"]);
            //            userReportPriviledge.ReportId = Convert.ToInt32(dataReader1["ReportId"]);
            //            //userReportPriviledge.ReportName = Convert.ToString(dataReader1["ReportName"]);
            //            userReportPriviledge.ReportColumnId = Convert.ToInt32(dataReader1["ReportColumnId"]);
            //            userReportPriviledge.DisplayName = Convert.ToString(dataReader1["DisplayName"]);
            //            userReportPriviledge.ColumnName = Convert.ToString(dataReader1["ColumnName"]);
            //            userReportPriviledge.View = Convert.ToBoolean(dataReader1["Edit"]);
            //            userReportPriviledge.Edit = Convert.ToBoolean(dataReader1["Sort"]);
            //            userReportPriviledge.Sort = Convert.ToBoolean(dataReader1["Sort"]);
            //            userReportPriviledge.SortOrder = Convert.ToBoolean(dataReader1["SortOrder"]);
            //            userReportPriviledge.ColumnTypeId = Convert.ToInt32(dataReader1["ColumnTypeId"]);
            //            userReportPriviledge.ColumnSequence = Convert.ToInt32(dataReader1["ColumnSequence"]);

            //            var rep = user.Reports.Where(x => x.Id == userReportPriviledge.ReportId).FirstOrDefault();

            //            if (rep.UserReportPriviledges == null)
            //                rep.UserReportPriviledges = new List<UserReportPriviledge>();
            //            rep.UserReportPriviledges.Add(userReportPriviledge);
            //        }
            //    }
            //    //user.UserReportPriviledges = userReportPriviledges;
            //    conn.Close();
            //}
            //foreach (User user in users)
            //{
            //    List<SSRSReport> sSRSReports = new List<SSRSReport>();
            //    commandText = "SELECT [Id],[Name],[Path] FROM[SSRSReport]  WITH(NOLOCK) ";

            //    using (SqlCommand cmd2 = new SqlCommand(commandText, conn))
            //    {
            //        cmd2.CommandType = CommandType.Text;

            //        conn.Open();

            //        var dataReader2 = await cmd2.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            //        while (dataReader2.Read())
            //        {
            //            var sSRSReport = new SSRSReport();
            //            sSRSReport.Id = Convert.ToInt32(dataReader2["Id"]);
            //            sSRSReport.Name = Convert.ToString(dataReader2["Name"]);
            //            sSRSReport.Path = Convert.ToString(dataReader2["Path"]);


            //            sSRSReports.Add(sSRSReport);
            //        }
            //        conn.Close();
            //    }
            //    user.SSRSReports = sSRSReports;
            //}
            return users;
        }

        public async Task<User> GetUserPropertyAsync(string userName)
        {
            var user = new User();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format("SELECT [Id] ,[UserName] ,[Password] ,[FirstName] ,[LastName] ,[Email] ,[PriviledgeId] ," +
                "[UserTypeId],[IsSuperAdmin] FROM [User]  WITH(NOLOCK) WHERE UserName ='{0}' ", userName);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {

                    user.Id = Convert.ToInt32(dataReader["Id"]);
                    user.UserName = Convert.ToString(dataReader["UserName"]);
                    user.FirstName = Convert.ToString(dataReader["FirstName"]);
                    user.LastName = Convert.ToString(dataReader["LastName"]);
                    user.Email = Convert.ToString(dataReader["Email"]);
                    user.PriviledgeId = Convert.ToInt32(dataReader["PriviledgeId"]);
                    user.UserTypeId = Convert.ToInt32(dataReader["UserTypeId"]);
                    user.IsSuperAdmin = Convert.ToBoolean(dataReader["IsSuperAdmin"]);
                }
                dataReader.Close();
                conn.Close();
            }

            if (!user.IsSuperAdmin)
            {
                user.CompanyIds = new List<int>();
                commandText = string.Format($"SELECT [Id],[UserId],[CompanyId]  FROM [UserCompanyAssignment]  WITH(NOLOCK) WHERE UserId ='{user.Id}'");
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {

                        user.CompanyIds.Add(Convert.ToInt32(dataReader["CompanyId"]));
                    }
                    dataReader.Close();
                    conn.Close();
                }

                user.userPriviledge = await this.priviledgeRepository.GetPriviledgeAsync(user.PriviledgeId);
               
           }

            //List<Report> reports = new List<Report>();
            //commandText = string.Format("SELECT distinct  R.Name as ReportName, URP.[PriviledgeId] , URP.[ReportId] FROM [UserReportPriviledges] URP INNER JOIN[USER] U ON U.PriviledgeId = URP.PriviledgeId " +
            //    "INNER JOIN[REPORT] R ON R.Id = URP.ReportId WHERE URP.PriviledgeId = '{0}'", user.PriviledgeId.ToString());

            //using (SqlCommand cmd3 = new SqlCommand(commandText, conn))
            //{
            //    cmd3.CommandType = CommandType.Text;
            //    conn.Open();
            //    var dataReader3 = cmd3.ExecuteReader(CommandBehavior.CloseConnection);

            //    while (dataReader3.Read())
            //    {
            //        var report = new Report();
            //        report.Id = Convert.ToInt32(dataReader3["ReportId"]);
            //        //report.PriviledgeId = Convert.ToInt32(dataReader3["PriviledgeId"]);
            //        report.Name = Convert.ToString(dataReader3["ReportName"]);

            //        reports.Add(report);
            //    }
            //}
            //user.Reports = reports;
            //conn.Close();


            //List<UserReportPriviledge> userReportPriviledges = new List<UserReportPriviledge>();
            //commandText = string.Format("SELECT URP.[Id], R.Name as ReportName, URP.[PriviledgeId] , URP.[ReportId],[ReportColumnId] , RC.ColumnName, URP.[ColumnName] AS 'DISPLAYNAME' ,[View] ,[Edit],[Sort],[SortOrder] ," +
            //    "[ColumnTypeId] ,[ColumnSequence] FROM [UserReportPriviledges] URP INNER JOIN[USER] U ON U.PriviledgeId = URP.PriviledgeId INNER JOIN[REPORT] R ON R.Id = URP.ReportId  " +
            //    "INNER JOIN ReportColumns RC ON RC.Id = URP.ReportColumnId WHERE U.PriviledgeId = '{0}'", user.PriviledgeId);

            //using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            //{
            //    cmd1.CommandType = CommandType.Text;
            //    conn.Open();
            //    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

            //    while (dataReader1.Read())
            //    {
            //        var userReportPriviledge = new UserReportPriviledge();
            //        userReportPriviledge.Id = Convert.ToInt32(dataReader1["Id"]);
            //        //userReportPriviledge.PriviledgeId = Convert.ToInt32(dataReader1["PriviledgeId"]);
            //        userReportPriviledge.ReportId = Convert.ToInt32(dataReader1["ReportId"]);
            //        //userReportPriviledge.ReportName = Convert.ToString(dataReader1["ReportName"]);
            //        userReportPriviledge.ReportColumnId = Convert.ToInt32(dataReader1["ReportColumnId"]);
            //        userReportPriviledge.DisplayName = Convert.ToString(dataReader1["DisplayName"]);
            //        userReportPriviledge.ColumnName = Convert.ToString(dataReader1["ColumnName"]);
            //        userReportPriviledge.View = Convert.ToBoolean(dataReader1["Edit"]);
            //        userReportPriviledge.Edit = Convert.ToBoolean(dataReader1["Sort"]);
            //        userReportPriviledge.Sort = Convert.ToBoolean(dataReader1["Sort"]);
            //        userReportPriviledge.SortOrder = Convert.ToBoolean(dataReader1["SortOrder"]);
            //        userReportPriviledge.ColumnTypeId = Convert.ToInt32(dataReader1["ColumnTypeId"]);
            //        userReportPriviledge.ColumnSequence = Convert.ToInt32(dataReader1["ColumnSequence"]);

            //        var rep = user.Reports.Where(x => x.Id == userReportPriviledge.ReportId).FirstOrDefault();

            //        if (rep.UserReportPriviledges == null)
            //            rep.UserReportPriviledges = new List<UserReportPriviledge>();
            //        rep.UserReportPriviledges.Add(userReportPriviledge);
            //    }
            //}
            ////user.UserReportPriviledges = userReportPriviledges;
            //conn.Close();


            //List<SSRSReport> sSRSReports = new List<SSRSReport>();
            //commandText = "SELECT [Id],[Name],[Path] FROM[SSRSReport]  WITH(NOLOCK) ";

            //using (SqlCommand cmd2 = new SqlCommand(commandText, conn))
            //{
            //    cmd2.CommandType = CommandType.Text;

            //    conn.Open();

            //    var dataReader2 = await cmd2.ExecuteReaderAsync(CommandBehavior.CloseConnection);

            //    while (dataReader2.Read())
            //    {
            //        var sSRSReport = new SSRSReport();
            //        sSRSReport.Id = Convert.ToInt32(dataReader2["Id"]);
            //        sSRSReport.Name = Convert.ToString(dataReader2["Name"]);
            //        sSRSReport.Path = Convert.ToString(dataReader2["Path"]);


            //        sSRSReports.Add(sSRSReport);
            //    }
            //    conn.Close();
            //}
            //user.SSRSReports = sSRSReports;

           
            return user;
        }

        public async Task<User> GeUserbyIdAsync(int userId)
        {           
            return await Task.Run(() => GetAllUsersAsync().Result.Where(p => p.Id == userId).FirstOrDefault());
        }

        public async Task UpdateUserAsync(User user)
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
                    var sql = string.Format($"DELETE FROM [dbo].[UserCompanyAssignment] WHERE UserId = '{user.Id}'");
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    sql = string.Format($"UPDATE [dbo].[User]   SET [UserName] = '{user.UserName}' ,[Password] = '{user.Password}' ,[FirstName] = '{user.FirstName}' ,[LastName] = '{user.LastName}' ,[Email] = '{user.Email}' ,[PriviledgeId] = '{user.PriviledgeId}' ,[UserTypeId] = '{user.UserTypeId}' ,[IsSuperAdmin] = '{user.IsSuperAdmin}' WHERE id = '{0}'", user.Id);
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();

                    foreach (Int32 companyId in user.CompanyIds)
                    {
                        sql = string.Format($"INSERT INTO [dbo].[UserCompanyAssignment]   ([UserId]   ,[CompanyId])     VALUES   ('{user.Id}'   ,'{companyId}'");
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
