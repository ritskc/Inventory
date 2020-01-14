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
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public SupplierRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<IEnumerable<Supplier>> GetAllSupplierAsync(int companyId)
        {
            List<Supplier> suppliers = new List<Supplier>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = string.Format("SELECT [id],[CompanyId],[Name],[ContactPersonName],[PhoneNo],[EmailID],[Address],[City],[State],[Country]," +
                                 "[ZIPCode],[FAXNo],[DateFormat],[noofstages],[CompanyProfileID] FROM [dbo].[supplier] WITH(NOLOCK) WHERE CompanyId = '{0}'", companyId);
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var supplier = new Supplier();
                    supplier.Id = Convert.ToInt32(dataReader["Id"]);
                    supplier.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    supplier.Name = Convert.ToString(dataReader["Name"]);
                    supplier.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                    supplier.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                    supplier.EmailID = Convert.ToString(dataReader["EmailID"]);
                    supplier.Address = Convert.ToString(dataReader["Address"]);
                    supplier.City = Convert.ToString(dataReader["City"]);
                    supplier.State = Convert.ToString(dataReader["State"]);
                    supplier.Country = Convert.ToString(dataReader["Country"]);
                    supplier.ZIPCode = Convert.ToString(dataReader["ZIPCode"]);
                    supplier.FAXNo = Convert.ToString(dataReader["FAXNo"]);
                    supplier.Address = Convert.ToString(dataReader["Address"]);
                    supplier.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                    supplier.DateFormat = Convert.ToString(dataReader["DateFormat"]);
                    supplier.noofstages = Convert.ToInt32(dataReader["noofstages"]);
                    supplier.CompanyProfileID = Convert.ToInt32(dataReader["CompanyProfileID"]);

                    suppliers.Add(supplier);
                }
                conn.Close();
            }
            foreach (Supplier supplier in suppliers)
            {
                List<SupplierTerms> lstTerms = new List<SupplierTerms>();
                commandText = string.Format("SELECT [id],[supplierId],[sequenceNo],[terms] FROM [dbo].[supplierterms] WITH(NOLOCK)  WHERE supplierid = '{0}'", supplier.Id);

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var terms = new SupplierTerms();
                        terms.Id = Convert.ToInt32(dataReader1["Id"]);
                        terms.SequenceNo = Convert.ToInt32(dataReader1["SequenceNo"]);
                        terms.Terms = Convert.ToString(dataReader1["Terms"]);
                        lstTerms.Add(terms);
                    }
                }
                supplier.Terms = lstTerms;
                conn.Close();
            }


            return suppliers.OrderBy(x => x.Name);
        }

        public async Task<Supplier> GetSupplierAsync(int id)
        {
            Supplier supplier = new Supplier();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = string.Format("SELECT [id],[CompanyId],[Name],[ContactPersonName],[PhoneNo],[EmailID],[Address],[City],[State],[Country]," +
                                 "[ZIPCode],[FAXNo],[DateFormat],[noofstages],[CompanyProfileID] FROM [dbo].[supplier] WITH(NOLOCK) WHERE id = '{0}'", id);
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    supplier.Id = Convert.ToInt32(dataReader["Id"]);
                    supplier.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    supplier.Name = Convert.ToString(dataReader["Name"]);
                    supplier.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                    supplier.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                    supplier.EmailID = Convert.ToString(dataReader["EmailID"]);
                    supplier.Address = Convert.ToString(dataReader["Address"]);
                    supplier.City = Convert.ToString(dataReader["City"]);
                    supplier.State = Convert.ToString(dataReader["State"]);
                    supplier.Country = Convert.ToString(dataReader["Country"]);
                    supplier.ZIPCode = Convert.ToString(dataReader["ZIPCode"]);
                    supplier.FAXNo = Convert.ToString(dataReader["FAXNo"]);
                    supplier.Address = Convert.ToString(dataReader["Address"]);
                    supplier.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                    supplier.DateFormat = Convert.ToString(dataReader["DateFormat"]);
                    supplier.noofstages = Convert.ToInt32(dataReader["noofstages"]);
                    supplier.CompanyProfileID = Convert.ToInt32(dataReader["CompanyProfileID"]);
                }
                conn.Close();
            }

            List<SupplierTerms> lstTerms = new List<SupplierTerms>();
            commandText = string.Format("SELECT [id],[supplierId],[sequenceNo],[terms] FROM [dbo].[supplierterms] WITH(NOLOCK)  WHERE supplierid = '{0}'", supplier.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var terms = new SupplierTerms();
                    terms.Id = Convert.ToInt32(dataReader1["Id"]);
                    terms.SequenceNo = Convert.ToInt32(dataReader1["SequenceNo"]);
                    terms.Terms = Convert.ToString(dataReader1["Terms"]);
                    lstTerms.Add(terms);
                }
            }
            supplier.Terms = lstTerms;
            conn.Close();



            return supplier;
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            string sql = string.Format("INSERT INTO [Supplier] ([CompanyId],[Name],[ContactPersonName],[PhoneNo],[EmailID],[Address],[City],[State],[Country]," +
                                 "[ZIPCode],[FAXNo],[DateFormat],[noofstages],[CompanyProfileID]) VALUES ('{0}', '{1}','{2}','{3}', '{4}','{5}','{6}', '{7}','{8}','{9}','{10}', '{11}','{12}','{13}')",
                                        supplier.CompanyId, supplier.Name.Replace("'", "''"), supplier.ContactPersonName.Replace("'", "''"), supplier.PhoneNo, supplier.EmailID.Replace("'", "''"), supplier.Address.Replace("'", "''"), supplier.City.Replace("'", "''"), supplier.State.Replace("'", "''"), supplier.Country.Replace("'", "''"),
                                        supplier.ZIPCode.Replace("'", "''"), supplier.FAXNo, supplier.DateFormat, supplier.noofstages, supplier.CompanyProfileID);

            sql = sql + " Select Scope_Identity()";


            var supplierId = _sqlHelper.ExecuteScalar(ConnectionSettings.ConnectionString, sql, CommandType.Text);


            foreach (SupplierTerms term in supplier.Terms)
            {
                sql = string.Format("INSERT INTO [dbo].[supplierterms]([supplierId],[sequenceNo],[terms]) VALUES ('{0}', '{1}','{2}')",
                                            supplierId, term.SequenceNo, term.Terms.Replace("'", "''"));

                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
            }
        }

        public async Task UpdateSupplierAsync(Supplier supplier)
        {
            try
            {
                var sql = string.Format("DELETE FROM [dbo].[supplierterms]  WHERE supplierid = '{0}'", supplier.Id);

                _sqlHelper.ExecuteNonQuery(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                //sql = "UPDATE [dbo].[Supplier] SET [CompanyId] = '{0}',[Name]= '{1}',[ContactPersonName]= '{2}',[PhoneNo]= '{3}',[EmailID]= '{4}',[Address]= '{5}',[City]= '{6}',[State]= '{7}',[Country]= '{8}',[ZIPCode]= '{9}',[FAXNo]= '{10}',[DateFormat]= '{11}',[noofstages]= '{12}',[CompanyProfileID] = '{13}'  WHERE id = '{14}'";

                sql = string.Format($"UPDATE [dbo].[Supplier] SET [CompanyId] = '{supplier.CompanyId}',[Name]= '{supplier.Name.Replace("'", "''")}',[ContactPersonName]= '{supplier.ContactPersonName.Replace("'", "''")}',[PhoneNo]= '{supplier.PhoneNo.Replace("'", "''")}',[EmailID]= '{supplier.EmailID.Replace("'", "''")}',[Address]= '{supplier.Address.Replace("'", "''")}',[City]= '{supplier.City.Replace("'", "''")}'," +
                                            $"[State]= '{supplier.State.Replace("'", "''")}',[Country]= '{supplier.Country.Replace("'", "''")}',[ZIPCode]= '{supplier.ZIPCode.Replace("'", "''")}',[FAXNo]= '{supplier.FAXNo}',[DateFormat]= '{supplier.DateFormat}',[noofstages]= '{supplier.noofstages}',[CompanyProfileID] = '{supplier.CompanyProfileID}'  WHERE id = '{supplier.Id}' ");
                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                foreach (SupplierTerms term in supplier.Terms)
                {
                    sql = string.Format($"INSERT INTO [dbo].[supplierterms] ([supplierId],[sequenceNo],[terms]) VALUES ('{supplier.Id}', '{term.SequenceNo}','{term.Terms.Replace("'", "''")}')" );
                    await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        public async Task<int> DeleteSupplierAsync(long id)
        {
            string sql = string.Format("DELETE FROM [dbo].[Supplier]  WHERE id = '{0}'", id);

            await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

            sql = string.Format("DELETE FROM [dbo].[supplierterms]  WHERE supplierid = '{0}'", id);

            return await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
        }

        public async Task<Supplier> GetSupplierByNameAsync(int companyId, string supplierName)
        {
            Supplier supplier = new Supplier();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = string.Format("SELECT [id],[CompanyId],[Name],[ContactPersonName],[PhoneNo],[EmailID],[Address],[City],[State],[Country]," +
                                 "[ZIPCode],[FAXNo],[DateFormat],[noofstages],[CompanyProfileID] FROM [dbo].[supplier] WITH(NOLOCK) WHERE CompanyId = '{0}' and Name = '{1}'", companyId, supplierName);
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    supplier.Id = Convert.ToInt32(dataReader["Id"]);
                    supplier.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    supplier.Name = Convert.ToString(dataReader["Name"]);
                    supplier.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                    supplier.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                    supplier.EmailID = Convert.ToString(dataReader["EmailID"]);
                    supplier.Address = Convert.ToString(dataReader["Address"]);
                    supplier.City = Convert.ToString(dataReader["City"]);
                    supplier.State = Convert.ToString(dataReader["State"]);
                    supplier.Country = Convert.ToString(dataReader["Country"]);
                    supplier.ZIPCode = Convert.ToString(dataReader["ZIPCode"]);
                    supplier.FAXNo = Convert.ToString(dataReader["FAXNo"]);
                    supplier.Address = Convert.ToString(dataReader["Address"]);
                    supplier.PhoneNo = Convert.ToString(dataReader["PhoneNo"]);
                    supplier.DateFormat = Convert.ToString(dataReader["DateFormat"]);
                    supplier.noofstages = Convert.ToInt32(dataReader["noofstages"]);
                    supplier.CompanyProfileID = Convert.ToInt32(dataReader["CompanyProfileID"]);
                }
                conn.Close();
            }

            List<SupplierTerms> lstTerms = new List<SupplierTerms>();
            commandText = string.Format("SELECT [id],[supplierId],[sequenceNo],[terms] FROM [dbo].[supplierterms] WITH(NOLOCK)  WHERE supplierid = '{0}'", supplier.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var terms = new SupplierTerms();
                    terms.Id = Convert.ToInt32(dataReader1["Id"]);
                    terms.SequenceNo = Convert.ToInt32(dataReader1["SequenceNo"]);
                    terms.Terms = Convert.ToString(dataReader1["Terms"]);
                    lstTerms.Add(terms);
                }
            }
            supplier.Terms = lstTerms;
            conn.Close();



            return supplier;
        }
    }
}
