using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.DBHelper
{
    public interface ISqlHelper
    {
        Task<int> ExecuteNonQueryAsync(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters);

        Task<Object> ExecuteScalarAsync(String connectionString, String commandText,
         CommandType commandType, params SqlParameter[] parameters);

        Task<SqlDataReader> ExecuteReaderAsync(String connectionString, String commandText,
         CommandType commandType, params SqlParameter[] parameters);

        int ExecuteNonQuery(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters);

        Object ExecuteScalar(String connectionString, String commandText,
         CommandType commandType, params SqlParameter[] parameters);

        SqlDataReader ExecuteReader(String connectionString, String commandText,
         CommandType commandType, params SqlParameter[] parameters);

        List<T> ExecuteReader<T>(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters);
    }
}
