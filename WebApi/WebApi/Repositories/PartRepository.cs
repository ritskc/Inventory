using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DBHelper;
using WebApi.IRepositories;
using WebApi.Models;
using WebApi.Settings;

namespace WebApi.Repositories
{
    public class PartRepository : IPartRepository
    {
        private readonly ISqlHelper _sqlHelper;
        
        public PartRepository(ISqlHelper sqlHelper)
        {            
            _sqlHelper = sqlHelper;
        }

        public async Task<IEnumerable<Part>> GetAllPartsAsync()
        {            
            List<Part> parts = new List<Part>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = "SELECT  [id] ,[name] ,[description] ,[issample]  FROM [tblPart] WITH(NOLOCK) ";
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;               

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var part = new Part();
                    part.Id = Convert.ToInt64(dataReader["id"]);
                    part.Name = Convert.ToString(dataReader["name"]);
                    part.Description = Convert.ToString(dataReader["description"]);
                    part.IsSample = Convert.ToBoolean(dataReader["issample"]);
                    parts.Add(part);
                }
                conn.Close();
            }
            return parts;
        }

        public async Task<int> AddPartAsync(Part part)
        {
            string sql = string.Format("INSERT INTO [tblPart] ([name] ,[description] ,[issample] ) VALUES ('{0}', '{1}','{2}')",part.Name,part.Description,part.IsSample);

            return await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
        }

        public async Task<int> UpdatePartAsync(Part part)
        {
            string sql = string.Format("UPDATE [dbo].[tblPart] SET[name] = '{0}'  ,[description] ='{1}' ,[issample] ='{2}'  WHERE id = '{3}'",part.Name,part.Description,part.IsSample,part.Id);

            return await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
        }

        public async Task<int> DeletePartAsync(long id)
        {
            string sql = string.Format("DELETE FROM [dbo].[tblPart]  WHERE id = '{0}'", id);

            return await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
        }        
    }
}
