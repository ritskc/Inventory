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
    public class CustomerRepository:ICustomerRepository
    {
        private readonly ISqlHelper _sqlHelper;

        public CustomerRepository(ISqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync(int companyId)
        {
            List<Customer> customers = new List<Customer>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = string.Format($"SELECT [id],[CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber]" +
                ",[EmailAddress],[TruckType] ,[CollectFreight],[Comments] ,[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia] ,[invoicingtypeid],[endcustomername]" +
                $",[DisplayLineNo] FROM[customer] WITH(NOLOCK) WHERE[CompanyId] = '{companyId}'" );
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var customer = new Customer();
                    customer.Id = Convert.ToInt32(dataReader["Id"]);
                    customer.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    customer.Name = Convert.ToString(dataReader["Name"]);
                    customer.AddressLine1 = Convert.ToString(dataReader["AddressLine1"]);
                    customer.City = Convert.ToString(dataReader["City"]);
                    customer.State = Convert.ToString(dataReader["State"]);
                    customer.ZIPCode = Convert.ToString(dataReader["ZIPCode"]);
                    customer.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                    customer.TelephoneNumber = Convert.ToString(dataReader["TelephoneNumber"]);
                    customer.FaxNumber = Convert.ToString(dataReader["FaxNumber"]);
                    customer.EmailAddress = Convert.ToString(dataReader["EmailAddress"]);
                    customer.TruckType = Convert.ToString(dataReader["TruckType"]);
                    customer.CollectFreight = Convert.ToString(dataReader["CollectFreight"]);
                    customer.Comments = Convert.ToString(dataReader["Comments"]);
                    customer.Surcharge = Convert.ToDecimal(dataReader["Surcharge"]);
                    customer.FOB = Convert.ToString(dataReader["FOB"]);
                    customer.Terms = Convert.ToString(dataReader["Terms"]);
                    customer.RePackingCharge = Convert.ToDecimal(dataReader["RePackingCharge"]);
                    customer.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    customer.Invoicingtypeid = Convert.ToInt32(dataReader["Invoicingtypeid"]);
                    customer.EndCustomerName = Convert.ToString(dataReader["EndCustomerName"]);
                    customer.DisplayLineNo = Convert.ToBoolean(dataReader["DisplayLineNo"]);                    

                    customers.Add(customer);
                }
                conn.Close();
            }
            foreach (Customer customer in customers)
            {
                List<CustomerShippingInfo> lstTerms = new List<CustomerShippingInfo>();
                commandText = string.Format($"SELECT [id] ,[CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode] ,[IsDefault] FROM [dbo].[customershippinginfo] WITH(NOLOCK)  WHERE customerid ='{customer.Id}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader1.Read())
                    {
                        var terms = new CustomerShippingInfo();
                        terms.Id = Convert.ToInt32(dataReader1["Id"]);                        
                        terms.Name = Convert.ToString(dataReader1["Name"]);
                        terms.ContactPersonName = Convert.ToString(dataReader1["ContactPersonName"]);
                        terms.AddressLine1 = Convert.ToString(dataReader1["AddressLine1"]);
                        terms.City = Convert.ToString(dataReader1["City"]);
                        terms.State = Convert.ToString(dataReader1["State"]);
                        terms.ZIPCode = Convert.ToString(dataReader1["ZIPCode"]);
                        terms.IsDefault = Convert.ToBoolean(dataReader1["IsDefault"]);
                        
                        lstTerms.Add(terms);
                    }
                }
                customer.ShippingInfos = lstTerms;
                conn.Close();
            }


            return customers;
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            Customer customer = new Customer();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = string.Format($"SELECT [id],[CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber]" +
                ",[EmailAddress],[TruckType] ,[CollectFreight],[Comments] ,[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia] ,[invoicingtypeid],[endcustomername]" +
                $",[DisplayLineNo] FROM[customer] WITH(NOLOCK) WHERE [id] = '{id}'");
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    customer.Id = Convert.ToInt32(dataReader["Id"]);
                    customer.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    customer.Name = Convert.ToString(dataReader["Name"]);
                    customer.AddressLine1 = Convert.ToString(dataReader["AddressLine1"]);
                    customer.City = Convert.ToString(dataReader["City"]);
                    customer.State = Convert.ToString(dataReader["State"]);
                    customer.ZIPCode = Convert.ToString(dataReader["ZIPCode"]);
                    customer.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                    customer.TelephoneNumber = Convert.ToString(dataReader["TelephoneNumber"]);
                    customer.FaxNumber = Convert.ToString(dataReader["FaxNumber"]);
                    customer.EmailAddress = Convert.ToString(dataReader["EmailAddress"]);
                    customer.TruckType = Convert.ToString(dataReader["TruckType"]);
                    customer.CollectFreight = Convert.ToString(dataReader["CollectFreight"]);
                    customer.Comments = Convert.ToString(dataReader["Comments"]);
                    customer.Surcharge = Convert.ToDecimal(dataReader["Surcharge"]);
                    customer.FOB = Convert.ToString(dataReader["FOB"]);
                    customer.Terms = Convert.ToString(dataReader["Terms"]);
                    customer.RePackingCharge = Convert.ToDecimal(dataReader["RePackingCharge"]);
                    customer.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    customer.Invoicingtypeid = Convert.ToInt32(dataReader["Invoicingtypeid"]);
                    customer.EndCustomerName = Convert.ToString(dataReader["EndCustomerName"]);
                    customer.DisplayLineNo = Convert.ToBoolean(dataReader["DisplayLineNo"]);
                }
                conn.Close();
            }

            List<CustomerShippingInfo> lstTerms = new List<CustomerShippingInfo>();
            commandText = string.Format($"SELECT [id] ,[CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode] ,[IsDefault] FROM [dbo].[customershippinginfo] WITH(NOLOCK)  WHERE customerid ='{customer.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var terms = new CustomerShippingInfo();
                    terms.Id = Convert.ToInt32(dataReader1["Id"]);                    
                    terms.Name = Convert.ToString(dataReader1["Name"]);
                    terms.ContactPersonName = Convert.ToString(dataReader1["ContactPersonName"]);
                    terms.AddressLine1 = Convert.ToString(dataReader1["AddressLine1"]);
                    terms.City = Convert.ToString(dataReader1["City"]);
                    terms.State = Convert.ToString(dataReader1["State"]);
                    terms.ZIPCode = Convert.ToString(dataReader1["ZIPCode"]);
                    terms.IsDefault = Convert.ToBoolean(dataReader1["IsDefault"]);

                    lstTerms.Add(terms);
                }
            }
            customer.ShippingInfos = lstTerms;
            conn.Close();



            return customer;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            string sql = string.Format($"INSERT INTO [dbo].[customer] ([CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber],[EmailAddress],[TruckType],[CollectFreight],[Comments],[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia],[invoicingtypeid],[endcustomername],[DisplayLineNo])  VALUES ('{customer.CompanyId}', " +
                $"'{customer.Name}', '{customer.AddressLine1}', '{customer.City}', '{customer.State}', '{customer.ZIPCode}', '{customer.ContactPersonName}', '{customer.TelephoneNumber}',     " +
                $"'{customer.FaxNumber}', '{customer.EmailAddress}', '{customer.TruckType}', '{customer.CollectFreight}', " +
                $"'{customer.Comments}', '{customer.Surcharge}', '{customer.FOB}', '{customer.Terms}',  '{customer.RePackingCharge}', " +
                $"'{customer.ShipVia}', '{customer.Invoicingtypeid}', '{customer.EndCustomerName}','{ customer.DisplayLineNo}')");

            sql = sql + " Select Scope_Identity()";


            var customerId = _sqlHelper.ExecuteScalar(ConnectionSettings.ConnectionString, sql, CommandType.Text);


            foreach (CustomerShippingInfo term in customer.ShippingInfos)
            {
                sql = string.Format($"INSERT INTO [dbo].[customershippinginfo] ([CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode],[IsDefault]) VALUES ( " +
                    $"'{customerId}' , '{term.Name}' , '{term.ContactPersonName}' ,'{term.AddressLine1}', '{term.City}' , '{term.State}' , '{term.ZIPCode}' , '{term.IsDefault}')");

                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var sql = string.Format("DELETE FROM [dbo].[customershippinginfo]  WHERE customerid = '{0}'", customer.Id);

                _sqlHelper.ExecuteNonQuery(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                //sql = "UPDATE [dbo].[Customer] SET [CompanyId] = '{0}',[Name]= '{1}',[ContactPersonName]= '{2}',[PhoneNo]= '{3}',[EmailID]= '{4}',[Address]= '{5}',[City]= '{6}',[State]= '{7}',[Country]= '{8}',[ZIPCode]= '{9}',[FAXNo]= '{10}',[DateFormat]= '{11}',[noofstages]= '{12}',[CompanyProfileID] = '{13}'  WHERE id = '{14}'";

                sql = string.Format($"UPDATE [dbo].[customer]SET[CompanyId] = '{customer.CompanyId}' ,[Name] = '{customer.Name}',[AddressLine1] = '{customer.AddressLine1}'" +
                    $",[City] = '{customer.City}',[State] = '{customer.State}',[ZIPCode] = '{customer.ZIPCode}' ,[ContactPersonName] = '{customer.ContactPersonName}'" +
                    $"      ,[TelephoneNumber] = '{customer.TelephoneNumber}' ,[FaxNumber] = '{customer.FaxNumber}',[EmailAddress] = '{customer.EmailAddress}' ,[TruckType] = '{customer.TruckType}'" +
                    $"  ,[CollectFreight] = '{customer.CollectFreight}',[Comments] = '{customer.Comments}' ,[Surcharge] = '{customer.Surcharge}' ,[FOB] = '{customer.FOB}'" +
                    $" ,[Terms] = '{customer.Terms}',[RePackingCharge] = '{customer.RePackingCharge}',[ShipVia] = '{customer.ShipVia}' ,[invoicingtypeid] = '{customer.Invoicingtypeid}'" +
                    $"   ,[endcustomername] = '{customer.EndCustomerName}' ,[DisplayLineNo] = '{customer.DisplayLineNo}' WHERE[CompanyId] = '{customer.Id}' ");
                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                foreach (CustomerShippingInfo term in customer.ShippingInfos)
                {
                    sql = string.Format($"INSERT INTO [dbo].[customershippinginfo] ([CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode],[IsDefault]) VALUES ( " +
                    $"'{customer.Id}' , '{term.Name}' , '{term.ContactPersonName}' ,'{term.AddressLine1}', '{term.City}' , '{term.State}' , '{term.ZIPCode}' , '{term.IsDefault}')");
                    await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
            }
        }

        public async Task<int> DeleteCustomerAsync(long id)
        {
            string sql = string.Format("DELETE FROM [dbo].[Customer]  WHERE id = '{0}'", id);

            await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

            sql = string.Format("DELETE FROM [dbo].[customershippinginfo]  WHERE customerid = '{0}'", id);

            return await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
        }       
    }
}
