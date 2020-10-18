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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public CompanyRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<IEnumerable<Company>> GetAllCompanyAsync()
        {
            try
            {
                List<Company> companys = new List<Company>();
                SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
                var commandText = "SELECT  [id] ,[Name]  ,[Address]  ,[PhoneNo]  ,[FaxNo]  ,[EMail] ,[ContactPersonName] ,[WHName] ,[WHAddress] ,[WHPhoneNo] ,[WHEmail] FROM [company] WITH(NOLOCK) ";
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        var company = new Company();
                        company.Id = Convert.ToInt32(dataReader["Id"]);
                        company.Name = Convert.ToString(dataReader["Name"]);
                        company.Address = Convert.ToString(dataReader["Address"]);
                        company.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                        company.FaxNo = Convert.ToString(dataReader["FaxNo"]);
                        company.EMail = Convert.ToString(dataReader["EMail"]);
                        company.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                        company.WHName = Convert.ToString(dataReader["WHName"]);
                        company.WHAddress = Convert.ToString(dataReader["WHAddress"]);
                        company.WHPhoneNo = Convert.ToString(dataReader["WHPhoneNo"]);
                        company.WHEmail = Convert.ToString(dataReader["WHEmail"]);
                        companys.Add(company);
                    }
                    conn.Close();
                }

                foreach (Company company in companys)
                {                    
                    commandText = string.Format("SELECT [id]  ,[CompanyId] ,[Name]  FROM [Warehouse] WITH(NOLOCK)  WHERE CompanyId = '{0}'", company.Id);
                    company.Warehouses = new List<Warehouse>();
                    using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                    {
                        cmd1.CommandType = CommandType.Text;
                        conn.Open();
                        var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                        while (dataReader1.Read())
                        {
                            var warehouse = new Warehouse();
                            warehouse.Id = Convert.ToInt32(dataReader1["Id"]);
                            warehouse.CompanyId = Convert.ToInt32(dataReader1["CompanyId"]);
                            warehouse.Name = Convert.ToString(dataReader1["Name"]);
                            company.Warehouses.Add(warehouse);
                        }
                    }                    
                    conn.Close();
                }

                return companys;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public IEnumerable<Company> GetAllCompany()
        {
            List<Company> companys = new List<Company>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = "SELECT  [id] ,[Name]  ,[Address]  ,[PhoneNo]  ,[FaxNo]  ,[EMail] ,[ContactPersonName] ,[WHName] ,[WHAddress] ,[WHPhoneNo] ,[WHEmail] FROM [company] WITH(NOLOCK) ";
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var company = new Company();
                    company.Id = Convert.ToInt32(dataReader["Id"]);
                    company.Name = Convert.ToString(dataReader["Name"]);
                    company.Address = Convert.ToString(dataReader["Address"]);
                    company.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                    company.FaxNo = Convert.ToString(dataReader["FaxNo"]);
                    company.EMail = Convert.ToString(dataReader["EMail"]);
                    company.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                    company.WHName = Convert.ToString(dataReader["WHName"]);
                    company.WHAddress = Convert.ToString(dataReader["WHAddress"]);
                    company.WHPhoneNo = Convert.ToString(dataReader["WHPhoneNo"]);
                    company.WHEmail = Convert.ToString(dataReader["WHEmail"]);
                    companys.Add(company);
                }
                conn.Close();
            }

            foreach (Company company in companys)
            {
                commandText = string.Format("SELECT [id]  ,[CompanyId] ,[Name]  FROM [Warehouse] WITH(NOLOCK)  WHERE CompanyId = '{0}'", company.Id);
                company.Warehouses = new List<Warehouse>();
                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var warehouse = new Warehouse();
                        warehouse.Id = Convert.ToInt32(dataReader1["Id"]);
                        warehouse.CompanyId = Convert.ToInt32(dataReader1["CompanyId"]);
                        warehouse.Name = Convert.ToString(dataReader1["Name"]);
                        company.Warehouses.Add(warehouse);
                    }
                }
                conn.Close();
            }

            return companys;

        }

        public async Task<Company> GetCompanyAsync(int id)
        {
            try
            {
                Company company = new Company();
                SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);                
                var commandText = string.Format("SELECT  [id] ,[Name]  ,[Address]  ,[PhoneNo]  ,[FaxNo]  ,[EMail] ,[ContactPersonName] ,[WHName] ,[WHAddress] ,[WHPhoneNo] ,[WHEmail] FROM [company] WITH(NOLOCK) WHERE id = '{0}'", id);
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {                        
                        company.Id = Convert.ToInt32(dataReader["Id"]);
                        company.Name = Convert.ToString(dataReader["Name"]);
                        company.Address = Convert.ToString(dataReader["Address"]);
                        company.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                        company.FaxNo = Convert.ToString(dataReader["FaxNo"]);
                        company.EMail = Convert.ToString(dataReader["EMail"]);
                        company.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                        company.WHName = Convert.ToString(dataReader["WHName"]);
                        company.WHAddress = Convert.ToString(dataReader["WHAddress"]);
                        company.WHPhoneNo = Convert.ToString(dataReader["WHPhoneNo"]);
                        company.WHEmail = Convert.ToString(dataReader["WHEmail"]);
                        
                    }
                    conn.Close();
                }

                commandText = string.Format("SELECT [id]  ,[CompanyId] ,[Name]  FROM [Warehouse] WITH(NOLOCK)  WHERE CompanyId = '{0}'", company.Id);
                company.Warehouses = new List<Warehouse>();
                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var warehouse = new Warehouse();
                        warehouse.Id = Convert.ToInt32(dataReader1["Id"]);
                        warehouse.CompanyId = Convert.ToInt32(dataReader1["CompanyId"]);
                        warehouse.Name = Convert.ToString(dataReader1["Name"]);
                        company.Warehouses.Add(warehouse);
                    }
                }
                conn.Close();

                return company;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public Company GetCompany(int id)
        {
            try
            {
                Company company = new Company();
                SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
                var commandText = string.Format("SELECT  [id] ,[Name]  ,[Address]  ,[PhoneNo]  ,[FaxNo]  ,[EMail] ,[ContactPersonName] ,[WHName] ,[WHAddress] ,[WHPhoneNo] ,[WHEmail] FROM [company] WITH(NOLOCK) WHERE id = '{0}'", id);
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        company.Id = Convert.ToInt32(dataReader["Id"]);
                        company.Name = Convert.ToString(dataReader["Name"]);
                        company.Address = Convert.ToString(dataReader["Address"]);
                        company.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                        company.FaxNo = Convert.ToString(dataReader["FaxNo"]);
                        company.EMail = Convert.ToString(dataReader["EMail"]);
                        company.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                        company.WHName = Convert.ToString(dataReader["WHName"]);
                        company.WHAddress = Convert.ToString(dataReader["WHAddress"]);
                        company.WHPhoneNo = Convert.ToString(dataReader["WHPhoneNo"]);
                        company.WHEmail = Convert.ToString(dataReader["WHEmail"]);

                    }
                    conn.Close();
                }
                return company;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task<Company> GetCompanyByNameAsync(string name)
        {
            try
            {
                Company company = new Company();
                SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
                var commandText = string.Format("SELECT  [id] ,[Name]  ,[Address]  ,[PhoneNo]  ,[FaxNo]  ,[EMail] ,[ContactPersonName] ,[WHName] ,[WHAddress] ,[WHPhoneNo] ,[WHEmail] FROM [company] WITH(NOLOCK) WHERE Name = '{0}'", name);
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        company.Id = Convert.ToInt32(dataReader["Id"]);
                        company.Name = Convert.ToString(dataReader["Name"]);
                        company.Address = Convert.ToString(dataReader["Address"]);
                        company.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                        company.FaxNo = Convert.ToString(dataReader["FaxNo"]);
                        company.EMail = Convert.ToString(dataReader["EMail"]);
                        company.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                        company.WHName = Convert.ToString(dataReader["WHName"]);
                        company.WHAddress = Convert.ToString(dataReader["WHAddress"]);
                        company.WHPhoneNo = Convert.ToString(dataReader["WHPhoneNo"]);
                        company.WHEmail = Convert.ToString(dataReader["WHEmail"]);

                    }
                    conn.Close();
                }
                return company;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }

        public async Task<int> AddCompanyAsync(Company company)
        {
            string sql = string.Format("INSERT INTO [Company] ([Name],[Address],[PhoneNo] ,[FaxNo] ,[EMail] ,[ContactPersonName] ,[WHName] ,[WHAddress] ,[WHPhoneNo] ,[WHEmail]) VALUES ('{0}', '{1}','{2}','{3}', '{4}','{5}','{6}', '{7}','{8}','{9}')",
                                        company.Name.Replace("'", "''"), company.Address.Replace("'", "''"), company.PhoneNo, company.FaxNo, company.EMail.Replace("'", "''"), company.ContactPersonName.Replace("'", "''"), company.WHName.Replace("'", "''"), company.WHAddress.Replace("'", "''"), company.PhoneNo.Replace("'", "''"), company.WHEmail.Replace("'", "''"));

            return await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
        }

        public async Task<int> UpdateCompanyAsync(Company company)
        {            
            string sql = string.Format("UPDATE [dbo].[Company] SET [name] = '{0}'  ,[Address] ='{1}' ,[PhoneNo] ='{2}' ,[FaxNo] = '{3}'  ,[EMail] ='{4}' ,[ContactPersonName] ='{5}',[WHName] = '{6}'  ,[WHAddress] ='{7}' ,[WHPhoneNo] ='{8}',[WHEmail] ='{9}'" +
                " WHERE id = '{10}'", company.Name.Replace("'", "''"), company.Address.Replace("'", "''"), company.PhoneNo, company.FaxNo, company.EMail.Replace("'", "''"), company.ContactPersonName.Replace("'", "''"), company.WHName.Replace("'", "''"), company.WHAddress.Replace("'", "''"), company.WHPhoneNo, company.WHEmail.Replace("'", "''"), company.Id);

            await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

            foreach (Warehouse warehouse in company.Warehouses)
            {
                if (warehouse.Id == 0)
                {
                    sql = string.Format($"INSERT INTO [dbo].[Warehouse] ([CompanyId],[Name]) VALUES ('{company.Id}','{warehouse.Name}')");
                    await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                }
                else
                {
                    sql = string.Format("UPDATE [dbo].[Warehouse] SET [name] = '{0}' where id = '{1}' ", warehouse.Name,warehouse.Id  );
                    await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                }
            }

            return 1;
        }

        public async Task<int> DeleteCompanyAsync(long id)
        {
            string sql = string.Format("DELETE FROM [dbo].[Company]  WHERE id = '{0}'", id);

            return await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
        }
    }
}
