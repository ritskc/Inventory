﻿using DAL.Models;
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
        Task<Int64> AddSupplierInvoiceAsync(SupplierInvoice supplierInvoice);
        Task UpdateSupplierInvoiceAsync(SupplierInvoice supplierInvoice);
        Task<bool> DeleteSupplierInvoiceAsync(long supplierInvoiceId);
        Task ReceiveSupplierInvoiceAsync(long supplierInvoiceId);
        Task ReceiveBoxInvoiceAsync(string barcode);
        Task UploadFileAsync(int id, string docType, string path);
    }
}
