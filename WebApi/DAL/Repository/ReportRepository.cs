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
using System.Linq;

namespace DAL.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ISqlHelper _sqlHelper;
        private readonly IOrderRepository _orderRepository;
        private readonly ICompanyRepository companyRepository;
        private readonly ICustomerRepository customerRepository;
        private readonly IPartRepository partRepository;
        private readonly ISupplierRepository supplierRepository;

        public ReportRepository(ISqlHelper sqlHelper, IOrderRepository orderRepository,
            ICompanyRepository companyRepository, ICustomerRepository customerRepository,
            IPartRepository partRepository, ISupplierRepository supplierRepository)
        {
            this._sqlHelper = sqlHelper;
            this._orderRepository = orderRepository;
            this.companyRepository = companyRepository;
            this.customerRepository = customerRepository;
            this.partRepository = partRepository;
            this.supplierRepository = supplierRepository;
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
                    if (packingSlip.CompanyId == 2)
                        packingSlip.PackingSlipNo = "CF" + packingSlip.PackingSlipNo;
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    packingSlip.Boxes = Convert.ToInt32(dataReader["Boxes"]);
                    packingSlip.TotalBoxes = packingSlip.TotalBoxes + packingSlip.Boxes;
                    var grossWeight = 1.05 * Convert.ToDouble(dataReader["GrossWeight"]);
                    packingSlip.GrossWeight = Convert.ToDecimal(grossWeight);
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
                        packingSlipReport.LineNumber = Convert.ToString(dataReader1["LineNumber"]);


                        if (packingSlipReport.LineNumber == string.Empty || packingSlipReport.LineNumber == "" || packingSlipReport.LineNumber == null)
                            packingSlipReport.OrderNo = packingSlipReport.OrderNo;
                        else
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

            decimal totalRepackingSlipCost = 0;

            foreach (PackingSlipReport packingSlipReport in packingSlipReports)
            {
                packingSlipReport.CompanyId = packingSlip.CompanyId;
                packingSlipReport.PackingSlipNo = packingSlip.PackingSlipNo;
                packingSlipReport.ShippingDate = packingSlip.ShippingDate;
                packingSlipReport.ShipVia = packingSlip.ShipVia;
                packingSlipReport.Crates = packingSlip.Crates;
                packingSlipReport.TotalBoxes = packingSlip.TotalBoxes;
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

                packingSlipReport.TotalRePackingCharge = customer.RePackingCharge * packingSlipReport.Qty;
                totalRepackingSlipCost = totalRepackingSlipCost + packingSlipReport.TotalRePackingCharge;
                var part = partRepository.GetPart(packingSlipReport.PartId);
                packingSlipReport.PartCode = part.Code;
                packingSlipReport.PartDescription = part.Description;
                packingSlipReport.RePackingSlipNo = packingSlipReport.PackingSlipNo;
            }

            foreach (PackingSlipReport packingSlipReport in packingSlipReports)
            {
                packingSlipReport.SumRePackingCharge = totalRepackingSlipCost;
            }

            return packingSlipReports;
        }

        public List<PackingSlipReport> GetRepackingInvoiceReport(long id)
        {
            var packingSlip = new PackingSlip();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Empty;

            commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate]  FROM [dbo].[PackingSlipMaster] where Id = '{id}' and IsRepackage = 1 ");

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
                    if (packingSlip.CompanyId == 2)
                        packingSlip.PackingSlipNo = "CF" + packingSlip.PackingSlipNo;
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    packingSlip.Boxes = Convert.ToInt32(dataReader["Boxes"]);
                    var grossWeight = 1.05 * Convert.ToDouble(dataReader["GrossWeight"]);
                    packingSlip.GrossWeight = Convert.ToDecimal(grossWeight);
                    // packingSlip.GrossWeight = Convert.ToDecimal(dataReader["GrossWeight"]);
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
                $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty],[LineNumber]  FROM [dbo].[PackingSlipDetails] where PackingSlipId = '{ packingSlip.Id}' and IsRepackage = 1");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);
                int SrNo = 1;
                while (dataReader1.Read())
                {
                    var packingSlipReport = new PackingSlipReport();
                    packingSlipReport.OrderNo = "60049508";//Convert.ToString(dataReader1["OrderNo"]);
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
                    //try
                    //{
                    //    packingSlipReport.LineNumber = Convert.ToInt32(dataReader1["LineNumber"]);


                    //    if (packingSlipReport.LineNumber > 0)
                    //        packingSlipReport.OrderNo = packingSlipReport.OrderNo + "-" + packingSlipReport.LineNumber.ToString();
                    //}
                    //catch
                    //{

                    //}
                    SrNo++;
                    packingSlipReports.Add(packingSlipReport);
                }
            }
            conn.Close();
            var company = companyRepository.GetCompany(packingSlip.CompanyId);
            var customer = customerRepository.GetCustomer(packingSlip.CustomerId);

            decimal totalRepackingSlipCost = 0;

            foreach (PackingSlipReport packingSlipReport in packingSlipReports)
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
                if (customer.RePackingPoNo == null || customer.RePackingPoNo == string.Empty)
                    packingSlipReport.RePackingSlipNo = customer.RePackingPoNo;

                packingSlipReport.ShipmentName = packingSlip.customerShippingInfo.Name;
                packingSlipReport.ShipmentContactPersonName = packingSlip.customerShippingInfo.ContactPersonName;
                packingSlipReport.ShipmentAddressLine1 = packingSlip.customerShippingInfo.AddressLine1;
                packingSlipReport.ShipmentCity = packingSlip.customerShippingInfo.City;
                packingSlipReport.ShipmentState = packingSlip.customerShippingInfo.State;
                packingSlipReport.ShipmentZIPCode = packingSlip.customerShippingInfo.ZIPCode;
                packingSlipReport.ShipmentIsDefault = packingSlip.customerShippingInfo.IsDefault;

                packingSlipReport.TotalRePackingCharge = customer.RePackingCharge * packingSlipReport.Qty;
                totalRepackingSlipCost = totalRepackingSlipCost + packingSlipReport.TotalRePackingCharge;
                var part = partRepository.GetPart(packingSlipReport.PartId);
                packingSlipReport.PartCode = part.Code;
                packingSlipReport.PartDescription = part.Description;
                packingSlipReport.RePackingSlipNo = packingSlipReport.PackingSlipNo;

                packingSlipReport.RePackingSlipNo = packingSlipReport.RePackingSlipNo.Insert(4, "P");

            }

            foreach (PackingSlipReport packingSlipReport in packingSlipReports)
            {
                packingSlipReport.SumRePackingCharge = totalRepackingSlipCost;
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
                    if (packingSlip.CompanyId == 2)
                        packingSlip.PackingSlipNo = "CF" + packingSlip.PackingSlipNo;
                    packingSlip.CustomerId = Convert.ToInt32(dataReader["CustomerId"]);
                    packingSlip.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    Crates = Crates + packingSlip.Crates;
                    packingSlip.Boxes = Convert.ToInt32(dataReader["Boxes"]);
                    Boxes = Boxes + packingSlip.Boxes;
                    packingSlip.TotalBoxes = packingSlip.TotalBoxes + packingSlip.Boxes;
                    packingSlip.GrossWeight = Convert.ToDecimal(dataReader["GrossWeight"]);
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

            if (packingSlips.Count > 0)
            {
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
                            packingSlipReport.LineNumber = Convert.ToString(dataReader1["LineNumber"]);


                            if (packingSlipReport.LineNumber == string.Empty || packingSlipReport.LineNumber == "" || packingSlipReport.LineNumber == null)
                                packingSlipReport.OrderNo = packingSlipReport.OrderNo;
                            else
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
                        packingSlipReport.TotalBoxes = Boxes;
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

            return packingSlipReports.OrderBy(x => x.PackingSlipNo).ToList();
        }

        public List<POReport> GetPoReport(long poId)
        {
            var po = new Po();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[SupplierId] ,[PoNo] ,[PoDate] ,[EmailIds] ,[Remarks] ,[IsClosed] ,[ClosingDate] ,[IsAcknowledged] ,[AcknowledgementDate] ,[PaymentTerms] ,[DeliveryTerms],[DueDate],[AccessId]  FROM [dbo].[PoMaster] where Id = '{poId}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {

                    po.Id = Convert.ToInt64(dataReader["Id"]);
                    po.CompanyId = Convert.ToInt32(dataReader["CompanyId"]);
                    po.SupplierId = Convert.ToInt32(dataReader["SupplierId"]);
                    po.PoNo = Convert.ToString(dataReader["PoNo"]);
                    po.PoDate = Convert.ToDateTime(dataReader["PoDate"]);
                    po.EmailIds = Convert.ToString(dataReader["EmailIds"]);
                    po.Remarks = Convert.ToString(dataReader["Remarks"]);
                    po.IsClosed = Convert.ToBoolean(dataReader["IsClosed"]);
                    if (dataReader["ClosingDate"] != DBNull.Value)
                        po.ClosingDate = Convert.ToDateTime(dataReader["ClosingDate"]);
                    else
                        po.ClosingDate = null;

                    if (dataReader["DueDate"] != DBNull.Value)
                        po.DueDate = Convert.ToDateTime(dataReader["DueDate"]);
                    else
                        po.DueDate = null;


                    po.IsAcknowledged = Convert.ToBoolean(dataReader["IsAcknowledged"]);

                    if (dataReader["AcknowledgementDate"] != DBNull.Value)
                        po.AcknowledgementDate = Convert.ToDateTime(dataReader["AcknowledgementDate"]);
                    else
                        po.AcknowledgementDate = null;
                    po.PaymentTerms = Convert.ToString(dataReader["PaymentTerms"]);
                    po.DeliveryTerms = Convert.ToString(dataReader["DeliveryTerms"]);
                    po.AccessId = Convert.ToString(dataReader["AccessId"]);

                }
                dataReader.Close();
                conn.Close();
            }


            List<PoDetail> poDetails = new List<PoDetail>();
            commandText = string.Format($"SELECT [Id] ,[PoId] ,[PartId] ,[ReferenceNo] ,[Qty] ,[UnitPrice] ,[DueDate] ,[Note] ,[AckQty] ,[InTransitQty] ,[ReceivedQty] ,[IsClosed] ,[ClosingDate],[SrNo]  FROM [dbo].[PoDetails] where poid = '{ po.Id}'");

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var poDetail = new PoDetail();
                    poDetail.Id = Convert.ToInt64(dataReader1["Id"]);
                    poDetail.PoId = Convert.ToInt64(dataReader1["PoId"]);
                    poDetail.PartId = Convert.ToInt64(dataReader1["PartId"]);
                    poDetail.ReferenceNo = Convert.ToString(dataReader1["ReferenceNo"]);
                    poDetail.Qty = Convert.ToInt32(dataReader1["Qty"]);
                    poDetail.UnitPrice = Convert.ToDecimal(dataReader1["UnitPrice"]);
                    poDetail.DueDate = Convert.ToDateTime(dataReader1["DueDate"]);
                    poDetail.Note = Convert.ToString(dataReader1["Note"]);
                    poDetail.AckQty = Convert.ToInt32(dataReader1["AckQty"]);
                    poDetail.InTransitQty = Convert.ToInt32(dataReader1["InTransitQty"]);
                    poDetail.ReceivedQty = Convert.ToInt32(dataReader1["ReceivedQty"]);
                    poDetail.IsClosed = Convert.ToBoolean(dataReader1["IsClosed"]);
                    if (dataReader1["SrNo"] != DBNull.Value)
                        poDetail.SrNo = Convert.ToInt32(dataReader1["SrNo"]);
                    else
                        poDetail.SrNo = 0;

                    if (dataReader1["ClosingDate"] != DBNull.Value)
                        poDetail.ClosingDate = Convert.ToDateTime(dataReader1["ClosingDate"]);
                    else
                        poDetail.ClosingDate = null;

                    poDetails.Add(poDetail);
                }
                dataReader1.Close();
                conn.Close();
            }
            po.poDetails = poDetails;


            List<PoTerm> poTerms = new List<PoTerm>();
            commandText = string.Format("SELECT [Id] ,[PoId] ,[SequenceNo] ,[Term]  FROM [dbo].[PoTerms] where poid = '{0}'", po.Id);

            using (SqlCommand cmd1 = new SqlCommand(commandText, conn))
            {
                cmd1.CommandType = CommandType.Text;
                conn.Open();
                var dataReader1 = cmd1.ExecuteReader(CommandBehavior.CloseConnection);

                while (dataReader1.Read())
                {
                    var poTerm = new PoTerm();
                    poTerm.Id = Convert.ToInt64(dataReader1["Id"]);
                    poTerm.PoId = Convert.ToInt64(dataReader1["PoId"]);
                    poTerm.SequenceNo = Convert.ToInt32(dataReader1["SequenceNo"]);
                    poTerm.Term = Convert.ToString(dataReader1["Term"]);

                    poTerms.Add(poTerm);
                }
                dataReader1.Close();
                conn.Close();
            }
            po.poTerms = poTerms;

            var company = companyRepository.GetCompany(po.CompanyId);
            var supplier = supplierRepository.GetSupplier(po.SupplierId);

            List<POReport> pOReports = new List<POReport>();
            int srNo = 1;
            foreach (PoDetail poDetail in po.poDetails)
            {
                var poReport = new POReport();

                poReport.Id = po.Id;
                poReport.CompanyId = po.CompanyId;
                poReport.SupplierId = po.SupplierId;
                poReport.PoNo = po.PoNo;
                poReport.PoDate = po.PoDate;
                poReport.EmailIds = po.EmailIds;
                poReport.Remarks = po.Remarks;
                poReport.IsClosed = po.IsClosed;
                poReport.ClosingDate = po.ClosingDate;
                poReport.DueDate = po.DueDate;
                poReport.IsAcknowledged = po.IsAcknowledged;
                poReport.AcknowledgementDate = po.AcknowledgementDate;
                poReport.PaymentTerms = po.PaymentTerms;
                poReport.DeliveryTerms = po.DeliveryTerms;
                poReport.AccessId = po.AccessId;


                poReport.PoId = poDetail.PoId;
                poReport.PartId = poDetail.PartId;
                poReport.ReferenceNo = poDetail.ReferenceNo;
                poReport.Qty = poDetail.Qty;
                poReport.UnitPrice = poDetail.UnitPrice;
                poReport.PartDueDate = poDetail.DueDate;
                poReport.Note = poDetail.Note;
                poReport.AckQty = poDetail.AckQty;
                poReport.InTransitQty = poDetail.InTransitQty;
                poReport.ReceivedQty = poDetail.ReceivedQty;
                poReport.Amount = poDetail.Qty * poDetail.UnitPrice;
                poReport.TotalAmount = poReport.TotalAmount + poReport.Amount;
                if (poDetail.IsClosed)
                    poReport.Status = "Close";
                else
                    poReport.Status = "Open";
                poReport.SrNo = poDetail.SrNo;
                srNo++;

                var part = partRepository.GetPart(poDetail.PartId);
                poReport.PartCode = part.Code;
                poReport.PartDescription = part.Description;

                poReport.PoLetterHead = supplier.PoLetterHead;


                poReport.SupplierName = supplier.Name;
                poReport.SupplierContactPersonName = supplier.ContactPersonName;
                poReport.SupplierPhoneNo = supplier.PhoneNo;
                poReport.SupplierEmailID = supplier.EmailID;
                poReport.SupplierAddress = supplier.Address;
                poReport.SupplierCity = supplier.City;
                poReport.SupplierState = supplier.State;
                poReport.SupplierCountry = supplier.Country;
                poReport.SupplierZIPCode = supplier.ZIPCode;
                poReport.SupplierFAXNo = supplier.FAXNo;
                poReport.SupplierAddress = supplier.Address;
                poReport.SupplierPhoneNo = supplier.PhoneNo;


                poReport.CompanyId = company.Id;
                poReport.CompanyPhoneNo = company.PhoneNo;
                poReport.CompanyFaxNo = company.FaxNo;
                poReport.CompanyEMail = company.EMail;
                poReport.CompanyContactPersonName = company.ContactPersonName;
                poReport.CompanyWHName = company.WHName;
                poReport.CompanyWHAddress = company.WHAddress;
                poReport.CompanyWHPhoneNo = company.WHPhoneNo;
                poReport.CompanyWHEmail = company.WHEmail;
                if (poReport.PoLetterHead == 2)
                {

                    poReport.CompanyName = "Phloem LLC.";
                    poReport.CompanyAddress = "226 West 27th Street, Northampton, PA 18067 USA";
                }
                else
                {
                    poReport.CompanyName = company.Name;
                    poReport.CompanyAddress = company.Address;
                }

                foreach (PoTerm poTerm in po.poTerms.OrderBy(x => x.SequenceNo))
                {
                    if (poTerm.SequenceNo == 1)
                        poReport.TermsConditions = poTerm.SequenceNo.ToString() + ". " + poTerm.Term;
                    else
                        poReport.TermsConditions = poReport.TermsConditions + Environment.NewLine + poTerm.SequenceNo.ToString() + ". " + poTerm.Term;
                }

                pOReports.Add(poReport);
            }
            return pOReports.OrderBy(x => x.SrNo).ToList();
        }

        public async Task<IEnumerable<SalesData>> GetSalesDataAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            List<SalesData> packingSlips = new List<SalesData>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT CM.Name as CustomerName ,PM.Code,PM.Description,PSM.PackingSlipNo,PSM.ShippingDate ,PSD.[PackingSlipId],PSD.[PartId] ,Sum([Qty]) Qty,PSD.[UnitPrice],sum(PSD.[Price])  Price" +
                $" FROM [PackingSlipDetails] PSD  INNER JOIN PackingSlipMaster PSM ON PSM.ID = PSD.PackingSlipId    INNER JOIN part PM ON PM.id = PSD.PartId   INNER JOIN customer CM ON CM.id = PSM.CustomerId   " +
                $" WHERE PSM.ShippingDate BETWEEN '{fromDate}' AND '{toDate}' AND PSM.CompanyId='{companyId}'   " +
                $" GROUP BY PSD.[PackingSlipId],PSD.[PartId],PSD.[UnitPrice], PSM.PackingSlipNo ,PSM.ShippingDate,CM.Name,PM.Code,PM.Description ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new SalesData();

                    packingSlip.CustomerName = Convert.ToString(dataReader["CustomerName"]);
                    packingSlip.Code = Convert.ToString(dataReader["Code"]);
                    packingSlip.Description = Convert.ToString(dataReader["Description"]);
                    packingSlip.PackingSlipNo = Convert.ToString(dataReader["PackingSlipNo"]);
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.Qty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlip.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlip.UnitPrice = Convert.ToDecimal(dataReader["UnitPrice"]);
                    packingSlip.Price = Convert.ToDecimal(dataReader["Price"]);
                    packingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }

            foreach (SalesData salesData in packingSlips)
            {
                commandText = string.Format($"select Top 1 name,UnitPrice from supplier sm inner join partsupplierassignment psa on sm.id = psa.SupplierID where partid = '{salesData.PartId}'");

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        salesData.SupplierName = Convert.ToString(dataReader["name"]);
                        salesData.PurchasePrice = Convert.ToDecimal(dataReader["UnitPrice"]);
                        salesData.TotalPurchasePrice = salesData.PurchasePrice * salesData.Qty;
                    }
                    dataReader.Close();
                    conn.Close();
                }
            }

            return packingSlips;
        }

        public async Task<IEnumerable<SalesData>> GetSalesDataSummaryAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            List<SalesData> packingSlips = new List<SalesData>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT CM.Name as CustomerName ,PM.Code,PM.Description,PSD.[PartId] ,Sum([Qty]) Qty,PSD.[UnitPrice],sum(PSD.[Price]) [Price]" +
                $"FROM [PackingSlipDetails] PSD  INNER JOIN PackingSlipMaster PSM ON PSM.ID = PSD.PackingSlipId    INNER JOIN part PM ON PM.id = PSD.PartId   INNER JOIN customer CM ON CM.id = PSM.CustomerId   " +
                $"WHERE PSM.ShippingDate BETWEEN '{fromDate}' AND '{toDate}' AND PSM.CompanyId='{companyId}'   " +
                $" GROUP BY PSD.[PartId],PSD.[UnitPrice],CM.Name,PM.Code,PM.Description" +
                $" Order by PM.Code ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new SalesData();

                    packingSlip.CustomerName = Convert.ToString(dataReader["CustomerName"]);
                    packingSlip.Code = Convert.ToString(dataReader["Code"]);
                    packingSlip.Description = Convert.ToString(dataReader["Description"]);
                    packingSlip.Qty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlip.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlip.UnitPrice = Convert.ToDecimal(dataReader["UnitPrice"]);
                    packingSlip.Price = Convert.ToDecimal(dataReader["Price"]);
                    packingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }

            foreach (SalesData salesData in packingSlips)
            {
                commandText = string.Format($"select Top 1 name,UnitPrice from supplier sm inner join partsupplierassignment psa on sm.id = psa.SupplierID where partid = '{salesData.PartId}'");

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        salesData.SupplierName = Convert.ToString(dataReader["name"]);
                        salesData.PurchasePrice = Convert.ToDecimal(dataReader["UnitPrice"]);
                        salesData.TotalPurchasePrice = salesData.PurchasePrice * salesData.Qty;
                    }
                    dataReader.Close();
                    conn.Close();
                }
            }

            return packingSlips;
        }

        public async Task<IEnumerable<PurchaseData>> GetPurchaseDataAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            List<PurchaseData> packingSlips = new List<PurchaseData>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT SM.Name as SupplierName	,SIM.InvoiceNo ,SIM.InvoiceDate	,SIM.ReceivedDate ,SID.[InvoiceId]," +
                $"SID.[PartId] ,PM.Code ,PM.Description ,Sum(SID.[Qty]) as Qty ,SID.Price,Sum(SID.[Total]) as Total " +
                $"FROM [SupplierInvoiceDetails] SID  INNER JOIN SupplierInvoiceMaster SIM ON SIM.Id = SID.InvoiceId  " +
                $"INNER JOIN part PM ON PM.id = SID.PartId  INNER JOIN supplier SM ON SM.id = SIM.SupplierId  " +
                $"WHERE SIM.ReceivedDate BETWEEN '{fromDate}' AND '{toDate}' AND SIM.CompanyId = '{companyId}'  " +
                $"Group by SID.InvoiceId, SID.PartId,SID.Price,SIM.InvoiceNo,SIM.InvoiceDate ,SIM.ReceivedDate,PM.Code ,PM.Description,SM.Name ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new PurchaseData();

                    packingSlip.SupplierName = Convert.ToString(dataReader["SupplierName"]);

                    packingSlip.InvoiceNo = Convert.ToString(dataReader["InvoiceNo"]);
                    packingSlip.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    packingSlip.ReceivedDate = Convert.ToDateTime(dataReader["ReceivedDate"]);
                    packingSlip.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlip.Code = Convert.ToString(dataReader["Code"]);
                    packingSlip.Description = Convert.ToString(dataReader["Description"]);

                    packingSlip.Qty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlip.Price = Convert.ToDecimal(dataReader["Price"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }


            commandText = string.Format($"SELECT SM.Name as SupplierName	,SIM.InvoiceNo ,SIM.InvoiceDate	,SIM.UploadedDate ,SID.[InvoiceId], " +
                 $"SID.[PartId], PM.Code, PM.Description, Sum(SID.[Qty]) as Qty, SID.Price, Sum(SID.[Total]) as Total " +
                 $"FROM[SupplierInvoiceDetails] SID  INNER JOIN SupplierInvoiceMaster SIM ON SIM.Id = SID.InvoiceId " +
                 $"INNER JOIN part PM ON PM.id = SID.PartId  INNER JOIN supplier SM ON SM.id = SIM.SupplierId " +
                 $"WHERE SIM.UploadedDate BETWEEN '{fromDate}' AND '{toDate}' AND SIM.CompanyId = '{companyId}' AND SIM.IsInvoiceUploaded = '1' AND SIM.IsInvoiceReceived = '0' " +
                 $"Group by SID.InvoiceId, SID.PartId, SID.Price, SIM.InvoiceNo, SIM.InvoiceDate, SIM.UploadedDate, PM.Code, PM.Description, SM.Name ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new PurchaseData();

                    packingSlip.SupplierName = Convert.ToString(dataReader["SupplierName"]);

                    packingSlip.InvoiceNo = Convert.ToString(dataReader["InvoiceNo"]);
                    packingSlip.InvoiceDate = Convert.ToDateTime(dataReader["InvoiceDate"]);
                    packingSlip.ReceivedDate = Convert.ToDateTime(dataReader["UploadedDate"]);
                    packingSlip.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlip.Code = Convert.ToString(dataReader["Code"]);
                    packingSlip.Description = Convert.ToString(dataReader["Description"]);
                    packingSlip.InTransitQty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlip.Price = Convert.ToDecimal(dataReader["Price"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }


            foreach (PurchaseData purchaseData in packingSlips)
            {
                commandText = string.Format($"select Top 1 name from customer cm inner join partcustomerassignment pca on cm.id = pca.CustomerId where partid = '{purchaseData.PartId}'");

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        purchaseData.CustomerName = Convert.ToString(dataReader["name"]);
                    }
                    dataReader.Close();
                    conn.Close();
                }
            }

            return packingSlips;
        }

        public async Task<IEnumerable<PurchaseData>> GetPurchaseDataSummaryAsync(int companyId, DateTime fromDate, DateTime toDate)
        {
            List<PurchaseData> packingSlips = new List<PurchaseData>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);


            var commandText = string.Format($"SELECT SM.Name as SupplierName , SID.[PartId] ,PM.Code ,PM.Description ,Sum(SID.[Qty]) as Qty ,SID.Price,Sum(SID.[Total]) as Total " +
                $"FROM [SupplierInvoiceDetails] SID  INNER JOIN SupplierInvoiceMaster SIM ON SIM.Id = SID.InvoiceId  " +
                $"INNER JOIN part PM ON PM.id = SID.PartId  INNER JOIN supplier SM ON SM.id = SIM.SupplierId  " +                
                $"WHERE SIM.UploadedDate BETWEEN '{fromDate}' AND '{toDate}' AND SIM.CompanyId = '{companyId}' AND SIM.IsInvoiceUploaded = '1' AND SIM.IsInvoiceReceived = '0' " +
                $"Group by SID.PartId,SID.Price,PM.Code ,PM.Description,SM.Name ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new PurchaseData();

                    packingSlip.SupplierName = Convert.ToString(dataReader["SupplierName"]);
                    packingSlip.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlip.Code = Convert.ToString(dataReader["Code"]);
                    packingSlip.Description = Convert.ToString(dataReader["Description"]);
                    packingSlip.Qty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlip.Price = Convert.ToDecimal(dataReader["Price"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }


            commandText = string.Format($"SELECT SM.Name as SupplierName , SID.[PartId] ,PM.Code ,PM.Description ,Sum(SID.[Qty]) as Qty ,SID.Price,Sum(SID.[Total]) as Total " +
                $"FROM [SupplierInvoiceDetails] SID  INNER JOIN SupplierInvoiceMaster SIM ON SIM.Id = SID.InvoiceId  " +
                $"INNER JOIN part PM ON PM.id = SID.PartId  INNER JOIN supplier SM ON SM.id = SIM.SupplierId  " +
                $"WHERE SIM.ReceivedDate BETWEEN '{fromDate}' AND '{toDate}' AND SIM.CompanyId = '{companyId}'  " +
                $"Group by SID.PartId,SID.Price,PM.Code ,PM.Description,SM.Name ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new PurchaseData();

                    packingSlip.SupplierName = Convert.ToString(dataReader["SupplierName"]);
                    packingSlip.PartId = Convert.ToInt32(dataReader["PartId"]);
                    packingSlip.Code = Convert.ToString(dataReader["Code"]);
                    packingSlip.Description = Convert.ToString(dataReader["Description"]);
                    packingSlip.InTransitQty = Convert.ToInt32(dataReader["Qty"]);
                    packingSlip.Price = Convert.ToDecimal(dataReader["Price"]);
                    packingSlip.Total = Convert.ToDecimal(dataReader["Total"]);
                    packingSlips.Add(packingSlip);
                }
                dataReader.Close();
                conn.Close();
            }


            foreach (PurchaseData purchaseData in packingSlips)
            {
                commandText = string.Format($"select Top 1 name from customer cm inner join partcustomerassignment pca on cm.id = pca.CustomerId where partid = '{purchaseData.PartId}'");

                using (SqlCommand cmd = new SqlCommand(commandText, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    conn.Open();

                    var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (dataReader.Read())
                    {
                        purchaseData.CustomerName = Convert.ToString(dataReader["name"]);
                    }
                    dataReader.Close();
                    conn.Close();
                }
            }

            return packingSlips;
        }

        public List<PackingSlipReport> GetMonthlyInvoiceReport(long id)
        {
            var packingSlip = new PackingSlip();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Empty;

            commandText = string.Format($"SELECT [Id] ,[CompanyId] ,[CustomerId] ,[PackingSlipNo] ,[ShippingDate] ,[ShipVia] ,[Crates] ," +
                $"[Boxes] ,[GrossWeight] ,[ShippingCharge] ,[CustomCharge] ,[SubTotal] ,[Total] ,[IsInvoiceCreated] ,[IsPaymentReceived] ,[FOB] ,[Terms] ," +
                $"[ShipmentInfoId] ,[InvoiceDate]  FROM [dbo].[InvoiceMaster] where Id = '{id}' ");

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
                    if (packingSlip.CompanyId == 2)
                        packingSlip.PackingSlipNo = "CF" + packingSlip.PackingSlipNo;
                    packingSlip.ShippingDate = Convert.ToDateTime(dataReader["ShippingDate"]);
                    packingSlip.ShipVia = Convert.ToString(dataReader["ShipVia"]);
                    packingSlip.Crates = Convert.ToInt32(dataReader["Crates"]);
                    packingSlip.Boxes = Convert.ToInt32(dataReader["Boxes"]);
                    var grossWeight = 1.05 * Convert.ToDouble(dataReader["GrossWeight"]);
                    packingSlip.GrossWeight = Convert.ToDecimal(grossWeight);
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
                $"[Boxes] ,[InBasket] ,[UnitPrice] ,[Price] ,[Surcharge] ,[SurchargePerPound] ,[SurchargePerUnit] ,[TotalSurcharge] ,[ExcessQty],[LineNumber]  FROM [dbo].[InvoiceDetails] where PackingSlipId = '{ packingSlip.Id}'");

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
                        packingSlipReport.LineNumber = Convert.ToString(dataReader1["LineNumber"]);


                        if (packingSlipReport.LineNumber == string.Empty || packingSlipReport.LineNumber == "" || packingSlipReport.LineNumber == null)
                            packingSlipReport.OrderNo = packingSlipReport.OrderNo;
                        else
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

            decimal totalRepackingSlipCost = 0;

            foreach (PackingSlipReport packingSlipReport in packingSlipReports)
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

                packingSlipReport.TotalRePackingCharge = customer.RePackingCharge * packingSlipReport.Qty;
                totalRepackingSlipCost = totalRepackingSlipCost + packingSlipReport.TotalRePackingCharge;
                var part = partRepository.GetPart(packingSlipReport.PartId);
                packingSlipReport.PartCode = part.Code;
                packingSlipReport.PartDescription = part.Description;
                packingSlipReport.RePackingSlipNo = packingSlipReport.PackingSlipNo;
            }

            foreach (PackingSlipReport packingSlipReport in packingSlipReports)
            {
                packingSlipReport.SumRePackingCharge = totalRepackingSlipCost;
            }

            return packingSlipReports;
        }

        public async Task<IEnumerable<TransactionDetail>> GetInventoryAdjustmentReport(int companyId,DateTime from,DateTime to)
        {
            var transactionDetails = new List<TransactionDetail>();
            SqlConnection conn = new SqlConnection(ConnectionSettings.ConnectionString);

            var commandText = string.Empty;

            commandText = string.Format($"SELECT PM.Code, PM.Description,[PartId],[TransactionTypeId],[TransactionDate],[DirectionTypeId],[InventoryTypeId],[ReferenceNo],[Qty] FROM[dbo].[TransactionDetails] TD  INNER JOIN part PM ON PM.ID = TD.PartId  where[TransactionTypeId] in (4, 5) AND PM.CompanyId = '{companyId}'   AND [TransactionDate] between '{from}' and '{to}' ");

            using (SqlCommand cmd = new SqlCommand(commandText, conn))
            {
                cmd.CommandType = CommandType.Text;

                conn.Open();

                var dataReader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (dataReader.Read())
                {
                    var packingSlip = new TransactionDetail();

                    packingSlip.Code = Convert.ToString(dataReader["Code"]);
                    packingSlip.Description = Convert.ToString(dataReader["Description"]);
                    packingSlip.PartId = Convert.ToInt32(dataReader["PartId"]);
                    if (Convert.ToInt32(dataReader["DirectionTypeId"]) == 1)
                        packingSlip.DirectionType = "IN";
                    else
                        packingSlip.DirectionType = "OUT";
                    
                    packingSlip.TransactionDate = Convert.ToDateTime(dataReader["TransactionDate"]);
                    packingSlip.ReferenceNo = Convert.ToString(dataReader["ReferenceNo"]);                   

                    transactionDetails.Add(packingSlip);

                }
                conn.Close();
            }
            

            return transactionDetails.OrderByDescending(x=>x.TransactionDate).ToList();
        }
    }
}
