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
    public class ReportRepository : IReportRepository
    {
        private readonly ISqlHelper _sqlHelper;
        private readonly IOrderRepository _orderRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IPartRepository partRepository;

        public ReportRepository(ISqlHelper sqlHelper, IOrderRepository orderRepository,
            ICompanyRepository companyRepository,ICustomerRepository customerRepository,
            IPartRepository partRepository)
        {
            this._sqlHelper = sqlHelper;
            this._orderRepository = orderRepository;
            this.companyRepository = companyRepository;
            this.customerRepository = customerRepository;
            this.partRepository = partRepository;
        }

        public List<PackingSlipReport> GetPackingSlipReport(long id)
        {
            var packingSlip = new PackingSlip();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Empty;

            commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate]  FROM [dbo].[PackingSlipMaster] where Id = '{id}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    packingSlip.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlip.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    packingSlip.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    packingSlip.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    packingSlip.Boxes = Convert.ToInt32(dataReader["Boxes"]);
                    packingSlip.GrossWeight = Convert.ToDecimal(dataReader["GrossWeight"]);
                    packingSlip.ShippingCharge = Convert.ToDecimal(dataReader["ShippingCharge"]);
                    packingSlip.CustomCharge = Convert.ToDecimal(dataReader["CustomCharge"]);
                    packingSlip.SubTotal = Convert.ToDecimal(dataReader["SubTotal"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlip.IsInvoiceCreated = Convert.ToBoolean(dataReader["IsInvoiceCreated"]);
                    packingSlip.IsPaymentReceived = Convert.ToBoolean(dataReader["IsPaymentReceived"]);
                    packingSlip.FOB = Convert.ToString(dataReader["FOB"]);
                    packingSlip.Terms = Convert.ToString(dataReader["Terms"]);
                    packingSlip.ShipmentInfoId = Convert.ToInt32(dataReader["ShipmentInfoId"]);
                    packingSlip.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);                   
                    
                }
                conn.Close();
            }

            packingSlip.customerShippingInfo = new CustomerShippingInfo();
            commandText = string.Format($"SELECT  [id] ,[CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] " +
                $",[City] ,[State] ,[ZIPCode] ,[IsDefault]  FROM [customershippinginfo] where Id = '{packingSlip.ShipmentInfoId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    packingSlip.customerShippingInfo.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlip.customerShippingInfo.Name = Convert.ToString(dataReader["Name"]);
                    packingSlip.customerShippingInfo.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                    packingSlip.customerShippingInfo.AddressLine1 = Convert.ToString(dataReader["AddressLine1"]);
                    packingSlip.customerShippingInfo.City = Convert.ToString(dataReader["City"]);
                    packingSlip.customerShippingInfo.State = Convert.ToString(dataReader["State"]);
                    packingSlip.customerShippingInfo.ZIPCode = Convert.ToString(dataReader["ZIPCode"]);
                    packingSlip.customerShippingInfo.IsDefault = Convert.ToBoolean(dataReader["IsDefault"]);
                }
                conn.Close();
            }

            List<PackingSlipReport> packingSlipReports = new List<PackingSlipReport>();
            commandText = string.Format($"SELECT [Id] ,[PackingSlipId] ,[IsBlankOrder] ,[OrderNo] ,[OrderId] ,[OrderDetailId] ,[PartId] ,[Qty] ," +
                $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty],[LineNumber]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);
                int SrNo = 1;
                while (dataReader1.Read())
                {
                    var packingSlipReport = new PackingSlipReport();                    
                    packingSlipReport.OrderNo = Convert.ToString(dataReader1["OrderNo"]);
                    packingSlipReport.SrNo = SrNo;
                    packingSlipReport.PartId = Convert.ToInt32(dataReader1["PartId"]);
                    packingSlipReport.Qty = Convert.ToInt32(dataReader1["Qty"]);
                    packingSlipReport.Boxes = Convert.ToInt32(dataReader1["Boxes"]);
                    packingSlipReport.InBasket = Convert.ToBoolean(dataReader1["InBasket"]);
                    packingSlipReport.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                    packingSlipReport.Price = Convert.ToDecimal(dataReader1["Price"]);
                    packingSlipReport.Surcharge = Convert.ToDecimal(dataReader1["Surcharge"]);
                    packingSlipReport.SurchargePerPound = Convert.ToDecimal(dataReader1["SurchargePerPound"]);
                    packingSlipReport.SurchargePerUnit = Convert.ToDecimal(dataReader1["SurchargePerUnit"]);
                    packingSlipReport.TotalSurcharge = Convert.ToDecimal(dataReader1["TotalSurcharge"]);
                    packingSlipReport.ExcessQty = Convert.ToInt32(dataReader1["ExcessQty"]);
                    try
                    {
                        packingSlipReport.LineNumber = Convert.ToInt32(dataReader1["LineNumber"]);


                        if (packingSlipReport.LineNumber > 0)
                            packingSlipReport.OrderNo = packingSlipReport.OrderNo + "-" + packingSlipReport.LineNumber.ToString();
                    }
                    catch
                    {

                    }
                    SrNo++;
                    packingSlipReports.Add(packingSlipReport);
                }
            }            
            conn.Close();

            var company = companyRepository.GetCompany(packingSlip.CompanyId);
            var customer = customerRepository.GetCustomer(packingSlip.CustomerId);
            
            foreach(PackingSlipReport packingSlipReport in packingSlipReports)
            {                
                packingSlipReport.CompanyId = packingSlip.CompanyId;                
                packingSlipReport.PackingSlipNo = packingSlip.PackingSlipNo;
                packingSlipReport.ShippingDate = packingSlip.ShippingDate;
                packingSlipReport.ShipVia = packingSlip.ShipVia;
                packingSlipReport.Crates = packingSlip.Crates;
                packingSlipReport.Boxes = packingSlip.Boxes;
                packingSlipReport.GrossWeight = packingSlip.GrossWeight;
                packingSlipReport.ShippingCharge = packingSlip.ShippingCharge;
                packingSlipReport.CustomCharge = packingSlip.CustomCharge;
                packingSlipReport.SubTotal = packingSlip.SubTotal;
                packingSlipReport.Total = packingSlip.Total;
                packingSlipReport.IsInvoiceCreated = packingSlip.IsInvoiceCreated;
                packingSlipReport.IsPaymentReceived = packingSlip.IsPaymentReceived;
                packingSlipReport.FOB = packingSlip.FOB;
                packingSlipReport.Terms = packingSlip.Terms;
                packingSlipReport.ShipmentInfoId = packingSlip.ShipmentInfoId;
                packingSlipReport.InvoiceDate = packingSlip.InvoiceDate;

                packingSlipReport.CompanyId = company.Id;
                packingSlipReport.CompanyName = company.Name;
                packingSlipReport.CompanyAddress = company.Address;
                packingSlipReport.CompanyPhoneNo = company.PhoneNo;
                packingSlipReport.CompanyFaxNo = company.FaxNo;
                packingSlipReport.CompanyEMail = company.EMail;
                packingSlipReport.CompanyContactPersonName = company.ContactPersonName;
                packingSlipReport.CompanyWHName = company.WHName;
                packingSlipReport.CompanyWHAddress = company.WHAddress;
                packingSlipReport.CompanyWHPhoneNo = company.WHPhoneNo;
                packingSlipReport.CompanyWHEmail = company.WHEmail;
                
                packingSlipReport.CompanyId = customer.CompanyId;
                packingSlipReport.CustomerName = customer.Name;
                packingSlipReport.CustomerAddressLine1 = customer.AddressLine1;
                packingSlipReport.CustomerCity = customer.City;
                packingSlipReport.CustomerState = customer.State;
                packingSlipReport.CustomerZIPCode = customer.ZIPCode;
                packingSlipReport.CustomerContactPersonName = customer.ContactPersonName;
                packingSlipReport.CustomerTelephoneNumber = customer.TelephoneNumber;
                packingSlipReport.CustomerFaxNumber = customer.FaxNumber;
                packingSlipReport.CustomerEmailAddress = customer.EmailAddress;
                packingSlipReport.CustomerTruckType = customer.TruckType;
                packingSlipReport.CustomerCollectFreight = customer.CollectFreight;
                packingSlipReport.CustomerComments = customer.Comments;
                packingSlipReport.CustomerSurcharge = customer.Surcharge;
                packingSlipReport.CustomerFOB = customer.FOB;
                packingSlipReport.CustomerTerms = customer.Terms;
                packingSlipReport.CustomerRePackingCharge = customer.RePackingCharge;
                packingSlipReport.CustomerShipVia = customer.ShipVia;
                packingSlipReport.CustomerInvoicingtypeid = customer.Invoicingtypeid;
                packingSlipReport.CustomerEndCustomerName = customer.EndCustomerName;
                packingSlipReport.Billing = customer.Billing;

                
                packingSlipReport.ShipmentName = packingSlip.customerShippingInfo.Name;
                packingSlipReport.ShipmentContactPersonName = packingSlip.customerShippingInfo.ContactPersonName;
                packingSlipReport.ShipmentAddressLine1 = packingSlip.customerShippingInfo.AddressLine1;
                packingSlipReport.ShipmentCity = packingSlip.customerShippingInfo.City;
                packingSlipReport.ShipmentState = packingSlip.customerShippingInfo.State;
                packingSlipReport.ShipmentZIPCode = packingSlip.customerShippingInfo.ZIPCode;
                packingSlipReport.ShipmentIsDefault = packingSlip.customerShippingInfo.IsDefault;

                var part = partRepository.GetPart(packingSlipReport.PartId);
                packingSlipReport.PartCode = part.Code;
                packingSlipReport.PartDescription = part.Description;
            }

            return packingSlipReports;
        }

        public List<PackingSlipReport> GetMasterPackingSlipReport(long id)
        {
            var packingSlips = new List<PackingSlip>();
            var Crates = 0;
            var Boxes = 0;
            decimal GrossWeight = 0;
            decimal ShippingCharge = 0;
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Empty;

             commandText = string.Format($"SELECT PSM.[Id] ,PSM.[CompanyId] ,PSM.[CustomerId] ,[PackingSlipNo],[ShippingDate] ,[ShipVia] ,[Crates]," +
                       $"MPSM.MasterPackingSlipNo,[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ," +
                       $"[IsPaymentReceived] ,[FOB] ,[Terms] ,[ShipmentInfoId] ,[InvoiceDate]  FROM [dbo].[PackingSlipMaster] PSM INNER JOIN MasterPackingSlipMaster MPSM ON MPSM.ID = PSM.MasterPackingSlipId where MPSM.ID = '{id}' ");
            

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
               

                while (dataReader.Read())
                {
                    var packingSlip = new PackingSlip();
                    packingSlip.Id = Convert.ToInt32(dataReader["Id"]);
                    packingSlip.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    packingSlip.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    packingSlip.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    Crates = Crates + packingSlip.Crates;
                    packingSlip.Boxes = + Convert.ToInt32(dataReader["Boxes"]);
                    Boxes = Boxes + packingSlip.Boxes;
                    packingSlip.GrossWeight =  Convert.ToDecimal(dataReader["GrossWeight"]);
                    GrossWeight = GrossWeight + packingSlip.GrossWeight;
                    packingSlip.ShippingCharge = ShippingCharge + Convert.ToDecimal(dataReader["ShippingCharge"]);
                    ShippingCharge = ShippingCharge + packingSlip.ShippingCharge;
                    packingSlip.CustomCharge = Convert.ToDecimal(dataReader["CustomCharge"]);
                    packingSlip.SubTotal = Convert.ToDecimal(dataReader["SubTotal"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlip.IsInvoiceCreated = Convert.ToBoolean(dataReader["IsInvoiceCreated"]);
                    packingSlip.IsPaymentReceived = Convert.ToBoolean(dataReader["IsPaymentReceived"]);
                    packingSlip.FOB = Convert.ToString(dataReader["FOB"]);
                    packingSlip.Terms = Convert.ToString(dataReader["Terms"]);
                    packingSlip.ShipmentInfoId = Convert.ToInt32(dataReader["ShipmentInfoId"]);
                    packingSlip.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    packingSlip.MasterPackingSlipNo = Convert.ToString(dataReader["MasterPackingSlipNo"]);

                    packingSlips.Add(packingSlip);
                }
                conn.Close();
            }

            packingSlips.FirstOrDefault().customerShippingInfo = new CustomerShippingInfo();
            commandText = string.Format($"SELECT  [id] ,[CustomerID] ,[Name] ,[ContactPersonName] ,[AddressLine1] " +
                $",[City] ,[State] ,[ZIPCode] ,[IsDefault]  FROM [customershippinginfo] where Id = '{packingSlips.FirstOrDefault().ShipmentInfoId}' ");

            foreach (PackingSlip packingSlip in packingSlips)
            {
                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        packingSlip.customerShippingInfo = new CustomerShippingInfo();
                        packingSlip.customerShippingInfo.Id = Convert.ToInt32(dataReader["Id"]);
                        packingSlip.customerShippingInfo.Name = Convert.ToString(dataReader["Name"]);
                        packingSlip.customerShippingInfo.ContactPersonName = Convert.ToString(dataReader["ContactPersonName"]);
                        packingSlip.customerShippingInfo.AddressLine1 = Convert.ToString(dataReader["AddressLine1"]);
                        packingSlip.customerShippingInfo.City = Convert.ToString(dataReader["City"]);
                        packingSlip.customerShippingInfo.State = Convert.ToString(dataReader["State"]);
                        packingSlip.customerShippingInfo.ZIPCode = Convert.ToString(dataReader["ZIPCode"]);
                        packingSlip.customerShippingInfo.IsDefault = Convert.ToBoolean(dataReader["IsDefault"]);
                    }
                    conn.Close();
                }
            }

            List<PackingSlipReport> packingSlipReports = new List<PackingSlipReport>();
            foreach (PackingSlip packingSlip in packingSlips)
            {

                commandText = string.Format($"SELECT [Id] ,[PackingSlipId] ,[IsBlankOrder] ,[OrderNo] ,[OrderId] ,[OrderDetailId] ,[PartId] ,[Qty] ," +
                    $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty],[LineNumber]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}'");

                using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
                {
                    cmd1.CommandType = CommandType.Text;
                    conn.Open();
                    var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);
                    int SrNo = 1;
                    while (dataReader1.Read())
                    {
                        var packingSlipReport = new PackingSlipReport();
                        packingSlipReport.OrderNo = Convert.ToString(dataReader1["OrderNo"]);
                        packingSlipReport.SrNo = SrNo;
                        packingSlipReport.PartId = Convert.ToInt32(dataReader1["PartId"]);
                        packingSlipReport.Qty = Convert.ToInt32(dataReader1["Qty"]);
                        packingSlipReport.Boxes = Convert.ToInt32(dataReader1["Boxes"]);
                        packingSlipReport.InBasket = Convert.ToBoolean(dataReader1["InBasket"]);
                        packingSlipReport.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                        packingSlipReport.Price = Convert.ToDecimal(dataReader1["Price"]);
                        packingSlipReport.Surcharge = Convert.ToDecimal(dataReader1["Surcharge"]);
                        packingSlipReport.SurchargePerPound = Convert.ToDecimal(dataReader1["SurchargePerPound"]);
                        packingSlipReport.SurchargePerUnit = Convert.ToDecimal(dataReader1["SurchargePerUnit"]);
                        packingSlipReport.TotalSurcharge = Convert.ToDecimal(dataReader1["TotalSurcharge"]);
                        packingSlipReport.ExcessQty = Convert.ToInt32(dataReader1["ExcessQty"]);
                        try
                        {
                            packingSlipReport.LineNumber = Convert.ToInt32(dataReader1["LineNumber"]);


                            if (packingSlipReport.LineNumber > 0)
                                packingSlipReport.OrderNo = packingSlipReport.OrderNo + "-" + packingSlipReport.LineNumber.ToString();
                        }
                        catch
                        {

                        }

                        packingSlipReport.CompanyId = packingSlip.CompanyId;
                        packingSlipReport.PackingSlipNo = packingSlip.PackingSlipNo;
                        packingSlipReport.MasterPackingSlipNo = packingSlip.MasterPackingSlipNo;
                        packingSlipReport.ShippingDate = packingSlip.ShippingDate;
                        packingSlipReport.ShipVia = packingSlip.ShipVia;
                        packingSlipReport.Crates = Crates;
                        packingSlipReport.Boxes = Boxes;
                        packingSlipReport.GrossWeight = GrossWeight;
                        packingSlipReport.ShippingCharge = ShippingCharge;
                        packingSlipReport.CustomCharge = packingSlip.CustomCharge;
                        packingSlipReport.SubTotal = packingSlip.SubTotal;
                        packingSlipReport.Total = packingSlip.Total;
                        packingSlipReport.IsInvoiceCreated = packingSlip.IsInvoiceCreated;
                        packingSlipReport.IsPaymentReceived = packingSlip.IsPaymentReceived;
                        packingSlipReport.FOB = packingSlip.FOB;
                        packingSlipReport.Terms = packingSlip.Terms;
                        packingSlipReport.ShipmentInfoId = packingSlip.ShipmentInfoId;
                        packingSlipReport.InvoiceDate = packingSlip.InvoiceDate;
                        SrNo++;
                        packingSlipReports.Add(packingSlipReport);
                    }
                }
                conn.Close();
            }

            var company = companyRepository.GetCompany(packingSlips.FirstOrDefault().CompanyId);
            var customer = customerRepository.GetCustomer(packingSlips.FirstOrDefault().CustomerId);

            foreach (PackingSlipReport packingSlipReport in packingSlipReports)
            {                

                packingSlipReport.CompanyId = company.Id;
                packingSlipReport.CompanyName = company.Name;
                packingSlipReport.CompanyAddress = company.Address;
                packingSlipReport.CompanyPhoneNo = company.PhoneNo;
                packingSlipReport.CompanyFaxNo = company.FaxNo;
                packingSlipReport.CompanyEMail = company.EMail;
                packingSlipReport.CompanyContactPersonName = company.ContactPersonName;
                packingSlipReport.CompanyWHName = company.WHName;
                packingSlipReport.CompanyWHAddress = company.WHAddress;
                packingSlipReport.CompanyWHPhoneNo = company.WHPhoneNo;
                packingSlipReport.CompanyWHEmail = company.WHEmail;

                packingSlipReport.CompanyId = customer.CompanyId;
                packingSlipReport.CustomerName = customer.Name;
                packingSlipReport.CustomerAddressLine1 = customer.AddressLine1;
                packingSlipReport.CustomerCity = customer.City;
                packingSlipReport.CustomerState = customer.State;
                packingSlipReport.CustomerZIPCode = customer.ZIPCode;
                packingSlipReport.CustomerContactPersonName = customer.ContactPersonName;
                packingSlipReport.CustomerTelephoneNumber = customer.TelephoneNumber;
                packingSlipReport.CustomerFaxNumber = customer.FaxNumber;
                packingSlipReport.CustomerEmailAddress = customer.EmailAddress;
                packingSlipReport.CustomerTruckType = customer.TruckType;
                packingSlipReport.CustomerCollectFreight = customer.CollectFreight;
                packingSlipReport.CustomerComments = customer.Comments;
                packingSlipReport.CustomerSurcharge = customer.Surcharge;
                packingSlipReport.CustomerFOB = customer.FOB;
                packingSlipReport.CustomerTerms = customer.Terms;
                packingSlipReport.CustomerRePackingCharge = customer.RePackingCharge;
                packingSlipReport.CustomerShipVia = customer.ShipVia;
                packingSlipReport.CustomerInvoicingtypeid = customer.Invoicingtypeid;
                packingSlipReport.CustomerEndCustomerName = customer.EndCustomerName;
                packingSlipReport.Billing = customer.Billing;


                packingSlipReport.ShipmentName = packingSlips.FirstOrDefault().customerShippingInfo.Name;
                packingSlipReport.ShipmentContactPersonName = packingSlips.FirstOrDefault().customerShippingInfo.ContactPersonName;
                packingSlipReport.ShipmentAddressLine1 = packingSlips.FirstOrDefault().customerShippingInfo.AddressLine1;
                packingSlipReport.ShipmentCity = packingSlips.FirstOrDefault().customerShippingInfo.City;
                packingSlipReport.ShipmentState = packingSlips.FirstOrDefault().customerShippingInfo.State;
                packingSlipReport.ShipmentZIPCode = packingSlips.FirstOrDefault().customerShippingInfo.ZIPCode;
                packingSlipReport.ShipmentIsDefault = packingSlips.FirstOrDefault().customerShippingInfo.IsDefault;

                var part = partRepository.GetPart(packingSlipReport.PartId);
                packingSlipReport.PartCode = part.Code;
                packingSlipReport.PartDescription = part.Description;
            }

            return packingSlipReports;
        }
    }
}
