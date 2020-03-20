using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface ISupplierInvoiceRepository
    {
        Task<IEnumerable<SupplierInvoice>> GetAllSupplierInvoicesAsync(int companyId);
        Task<SupplierInvoice> GetSupplierInvoiceAsync(long supplierInvoiceId);
        Task<IEnumerable<SupplierInvoice>> GetIntransitSupplierInvoicesAsync(int companyId);
        Task<IEnumerable<SupplierInvoice>> GetIntransitSupplierInvoicesByPartIdAsync(int companyId,int partId);
        Task<SupplierInvoice> GetSupplierInvoiceAsync(string invoiceNo);
        Task<Int64> AddSupplierInvoiceAsync(SupplierInvoice supplierInvoice);
        Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice);
        Task<bool> DeleteSupplierInvoiceAsync(long supplierInvoiceId);
        Task ReceiveSupplierInvoiceAsync(long supplierInvoiceId);
        Task UnReceiveSupplierInvoiceAsync(long supplierInvoiceId);
        Task ReceiveBoxInvoiceAsync(string barcode);
        Task UploadFileAsync(int id, string docType, string path);
    }
}
