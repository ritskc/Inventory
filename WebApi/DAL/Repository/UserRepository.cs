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
        public async Task<User> GetUserPropertyAsync(string userName)
        {
            var user = new User();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format("SELECT [Id] ,[UserName] ,[Password] ,[FirstName] ,[LastName] ,[Email] ,[PriviledgeId] ," +
                "[UserTypeId] ,[MapId] FROM [User]  WITH(NOLOCK) WHERE UserName ='{0}' ", userName);

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
                    user.MapId = Convert.ToInt32(dataReader["MapId"]);
                }
                conn.Close();
            }

            List<Report> reports = new List<Report>();
            commandText = string.Format("SELECT distinct  R.Name as ReportName, URP.[PriviledgeId] , URP.[ReportId] FROM [UserReportPriviledges] URP INNER JOIN[USER] U ON U.PriviledgeId = URP.PriviledgeId " +
                "INNER JOIN[REPORT] R ON R.Id = URP.ReportId", user.Id);

            using (SqlCommand cmd3 = new SqlCommand(commandText, conn))
            {
                cmd3.CommandType = CommandType.Text;
                conn.Open();
                var dataReader3 = cmd3.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader3.Read())
                {
                    var report = new Report();
                    report.Id = Convert.ToInt32(dataReader3["ReportId"]);
                    report.PriviledgeId = Convert.ToInt32(dataReader3["PriviledgeId"]);
                    report.Name = Convert.ToString(dataReader3["ReportName"]);
                   
                    reports.Add(report);
                }
            }
            user.Reports = reports;
            conn.Close();


            List<UserReportPriviledge> userReportPriviledges = new List<UserReportPriviledge>();
            commandText = string.Format("SELECT URP.[Id], R.Name as ReportName, URP.[PriviledgeId] , URP.[ReportId],[ReportColumnId] , URP.[ColumnName] ,[View] ,[Edit],[Sort],[SortOrder] ," +
                "[ColumnTypeId] ,[ColumnSequence] FROM [UserReportPriviledges] URP INNER JOIN[USER] U ON U.PriviledgeId = URP.PriviledgeId INNER JOIN[REPORT] R ON R.Id = URP.ReportId  WHERE U.Id = '{0}'", user.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var userReportPriviledge = new UserReportPriviledge();
                    userReportPriviledge.Id = Convert.ToInt32(dataReader1["Id"]);
                    userReportPriviledge.PriviledgeId = Convert.ToInt32(dataReader1["PriviledgeId"]);
                    userReportPriviledge.ReportId = Convert.ToInt32(dataReader1["ReportId"]);
                    userReportPriviledge.ReportName = Convert.ToString(dataReader1["ReportName"]);
                    userReportPriviledge.ReportColumnId = Convert.ToInt32(dataReader1["ReportColumnId"]);
                    userReportPriviledge.ColumnName = Convert.ToString(dataReader1["ColumnName"]);
                    userReportPriviledge.View = Convert.ToBoolean(dataReader1["Edit"]);
                    userReportPriviledge.Edit = Convert.ToBoolean(dataReader1["Sort"]);
                    userReportPriviledge.Sort = Convert.ToBoolean(dataReader1["Sort"]);
                    userReportPriviledge.SortOrder = Convert.ToBoolean(dataReader1["SortOrder"]);
                    userReportPriviledge.ColumnTypeId = Convert.ToInt32(dataReader1["ColumnTypeId"]);
                    userReportPriviledge.ColumnSequence = Convert.ToInt32(dataReader1["ColumnSequence"]);

                    var rep = user.Reports.Where(x => x.Id == userReportPriviledge.ReportId).FirstOrDefault();

                    if (rep.UserReportPriviledges == null)
                        rep.UserReportPriviledges = new List<UserReportPriviledge>();
                    rep.UserReportPriviledges.Add(userReportPriviledge);
                }
            }
            //user.UserReportPriviledges = userReportPriviledges;
            conn.Close();

  
            List<SSRSReport> sSRSReports = new List<SSRSReport>();
            commandText = "SELECT [Id],[Name],[Path] FROM[SSRSReport]  WITH(NOLOCK) ";

            using (SqlCommand cmd2 = new SqlCommand(commandText, conn))
            {
                cmd2.CommandType = CommandType.Text;

                conn.Open();

                var dataReader2 = await cmd2.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader2.Read())
                {
                    var sSRSReport = new SSRSReport();
                    sSRSReport.Id = Convert.ToInt32(dataReader2["Id"]);
                    sSRSReport.Name = Convert.ToString(dataReader2["Name"]);
                    sSRSReport.Path = Convert.ToString(dataReader2["Path"]);


                    sSRSReports.Add(sSRSReport);
                }
                conn.Close();
            }
            user.SSRSReports = sSRSReports;
            return user;
        }
    }
}
