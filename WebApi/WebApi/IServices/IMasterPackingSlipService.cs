using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IMasterPackingSlipService
    {
        Task<IEnumerable<MasterPackingSlip>> GetAllMasterPackingSlipsAsync(int companyId,int userId);
        Task<MasterPackingSlip> GetMasterPackingSlipAsync(long Id);
        MasterPackingSlip GetMasterPackingSlip(long id);
        Task<Int32> AddMasterPackingSlipAsync(MasterPackingSlip packingSlip);
        Task<bool> UpdateMasterPackingSlipAsync(MasterPackingSlip packingSlip);
        Task<bool> DeleteMasterPackingSlipAsync(long id);
        Task UpdatePOSAsync(int masterPackingSlipId, string path, string trackingNumber);
    }
}
