using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IPackingSlipRepository
    {
        Task<IEnumerable<PackingSlip>> GetAllPackingSlipsAsync(int companyId,int userId);
        Task<PackingSlip> GetPackingSlipAsync(long Id);
        PackingSlip GetPackingSlip(long id);
        Task<Int32> AddPackingSlipAsync(PackingSlip packingSlip);
        Task<bool> UpdatePackingSlipAsync(PackingSlip packingSlip);
        Task<bool> DeletePackingSlipAsync(long id);
        Task CreateInvoiceAsync(PackingSlip packingSlip);
        Task UpdatePOSAsync(int packingSlipId, string path,string trackingNumber,string accessId);
        Task<IEnumerable<DeletedPackingSlip>> GetDeletedPackingSlipAsync(int companyId);
        Task<bool> VerifyPackingSlipAsync(PackingSlip packingSlip, int userId);
        Task<bool> UndoVerifyPackingSlipAsync(int packingSlipId, int userId);
        Task<List<PackingSlipScanBoxeStatus>> ScanPackingSlipBox(string barcode, int userId);
        Task<bool> ScanAutoPackingSlip(int packingSlipId, int userId);
        Task<PackingSlip> GetPackingSlipFromBarcodeAsync(string barcode);
        Task<bool> AllowScanning(int packingSlipId, int userId);
        Task<int> GetIdByAccessIdAsync(string accessId);
    }
}
