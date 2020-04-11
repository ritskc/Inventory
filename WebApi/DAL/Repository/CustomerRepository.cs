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
        private readonly IUserRepository userRepository;

        public CustomerRepository(ISqlHelper sqlHelper, IUserRepository userRepository)
        {
            _sqlHelper = sqlHelper;
            this.userRepository = userRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync(int companyId, int userId)
        {            
            List<Customer> customers = new List<Customer>();
            var commandText = "";
            var userInfo = await userRepository.GeUserbyIdAsync(userId);
            if (userInfo.UserTypeId == 1)
            {
                commandText = string.Format($"SELECT [id],[CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber]" +
               ",[EmailAddress],[TruckType] ,[CollectFreight],[Comments] ,[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia] ,[invoicingtypeid],[endcustomername]" +
               $",[DisplayLineNo],[Billing] FROM[customer] WITH(NOLOCK) WHERE[CompanyId] = '{companyId}'");
            }
            if (userInfo.UserTypeId == 2)
            {
                string companylist = string.Join(",", userInfo.CompanyIds);
                commandText = string.Format($"SELECT [id],[CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber]" +
               ",[EmailAddress],[TruckType] ,[CollectFreight],[Comments] ,[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia] ,[invoicingtypeid],[endcustomername]" +
               $",[DisplayLineNo],[Billing] FROM[customer] WITH(NOLOCK) WHERE[CompanyId] = '{companyId}' and  [id] in ({companylist})");
            }
            if (userInfo.UserTypeId == 3)
            {
                return customers;
            }
            
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
           
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
                    customer.Billing = Convert.ToString(dataReader["Billing"]);

                    customers.Add(customer);
                }
                conn.Close();
            }
            foreach (Customer customer in customers)
            {
                List<CustomerShippingInfo> lstTerms = new List<CustomerShippingInfo>();
                commandText = string.Format($"SELECT [id] ,[CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode] ,[IsDefault] FROM [dbo].[customershippinginfo] WITH(NOLOCK)  WHERE customerid ='{customer.Id}' and IsOld = 0");

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


            return customers.OrderBy(x => x.Name);
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            Customer customer = new Customer();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = string.Format($"SELECT [id],[CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber]" +
                ",[EmailAddress],[TruckType] ,[CollectFreight],[Comments] ,[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia] ,[invoicingtypeid],[endcustomername]" +
                $",[DisplayLineNo],[Billing] FROM[customer] WITH(NOLOCK) WHERE [id] = '{id}'");
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
                    customer.Billing = Convert.ToString(dataReader["Billing"]);
                }
                conn.Close();
            }

            List<CustomerShippingInfo> lstTerms = new List<CustomerShippingInfo>();
            commandText = string.Format($"SELECT [id] ,[CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode] ,[IsDefault] FROM [dbo].[customershippinginfo] WITH(NOLOCK)  WHERE customerid ='{customer.Id}'  and IsOld = 0");

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

        public async Task<Customer> GetCustomerAsync(int id, SqlConnection conn, SqlTransaction transaction)
        {
            Customer customer = new Customer();
            
            var commandText = string.Format($"SELECT [id],[CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber]" +
                ",[EmailAddress],[TruckType] ,[CollectFreight],[Comments] ,[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia] ,[invoicingtypeid],[endcustomername]" +
                $",[DisplayLineNo],[Billing] FROM[customer] WITH(NOLOCK) WHERE [id] = '{id}'");
            using (SqlCommand cmd = new SqlCommand(commandText, conn, transaction))
            {
                cmd.CommandType = CommandType.Text;               

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.Default);

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
                    customer.Billing = Convert.ToString(dataReader["Billing"]);
                }
                dataReader.Close();
            }

            List<CustomerShippingInfo> lstTerms = new List<CustomerShippingInfo>();
            commandText = string.Format($"SELECT [id] ,[CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode] ,[IsDefault] FROM [dbo].[customershippinginfo] WITH(NOLOCK)  WHERE customerid ='{customer.Id}'  and IsOld = 0");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn, transaction))
            {
                cmd1.CommandType = CommandType.Text;
                
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.Default);

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
                dataReader1.Close();
            }
            customer.ShippingInfos = lstTerms;
            return customer;
        }

        public Customer GetCustomer(int id)
        {
            Customer customer = new Customer();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);
            var commandText = string.Format($"SELECT [id],[CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber]" +
                ",[EmailAddress],[TruckType] ,[CollectFreight],[Comments] ,[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia] ,[invoicingtypeid],[endcustomername]" +
                $",[DisplayLineNo],[Billing] FROM[customer] WITH(NOLOCK) WHERE [id] = '{id}'");
            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

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
                    customer.Billing = Convert.ToString(dataReader["Billing"]);
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
            if (customer.Name == null)
                customer.Name = string.Empty;
            if (customer.AddressLine1 == null)
                customer.AddressLine1 = string.Empty;
            if (customer.City == null)
                customer.City = string.Empty;
            if (customer.State == null)
                customer.State = string.Empty;
            if (customer.ZIPCode == null)
                customer.ZIPCode = string.Empty;
            if (customer.ContactPersonName == null)
                customer.ContactPersonName = string.Empty;
            if (customer.TelephoneNumber == null)
                customer.TelephoneNumber = string.Empty;
            if (customer.FaxNumber == null)
                customer.FaxNumber = string.Empty;
            if (customer.EmailAddress == null)
                customer.EmailAddress = string.Empty;
            if (customer.TruckType == null)
                customer.TruckType = string.Empty;
            if (customer.CollectFreight == null)
                customer.CollectFreight = string.Empty;
            if (customer.Comments == null)
                customer.Comments = string.Empty;
            if (customer.FOB == null)
                customer.FOB = string.Empty;
            if (customer.Terms == null)
                customer.Terms = string.Empty;
            if (customer.ShipVia == null)
                customer.ShipVia = string.Empty;
            if (customer.EndCustomerName == null)
                customer.EndCustomerName = string.Empty;
            if (customer.Billing == null)
                customer.Billing = string.Empty;
           

            string sql = string.Format($"INSERT INTO [dbo].[customer] ([CompanyId],[Name],[AddressLine1],[City],[State],[ZIPCode],[ContactPersonName],[TelephoneNumber],[FaxNumber],[EmailAddress],[TruckType],[CollectFreight],[Comments],[Surcharge],[FOB],[Terms],[RePackingCharge],[ShipVia],[invoicingtypeid],[endcustomername],[DisplayLineNo],[Billing])  VALUES ('{customer.CompanyId}', " +
                $"'{customer.Name.Replace("'", "''")}', '{customer.AddressLine1.Replace("'", "''")}', '{customer.City.Replace("'", "''")}', '{customer.State.Replace("'", "''")}', '{customer.ZIPCode.Replace("'", "''")}', '{customer.ContactPersonName.Replace("'", "''")}', '{customer.TelephoneNumber.Replace("'", "''")}',     " +
                $"'{customer.FaxNumber.Replace("'", "''")}', '{customer.EmailAddress.Replace("'", "''")}', '{customer.TruckType.Replace("'", "''")}', '{customer.CollectFreight.Replace("'", "''")}', " +
                $"'{customer.Comments.Replace("'", "''")}', '{customer.Surcharge}', '{customer.FOB.Replace("'", "''")}', '{customer.Terms.Replace("'", "''")}',  '{customer.RePackingCharge}', " +
                $"'{customer.ShipVia.Replace("'", "''")}', '{customer.Invoicingtypeid}', '{customer.EndCustomerName.Replace("'", "''")}','{ customer.DisplayLineNo}','{ customer.Billing.Replace("'", "''")}')");

            sql = sql + " Select Scope_Identity()";


            var customerId = _sqlHelper.ExecuteScalar(ConnectionSettings.ConnectionString, sql, CommandType.Text);


            foreach (CustomerShippingInfo term in customer.ShippingInfos)
            {
                if (term.Name == null)
                    term.Name = string.Empty;
                if (term.ContactPersonName == null)
                    term.ContactPersonName = string.Empty;
                if (term.AddressLine1 == null)
                    term.AddressLine1 = string.Empty;
                if (term.City == null)
                    term.City = string.Empty;
                if (term.State == null)
                    term.State = string.Empty;
                if (term.ZIPCode == null)
                    term.ZIPCode = string.Empty;
               

                sql = string.Format($"INSERT INTO [dbo].[customershippinginfo] ([CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode],[IsDefault],[IsOld]) VALUES ( " +
                    $"'{customerId}' , '{term.Name.Replace("'", "''")}' , '{term.ContactPersonName.Replace("'", "''")}' ,'{term.AddressLine1.Replace("'", "''")}', '{term.City.Replace("'", "''")}' , '{term.State.Replace("'", "''")}' , '{term.ZIPCode.Replace("'", "''")}' , '{term.IsDefault}','{false}')");

                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var sql = string.Format("UPDATE customershippinginfo SET IsOld = 1  WHERE customerid = '{0}'", customer.Id);

                _sqlHelper.ExecuteNonQuery(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                //sql = "UPDATE [dbo].[Customer] SET [CompanyId] = '{0}',[Name]= '{1}',[ContactPersonName]= '{2}',[PhoneNo]= '{3}',[EmailID]= '{4}',[Address]= '{5}',[City]= '{6}',[State]= '{7}',[Country]= '{8}',[ZIPCode]= '{9}',[FAXNo]= '{10}',[DateFormat]= '{11}',[noofstages]= '{12}',[CompanyProfileID] = '{13}'  WHERE id = '{14}'";

                if (customer.Name == null)
                    customer.Name = string.Empty;
                if (customer.AddressLine1 == null)
                    customer.AddressLine1 = string.Empty;
                if (customer.City == null)
                    customer.City = string.Empty;
                if (customer.State == null)
                    customer.State = string.Empty;
                if (customer.ZIPCode == null)
                    customer.ZIPCode = string.Empty;
                if (customer.ContactPersonName == null)
                    customer.ContactPersonName = string.Empty;
                if (customer.TelephoneNumber == null)
                    customer.TelephoneNumber = string.Empty;
                if (customer.FaxNumber == null)
                    customer.FaxNumber = string.Empty;
                if (customer.EmailAddress == null)
                    customer.EmailAddress = string.Empty;
                if (customer.TruckType == null)
                    customer.TruckType = string.Empty;
                if (customer.CollectFreight == null)
                    customer.CollectFreight = string.Empty;
                if (customer.Comments == null)
                    customer.Comments = string.Empty;
                if (customer.FOB == null)
                    customer.FOB = string.Empty;
                if (customer.Terms == null)
                    customer.Terms = string.Empty;
                if (customer.ShipVia == null)
                    customer.ShipVia = string.Empty;
                if (customer.EndCustomerName == null)
                    customer.EndCustomerName = string.Empty;
                if (customer.Billing == null)
                    customer.Billing = string.Empty;

                sql = string.Format($"UPDATE [dbo].[customer]SET[CompanyId] = '{customer.CompanyId}' ,[Name] = '{customer.Name.Replace("'", "''")}',[AddressLine1] = '{customer.AddressLine1.Replace("'", "''")}'" +
                    $",[City] = '{customer.City.Replace("'", "''")}',[State] = '{customer.State.Replace("'", "''")}',[ZIPCode] = '{customer.ZIPCode.Replace("'", "''")}' ,[ContactPersonName] = '{customer.ContactPersonName.Replace("'", "''")}'" +
                    $"      ,[TelephoneNumber] = '{customer.TelephoneNumber}' ,[FaxNumber] = '{customer.FaxNumber}',[EmailAddress] = '{customer.EmailAddress}' ,[TruckType] = '{customer.TruckType}'" +
                    $"  ,[CollectFreight] = '{customer.CollectFreight}',[Comments] = '{customer.Comments.Replace("'", "''")}' ,[Surcharge] = '{customer.Surcharge}' ,[FOB] = '{customer.FOB}'" +
                    $" ,[Terms] = '{customer.Terms}',[RePackingCharge] = '{customer.RePackingCharge}',[ShipVia] = '{customer.ShipVia.Replace("'", "''")}' ,[invoicingtypeid] = '{customer.Invoicingtypeid}'" +
                    $"   ,[endcustomername] = '{customer.EndCustomerName.Replace("'", "''")}' ,[DisplayLineNo] = '{customer.DisplayLineNo}',[Billing] = '{customer.Billing.Replace("'","''")}' WHERE[Id] = '{customer.Id}' ");
                await _sqlHelper.ExecuteNonQueryAsync(ConnectionSettings.ConnectionString, sql, CommandType.Text);

                foreach (CustomerShippingInfo term in customer.ShippingInfos)
                {
                    if (term.Name == null)
                        term.Name = string.Empty;
                    if (term.ContactPersonName == null)
                        term.ContactPersonName = string.Empty;
                    if (term.AddressLine1 == null)
                        term.AddressLine1 = string.Empty;
                    if (term.City == null)
                        term.City = string.Empty;
                    if (term.State == null)
                        term.State = string.Empty;
                    if (term.ZIPCode == null)
                        term.ZIPCode = string.Empty;


                    sql = string.Format($"INSERT INTO [dbo].[customershippinginfo] ([CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] ,[City] ,[State],[ZIPCode],[IsDefault],[IsOld]) VALUES ( " +
                        $"'{customer.Id}' , '{term.Name.Replace("'", "''")}' , '{term.ContactPersonName.Replace("'", "''")}' ,'{term.AddressLine1.Replace("'", "''")}', '{term.City.Replace("'", "''")}' , '{term.State.Replace("'", "''")}' , '{term.ZIPCode.Replace("'", "''")}' , '{term.IsDefault}', '{false}')");

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
