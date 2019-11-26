using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface ISupplierInvoiceService
    {
        Task<IEnumerable<SupplierInvoice>> GetAllSupplierInvoicesAsync(int companyId);
        Task<SupplierInvoice> GetSupplierInvoiceAsync(long supplierInvoiceId);
        Task<SupplierInvoice> AddSupplierInvoiceAsync(SupplierInvoice supplierInvoice);
        Task<SupplierInvoice> GetSupplierInvoicePODetailAsync(SupplierInvoice supplierInvoice);
        Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice);
        Task<bool> DeleteSupplierInvoiceAsync(long supplierInvoiceId);
        Task ReceiveSupplierInvoiceAsync(long supplierInvoiceId);
        Task ReceiveBoxInvoiceAsync(string barcode);
        Task UploadFileAsync(int id, string docType, string path);
    }
}
