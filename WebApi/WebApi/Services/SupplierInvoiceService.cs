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

            
            //foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
            //{
            //    List<SupplierInvoicePoDetails> supplierInvoicePoDetails = new List<SupplierInvoicePoDetails>();

            //    var part = await _partRepository.GetPartByMapCodeAsync(supplierInvoice.SupplierId, supplierInvoiceDetail.PartCode);
            //    supplierInvoiceDetail.PartId = part.Id;

            //    //var adjustedQty = 0;
            //    var pos = await _poRepository.GetAllPosAsync(supplierInvoice.CompanyId);

            //    var openPos = pos.Where(x => x.IsClosed == false && x.SupplierId == supplierInvoice.SupplierId).OrderBy(x => x.PoDate);                

            //    var transactionDetail = new TransactionDetail();
            //    transactionDetail.PartId = supplierInvoiceDetail.PartId;
            //    transactionDetail.Qty = supplierInvoiceDetail.Qty;
            //    transactionDetail.TransactionTypeId = BusinessConstants.TRANSACTION_TYPE.UPLOAD_SUPPLIER_INVOICE;
            //    transactionDetail.TransactionDate = DateTime.Now;
            //    transactionDetail.DirectionId = BusinessConstants.DIRECTION.IN;
            //    transactionDetail.InventoryType = BusinessConstants.INVENTORY_TYPE.INTRANSIT_QTY;
            //    transactionDetail.ReferenceNo = supplierInvoice.InvoiceNo;

            //    await this._transactionRepository.AddTransactionAsync(transactionDetail);
            //}

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
                var pos = await _poRepository.GetAllPosAsync(supplierInvoice.CompanyId);

                var openPos = pos.Where(x => x.IsClosed == false && x.SupplierId == supplierInvoice.SupplierId).OrderBy(x => x.PoDate);

                foreach (Po po in pos)
                {
                    if (po.IsClosed == false && po.SupplierId == supplierInvoice.SupplierId)
                    {
                        foreach (PoDetail poDetail in po.poDetails)
                        {
                            if (poDetail.PartId == part.Id && poDetail.IsClosed == false && poDetail.AckQty > poDetail.InTransitQty + poDetail.ReceivedQty)
                            {                    
                                SupplierInvoicePoDetails supplierInvoicePoDetail = new SupplierInvoicePoDetails();
                                supplierInvoicePoDetail.PartId = poDetail.PartId;
                                supplierInvoicePoDetail.PoId = poDetail.PoId;
                                supplierInvoicePoDetail.PODetailId = poDetail.Id;
                                supplierInvoicePoDetail.PONo = po.PoNo;

                                var remainingPoQty = poDetail.AckQty - poDetail.InTransitQty + poDetail.ReceivedQty;

                                foreach (SupplierInvoiceDetail tmpSupplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                                {
                                    if (tmpSupplierInvoiceDetail.supplierInvoicePoDetails != null)
                                    {
                                        foreach (SupplierInvoicePoDetails tmpPoDetail in tmpSupplierInvoiceDetail.supplierInvoicePoDetails)
                                        {
                                            if (tmpPoDetail.PartId == poDetail.PartId && tmpPoDetail.PODetailId == poDetail.Id)
                                            {
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

                //var transactionDetail = new TransactionDetail();
                //transactionDetail.PartId = supplierInvoiceDetail.PartId;
                //transactionDetail.Qty = supplierInvoiceDetail.Qty;
                //transactionDetail.TransactionTypeId = BusinessConstants.TRANSACTION_TYPE.UPLOAD_SUPPLIER_INVOICE;
                //transactionDetail.TransactionDate = DateTime.Now;
                //transactionDetail.DirectionId = BusinessConstants.DIRECTION.IN;
                //transactionDetail.InventoryType = BusinessConstants.INVENTORY_TYPE.INTRANSIT_QTY;
                //transactionDetail.ReferenceNo = supplierInvoice.InvoiceNo;

                //await this._transactionRepository.AddTransactionAsync(transactionDetail);
            }

            return supplierInvoice;
        }

        public async Task<bool> DeleteSupplierInvoiceAsync(long supplierInvoiceId)
        {
            return await this._supplierInvoiceRepository.DeleteSupplierInvoiceAsync(supplierInvoiceId);
        }

        public async Task<IEnumerable<SupplierInvoice>> GetAllSupplierInvoicesAsync(int companyId)
        {
           var result =  await this._supplierInvoiceRepository.GetAllSupplierInvoicesAsync(companyId);
            var companyList = await this._companyRepository.GetAllCompanyAsync();
            var supplierList = await this._supplierRepository.GetAllSupplierAsync(companyId);
            var partList = await this._partRepository.GetAllPartsAsync(companyId);
            foreach (SupplierInvoice supplierInvoice in result)
            {
                supplierInvoice.CompanyDetail = companyList.Where(p => p.Id == supplierInvoice.CompanyId).FirstOrDefault();
                if(supplierInvoice !=null  && supplierInvoice.CompanyDetail !=null)
                    supplierInvoice.CompanyName = supplierInvoice.CompanyDetail.Name;
                supplierInvoice.SupplierDetail = supplierList.Where(p => p.Id == supplierInvoice.SupplierId).FirstOrDefault();
                if (supplierInvoice != null && supplierInvoice.SupplierDetail !=null)
                    supplierInvoice.SupplierName = supplierInvoice.SupplierDetail.Name;
                foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
                {
                    supplierInvoiceDetail.PartDetail = partList.Where(p => p.Id == supplierInvoiceDetail.PartId).FirstOrDefault(); //await this._partRepository.GetPartAsync(supplierInvoiceDetail.PartId);                    
                }
            }
            
            return result;
        }

        public async Task<SupplierInvoice> GetSupplierInvoiceAsync(long supplierInvoiceId)
        {
            var supplierInvoice = await this._supplierInvoiceRepository.GetSupplierInvoiceAsync(supplierInvoiceId);

            supplierInvoice.CompanyDetail = this._companyRepository.GetAllCompanyAsync().Result.Where(p => p.Id == supplierInvoice.CompanyId).FirstOrDefault();
            supplierInvoice.SupplierDetail = this._supplierRepository.GetAllSupplierAsync(supplierInvoice.CompanyId).Result.Where(p => p.Id == supplierInvoice.SupplierId).FirstOrDefault();
            var partList = await this._partRepository.GetAllPartsAsync(supplierInvoice.CompanyDetail.Id);
            foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
            {
                supplierInvoiceDetail.PartDetail = partList.Where(p => p.Id == supplierInvoiceDetail.PartId).FirstOrDefault(); 
            }

            return supplierInvoice;
        }

        public async Task ReceiveSupplierInvoiceAsync(long supplierInvoiceId)
        {
            await this._supplierInvoiceRepository.ReceiveSupplierInvoiceAsync(supplierInvoiceId);

            var supplierInvoice = await this._supplierInvoiceRepository.GetSupplierInvoiceAsync(supplierInvoiceId);

            foreach (SupplierInvoiceDetail supplierInvoiceDetail in supplierInvoice.supplierInvoiceDetails)
            {
                var transactionDetail = new TransactionDetail();
                transactionDetail.PartId = supplierInvoiceDetail.PartId;
                transactionDetail.Qty = supplierInvoiceDetail.Qty;
                transactionDetail.TransactionTypeId = BusinessConstants.TRANSACTION_TYPE.RECEIVE_SUPPLIER_INVOICE;
                transactionDetail.TransactionDate =  DateTime.Now;
                transactionDetail.DirectionId = BusinessConstants.DIRECTION.OUT;
                transactionDetail.InventoryType = BusinessConstants.INVENTORY_TYPE.INTRANSIT_QTY;
                await this._transactionRepository.AddTransactionAsync(transactionDetail);

                transactionDetail = new TransactionDetail();
                transactionDetail.PartId = supplierInvoiceDetail.PartId;
                transactionDetail.Qty = supplierInvoiceDetail.Qty;
                transactionDetail.TransactionTypeId = BusinessConstants.TRANSACTION_TYPE.RECEIVE_SUPPLIER_INVOICE;
                transactionDetail.TransactionDate = DateTime.Now;
                transactionDetail.DirectionId = BusinessConstants.DIRECTION.IN;
                transactionDetail.InventoryType = BusinessConstants.INVENTORY_TYPE.QTY_IN_HAND;
                await this._transactionRepository.AddTransactionAsync(transactionDetail);
            }
        }

        public async Task ReceiveBoxInvoiceAsync(string barcode)
        {
            await this._supplierInvoiceRepository.ReceiveBoxInvoiceAsync(barcode);            
        }

        public Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice)
        {
            throw new NotImplementedException();
        }

        public async Task UploadFileAsync(int id, string docType, string path)
        {
            await this._supplierInvoiceRepository.UploadFileAsync(id,docType,path);
        }
    }
}
