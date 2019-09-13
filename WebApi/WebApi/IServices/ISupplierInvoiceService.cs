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
        Task AddSupplierInvoiceAsync(SupplierInvoice supplierInvoice);
        Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice);
        Task<int> DeleteSupplierInvoiceAsync(long supplierInvoiceId);
        Task ReceiveSupplierInvoiceAsync(long supplierInvoiceId);
        Task ReceiveBoxInvoiceAsync(string barcode);
    }
}
