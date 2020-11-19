using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IPoRepository
    {
        Task<IEnumerable<Po>> GetAllPosAsync(int companyId,int userId);
        Task<IEnumerable<Po>> GetAllPosAsync(int companyId, int userId, int supplierId);
        Task<IEnumerable<Po>> GetAllOpenPosAsync(int companyId, int userId);
        Task<Po> GetPoAsync(long poId);
        Task<Po> GetPobyPoNoAsync(int companyId, string poNo);
        Task<Po> GetPoByAccessIdAsync(string id);
        Task<Po> GetPoAsync(long poId, SqlConnection conn, SqlTransaction transaction);
        Task AddPoAsync(Po po);
        Task UpdatePoAsync(Po po);
        Task AcknowledgePoAsync(Po po);
        Task AcknowledgePoAsync(int poId,string accessId);
        Task<int> DeletePoAsync(long poId);
    }
}
