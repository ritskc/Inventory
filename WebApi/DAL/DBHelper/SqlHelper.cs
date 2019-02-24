using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DAL.DBHelper;

namespace DAL.DBHelper
{
    public class SqlHelper : ISqlHelper
    {
        public int ExecuteNonQuery(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect 
                    // type is only for OLE DB.  
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);
                    cmd.CommandTimeout = 1800;
                    conn.Open();
                    var result = cmd.ExecuteNonQuery();
                    conn.Close();
                    return result;

                }
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    // There're three command types: StoredProcedure, Text, TableDirect. The TableDirect 
                    // type is only for OLE DB.  
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    var result = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                    return result;
                }
            }
        }

        public SqlDataReader ExecuteReader(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                // When using CommandBehavior.CloseConnection, the connection will be closed when the 
                // IDataReader is closed.
                var result = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                conn.Close();

                return result;

            }
        }

        public List<T> ExecuteReader<T>(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                // When using CommandBehavior.CloseConnection, the connection will be closed when the 
                // IDataReader is closed.
                var result = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                List<T> list = new List<T>();
                T obj = default(T);
                while (result.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (!object.Equals(result[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, result[prop.Name], null);
                        }
                    }
                    list.Add(obj);
                }


                conn.Close();

                return list;

            }
        }

        public async Task<SqlDataReader> ExecuteReaderAsync(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            SqlConnection conn = new SqlConnection(connectionString);

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = commandType;
                cmd.Parameters.AddRange(parameters);

                conn.Open();
                // When using CommandBehavior.CloseConnection, the connection will be closed when the 
                // IDataReader is closed.
                var result = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                conn.Close();
                return result;
            }
        }

        public object ExecuteScalar(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    var result = cmd.ExecuteScalar();
                    conn.Close();
                    return result;
                }
            }
        }

        public async Task<object> ExecuteScalarAsync(string connectionString, string commandText, CommandType commandType, params SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = commandType;
                    cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    var result = await cmd.ExecuteScalarAsync();
                    conn.Close();
                    return result;
                }
            }
        }
    }

}
