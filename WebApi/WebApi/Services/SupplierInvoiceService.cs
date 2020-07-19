using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using WebApi.IServices;

namespace WebApi.Services
{
    public class SupplierInvoiceService : ISupplierInvoiceService
    {
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPartRepository _partRepository;
        private readonly IPoRepository _poRepository;
        private readonly ITransactionRepository _transactionRepository;

        public SupplierInvoiceService(ISupplierInvoiceRepository supplierInvoiceRepository,
            ICompanyRepository companyRepository, ISupplierRepository supplierRepository,
            IPartRepository partRepository, IPoRepository poRepository, ITransactionRepository transactionRepository)
        {
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _companyRepository = companyRepository;
            _supplierRepository = supplierRepository;
            _partRepository = partRepository;
            _poRepository = poRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<SupplierInvoice> AddSupplierInvoiceAsync(SupplierInvoice supplierInvoice)
        {
            var company = await _companyRepository.GetCompanyByNameAsync(supplierInvoice.CompanyName);
            supplierInvoice.CompanyId = company.Id;

            var supplier = await _supplierRepository.GetSupplierByNameAsync(supplierInvoice.CompanyId, supplierInvoice.SupplierName);
            supplierInvoice.SupplierId = supplier.Id;          

            supplierInvoice.Id= await _supplierInvoiceRepository.AddSupplierInvoiceAsync(supplierInvoice);
            return supplierInvoice;
        }

        public async Task<SupplierInvoice> GetSupplierInvoicePODetailAsync(SupplierInvoice supplierInvoice)
        {
            var company = await _companyRepository.GetCompanyByNameAsync(supplierInvoice.CompanyName);
            supplierInvoice.CompanyId = company.Id;

            var supplier = await _supplierRepository.GetSupplierByNameAsync(supplierInvoice.CompanyId, supplierInvoice.SupplierName);
            supplierInvoice.SupplierId = supplier.Id;

            int srNo = 1;
            foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
            {
                List<SupplierInvoicePoDetails> supplierInvoicePoDetails = new List<SupplierInvoicePoDetails>();

                var part = await _partRepository.GetPartByMapCodeAsync(supplierInvoice.SupplierId, supplierInvoiceDetail.PartCode);
                supplierInvoiceDetail.PartId = part.Id;
                supplierInvoiceDetail.SrNo = srNo;
                srNo++;
                var adjustedQty = 0;
                IEnumerable<Po> pos = new List<Po>();

                if(!(supplierInvoice.DontImpactPO))
                    pos = await _poRepository.GetAllPosAsync(supplierInvoice.CompanyId,1);
                pos = pos.OrderBy(x => x.DueDate);
                //List<string> supplierPos = supplierInvoice.PoNo.Split(',').ToList();
                foreach (Po po in pos)
                {
                    if (po.IsClosed == false && po.SupplierId == supplierInvoice.SupplierId)
                    {
                        foreach (PoDetail poDetail in po.poDetails.OrderBy(x => x.DueDate))
                        {
                            if (poDetail.PartId == part.Id && poDetail.IsClosed == false && poDetail.AckQty > poDetail.InTransitQty + poDetail.ReceivedQty)
                            {                    
                                SupplierInvoicePoDetails supplierInvoicePoDetail = new SupplierInvoicePoDetails();
                                supplierInvoicePoDetail.PartId = poDetail.PartId;
                                supplierInvoicePoDetail.PoId = poDetail.PoId;
                                supplierInvoicePoDetail.PODetailId = poDetail.Id;
                                supplierInvoicePoDetail.PONo = po.PoNo;
                                supplierInvoicePoDetail.UnitPrice = poDetail.UnitPrice;

                                var remainingPoQty = poDetail.AckQty - (poDetail.InTransitQty + poDetail.ReceivedQty);

                                foreach (SupplierInvoiceDetail tmpSupplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                                {
                                    if (tmpSupplierInvoiceDetail.supplierInvoicePoDetails != null)
                                    {
                                        foreach (SupplierInvoicePoDetails tmpPoDetail in tmpSupplierInvoiceDetail.supplierInvoicePoDetails)
                                        {
                                            if (tmpPoDetail.PartId == poDetail.PartId && tmpPoDetail.PODetailId == poDetail.Id)
                                            {
                                                if(remainingPoQty - tmpPoDetail.Qty >= 0)
                                                    remainingPoQty = remainingPoQty - tmpPoDetail.Qty;                                               

                                            }
                                        }
                                    }
                                }

                                //case 1 we found the podetail where ackqty is adjusted for all invoiceqty
                                //case 2 we have to adjusted invoiceqty in different pos
                                //case 3 we have invoiceqty greater than available poqty

                                //case 1 we found the podetail where ackqty is adjusted for all invoiceqty
                                if (remainingPoQty > 0)
                                {
                                    if (remainingPoQty >= supplierInvoiceDetail.Qty - adjustedQty)
                                    {
                                        supplierInvoicePoDetail.Qty = supplierInvoiceDetail.Qty - adjustedQty;
                                        adjustedQty = adjustedQty + supplierInvoicePoDetail.Qty;
                                    }

                                    //case 2 we have to adjusted invoiceqty in different pos
                                    if (remainingPoQty < supplierInvoiceDetail.Qty - adjustedQty)
                                    {
                                        supplierInvoicePoDetail.Qty = remainingPoQty;
                                        adjustedQty = adjustedQty + remainingPoQty;
                                    }

                                    supplierInvoicePoDetails.Add(supplierInvoicePoDetail);
                                    if (adjustedQty == supplierInvoiceDetail.Qty)
                                        break;
                                }
                            }
                        }
                    }
                    if (adjustedQty == supplierInvoiceDetail.Qty)
                        break;
                }
                supplierInvoiceDetail.AdjustedPOQty = adjustedQty;
                supplierInvoiceDetail.ExcessQty = supplierInvoiceDetail.Qty - adjustedQty;
                supplierInvoiceDetail.supplierInvoicePoDetails = supplierInvoicePoDetails;               
            }

            return supplierInvoice;
        }

        public async Task<bool> DeleteSupplierInvoiceAsync(long supplierInvoiceId)
        {
            return await this._supplierInvoiceRepository.DeleteSupplierInvoiceAsync(supplierInvoiceId);
        }

        public async Task<IEnumerable<SupplierInvoice>> GetAllSupplierInvoicesAsync(int companyId,int userId)
        {
           var result =  await this._supplierInvoiceRepository.GetAllSupplierInvoicesAsync(companyId,userId);
            var companyList = await this._companyRepository.GetAllCompanyAsync();
            var supplierList = await this._supplierRepository.GetAllSupplierAsync(companyId,userId);
            var partList = await this._partRepository.GetAllPartsAsync(companyId,userId);

            

            foreach (SupplierInvoice supplierInvoice in result)
            {
                List<SupplierInvoiceGroupDetail> supplierInvoiceGroupDetails = new List<SupplierInvoiceGroupDetail>();
                supplierInvoice.CompanyDetail = companyList.Where(p => p.Id == supplierInvoice.CompanyId).FirstOrDefault();
                if(supplierInvoice !=null  && supplierInvoice.CompanyDetail !=null)
                    supplierInvoice.CompanyName = supplierInvoice.CompanyDetail.Name;
                supplierInvoice.SupplierDetail = supplierList.Where(p => p.Id == supplierInvoice.SupplierId).FirstOrDefault();
                if (supplierInvoice != null && supplierInvoice.SupplierDetail !=null)
                    supplierInvoice.SupplierName = supplierInvoice.SupplierDetail.Name;
                foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                {
                    supplierInvoiceDetail.PartDetail = partList.Where(p => p.Id == supplierInvoiceDetail.PartId).FirstOrDefault(); //await this._partRepository.GetPartAsync(supplierInvoiceDetail.PartId);

                    var supplierInvoiceGroupDetail = supplierInvoiceGroupDetails.Where(x => x.InvoiceId == supplierInvoiceDetail.InvoiceId && x.PartId == supplierInvoiceDetail.PartId && x.Price == supplierInvoiceDetail.Price).FirstOrDefault();
                    if (supplierInvoiceGroupDetail == null)
                    {
                        //Id ,InvoiceId ,SrNo,PartId,PartCode, Qty, Price,Total,AdjustedPOQty,ExcessQty,BoxNo, Barcode ,IsBoxReceived,IsOpen,
                        //AdjustedInvoiceQty,OpenQty,PoNo,PoQty, PartDetail , supplierInvoicePoDetails
                        supplierInvoiceGroupDetail = new SupplierInvoiceGroupDetail();
                        supplierInvoiceGroupDetail.Id = supplierInvoiceDetail.Id.ToString();
                        supplierInvoiceGroupDetail.InvoiceId = supplierInvoiceDetail.InvoiceId;
                        supplierInvoiceGroupDetail.SrNo = supplierInvoiceDetail.SrNo.ToString();
                        supplierInvoiceGroupDetail.PartId = supplierInvoiceDetail.PartId;
                        supplierInvoiceGroupDetail.PartCode = supplierInvoiceDetail.PartCode;
                        supplierInvoiceGroupDetail.Qty = supplierInvoiceDetail.Qty;
                        supplierInvoiceGroupDetail.Price = supplierInvoiceDetail.Price;
                        supplierInvoiceGroupDetail.Total = supplierInvoiceDetail.Total;
                        supplierInvoiceGroupDetail.ExcessQty = supplierInvoiceDetail.ExcessQty;
                        supplierInvoiceGroupDetail.BoxNo = supplierInvoiceDetail.BoxNo.ToString();
                        supplierInvoiceGroupDetail.Barcode = supplierInvoiceDetail.Barcode;
                        supplierInvoiceGroupDetail.IsOpen = supplierInvoiceDetail.IsOpen;
                        supplierInvoiceGroupDetail.AdjustedInvoiceQty = supplierInvoiceDetail.AdjustedInvoiceQty;
                        supplierInvoiceGroupDetail.OpenQty = supplierInvoiceDetail.OpenQty;
                        foreach (SupplierInvoicePoDetails supplierInvoicePoDetails in supplierInvoiceDetail.supplierInvoicePoDetails)
                        {
                            supplierInvoiceGroupDetail.PONo = supplierInvoiceGroupDetail.PONo + "," + supplierInvoicePoDetails.PONo;
                            supplierInvoiceGroupDetail.AdjustedPOQty = supplierInvoiceGroupDetail.AdjustedPOQty + "," + supplierInvoicePoDetails.Qty.ToString();
                            supplierInvoiceGroupDetail.AdjustedPOPrice = supplierInvoiceGroupDetail.AdjustedPOPrice + "," + supplierInvoicePoDetails.UnitPrice.ToString();
                        }
                        supplierInvoiceGroupDetail.PartDetail = supplierInvoiceDetail.PartDetail;
                        supplierInvoiceGroupDetails.Add(supplierInvoiceGroupDetail);
                    }
                    else
                    {
                        supplierInvoiceGroupDetail.Id = supplierInvoiceGroupDetail.Id + "," + supplierInvoiceDetail.Id.ToString();
                        supplierInvoiceGroupDetail.SrNo = supplierInvoiceGroupDetail.SrNo + "," + supplierInvoiceDetail.SrNo.ToString();

                        supplierInvoiceGroupDetail.Qty = supplierInvoiceGroupDetail.Qty + supplierInvoiceDetail.Qty;
                        supplierInvoiceGroupDetail.Total = supplierInvoiceGroupDetail.Total + supplierInvoiceDetail.Total;
                        supplierInvoiceGroupDetail.ExcessQty = supplierInvoiceGroupDetail.ExcessQty + supplierInvoiceDetail.ExcessQty;
                        supplierInvoiceGroupDetail.BoxNo = supplierInvoiceGroupDetail.BoxNo + "," + supplierInvoiceDetail.BoxNo.ToString();
                        supplierInvoiceGroupDetail.Barcode = supplierInvoiceGroupDetail.Barcode + "," + supplierInvoiceDetail.Barcode;
                        supplierInvoiceGroupDetail.AdjustedInvoiceQty = supplierInvoiceGroupDetail.AdjustedInvoiceQty + supplierInvoiceDetail.AdjustedInvoiceQty;
                        supplierInvoiceGroupDetail.OpenQty = supplierInvoiceGroupDetail.OpenQty + supplierInvoiceDetail.OpenQty;
                        foreach (SupplierInvoicePoDetails supplierInvoicePoDetails in supplierInvoiceDetail.supplierInvoicePoDetails)
                        {
                            supplierInvoiceGroupDetail.PONo = supplierInvoiceGroupDetail.PONo + "," + supplierInvoicePoDetails.PONo;
                            supplierInvoiceGroupDetail.AdjustedPOQty = supplierInvoiceGroupDetail.AdjustedPOQty + "," + supplierInvoicePoDetails.Qty.ToString();
                            supplierInvoiceGroupDetail.AdjustedPOPrice = supplierInvoiceGroupDetail.AdjustedPOPrice + "," + supplierInvoicePoDetails.UnitPrice.ToString();
                        }
                        supplierInvoiceGroupDetail.PartDetail = supplierInvoiceDetail.PartDetail;
                    }

                }
                supplierInvoice.supplierInvoiceGroupDetails = supplierInvoiceGroupDetails;
            }
            result = result.OrderByDescending(x => x.InvoiceDate);
            return result;
        }

        public async Task<IEnumerable<SupplierInvoice>> GetIntransitSupplierInvoicesAsync(int companyId)
        {
            var result = await this._supplierInvoiceRepository.GetIntransitSupplierInvoicesAsync(companyId);
            var companyList = await this._companyRepository.GetAllCompanyAsync();
            var supplierList = await this._supplierRepository.GetAllSupplierAsync(companyId,1);
            var partList = await this._partRepository.GetAllPartsAsync(companyId,1);
            foreach (SupplierInvoice supplierInvoice in result)
            {
                supplierInvoice.CompanyDetail = companyList.Where(p => p.Id == supplierInvoice.CompanyId).FirstOrDefault();
                if (supplierInvoice != null && supplierInvoice.CompanyDetail != null)
                    supplierInvoice.CompanyName = supplierInvoice.CompanyDetail.Name;
                supplierInvoice.SupplierDetail = supplierList.Where(p => p.Id == supplierInvoice.SupplierId).FirstOrDefault();
                if (supplierInvoice != null && supplierInvoice.SupplierDetail != null)
                    supplierInvoice.SupplierName = supplierInvoice.SupplierDetail.Name;
                foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                {
                    supplierInvoiceDetail.PartDetail = partList.Where(p => p.Id == supplierInvoiceDetail.PartId).FirstOrDefault(); //await this._partRepository.GetPartAsync(supplierInvoiceDetail.PartId);                    
                }
            }

            return result;
        }


        public async Task<IEnumerable<SupplierIntransitInvoice>> GetIntransitSupplierInvoicesByPartIdAsync(int companyId, int partId)
        {
            var result = await this._supplierInvoiceRepository.GetIntransitSupplierInvoicesByPartIdAsync(companyId,partId);
            var companyList = await this._companyRepository.GetAllCompanyAsync();
            var supplierList = await this._supplierRepository.GetAllSupplierAsync(companyId,1);
            //var partList = await this._partRepository.GetAllPartsAsync(companyId);

            List<SupplierIntransitInvoice> supplierIntransitInvoices = new List<SupplierIntransitInvoice>();
            foreach (SupplierInvoice supplierInvoice in result)
            {
                SupplierIntransitInvoice supplierIntransitInvoice = new SupplierIntransitInvoice();
                supplierInvoice.CompanyDetail = companyList.Where(p => p.Id == supplierInvoice.CompanyId).FirstOrDefault();
                if (supplierInvoice != null && supplierInvoice.CompanyDetail != null)
                    supplierInvoice.CompanyName = supplierInvoice.CompanyDetail.Name;
                supplierInvoice.SupplierDetail = supplierList.Where(p => p.Id == supplierInvoice.SupplierId).FirstOrDefault();
                if (supplierInvoice != null && supplierInvoice.SupplierDetail != null)
                    supplierInvoice.SupplierName = supplierInvoice.SupplierDetail.Name;

                foreach(SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                {
                    supplierIntransitInvoice.Id = supplierInvoice.Id;
                    supplierIntransitInvoice.CompanyId = supplierInvoice.CompanyId;
                    supplierIntransitInvoice.CompanyName = supplierInvoice.CompanyName;
                    supplierIntransitInvoice.SupplierId = supplierInvoice.SupplierId;
                    supplierIntransitInvoice.SupplierName = supplierInvoice.SupplierName;
                    supplierIntransitInvoice.InvoiceNo = supplierInvoice.InvoiceNo;
                    supplierIntransitInvoice.InvoiceDate = supplierInvoice.InvoiceDate;
                    supplierIntransitInvoice.ETA = supplierInvoice.ETA;
                    supplierIntransitInvoice.IsAirShipment = supplierInvoice.IsAirShipment;
                    supplierIntransitInvoice.PoNo = supplierInvoice.PoNo;
                    supplierIntransitInvoice.ReferenceNo = supplierInvoice.ReferenceNo;
                    supplierIntransitInvoice.ByCourier = supplierInvoice.ByCourier;
                    supplierIntransitInvoice.UploadedDate = supplierInvoice.UploadedDate;
                    supplierIntransitInvoice.ReceivedDate = supplierInvoice.ReceivedDate;


                    supplierIntransitInvoice.InvoiceDetailId = supplierInvoiceDetail.Id;
                    supplierIntransitInvoice.SrNo = supplierInvoiceDetail.SrNo;
                    supplierIntransitInvoice.PartId = supplierInvoiceDetail.PartId;
                    supplierIntransitInvoice.PartCode = supplierInvoiceDetail.PartCode;
                    supplierIntransitInvoice.Qty = supplierInvoiceDetail.Qty;
                    supplierIntransitInvoice.BoxNo = supplierInvoiceDetail.BoxNo;                  

                    supplierIntransitInvoices.Add(supplierIntransitInvoice);
                }               
            }

            return supplierIntransitInvoices;
        }

        public async Task<IEnumerable<SupplierOpenInvoice>> GetOpenSupplierInvoicesByPartIdAsync(int companyId, int partId)
        {
            var result = await this._supplierInvoiceRepository.GetOpenSupplierInvoicesByPartIdAsync(companyId, partId);           

            return result;
        }

        public async Task<SupplierInvoice> GetSupplierInvoiceAsync(long supplierInvoiceId)
        {
            var supplierInvoice = await this._supplierInvoiceRepository.GetSupplierInvoiceAsync(supplierInvoiceId);

            supplierInvoice.CompanyDetail = this._companyRepository.GetAllCompanyAsync().Result.Where(p => p.Id == supplierInvoice.CompanyId).FirstOrDefault();
            supplierInvoice.SupplierDetail = this._supplierRepository.GetAllSupplierAsync(supplierInvoice.CompanyId,1).Result.Where(p => p.Id == supplierInvoice.SupplierId).FirstOrDefault();
            var partList = await this._partRepository.GetAllPartsAsync(supplierInvoice.CompanyDetail.Id,1);
            foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
            {
                supplierInvoiceDetail.PartDetail = partList.Where(p => p.Id == supplierInvoiceDetail.PartId).FirstOrDefault();
            }

            return supplierInvoice;
        }

        public async Task<SupplierInvoice> GetSupplierInvoiceAsync(string invoiceNo)
        {
            var supplierInvoice = await this._supplierInvoiceRepository.GetSupplierInvoiceAsync(invoiceNo);            

            return supplierInvoice;
        }

        public async Task ReceiveSupplierInvoiceAsync(long supplierInvoiceId)
        {
            await this._supplierInvoiceRepository.ReceiveSupplierInvoiceAsync(supplierInvoiceId);                     
        }

        public async Task UnReceiveSupplierInvoiceAsync(long supplierInvoiceId)
        {
            await this._supplierInvoiceRepository.UnReceiveSupplierInvoiceAsync(supplierInvoiceId);
        }

        public async Task ReceiveBoxInvoiceAsync(string barcode)
        {
            await this._supplierInvoiceRepository.ReceiveBoxInvoiceAsync(barcode);            
        }

        public async Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice)
        {
            await this._supplierInvoiceRepository.UpdateSupplierInvoiceAsync(supplierInvoice);
        }

        public async Task UploadFileAsync(int id, string docType, string path)
        {
            await this._supplierInvoiceRepository.UploadFileAsync(id,docType,path);
        }
    }
}
