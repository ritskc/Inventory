using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IUserActivityReportRepository
    {
        Task AddActivityAsync(UserActivityReport userActivityReport);
        Task AddActivityAsync(UserActivityReport userActivityReport, SqlConnection connection, SqlTransaction transaction, SqlCommand command);
    }
}
