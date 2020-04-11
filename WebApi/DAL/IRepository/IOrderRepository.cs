using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderMaster>> GetAllOrderMastersAsync(int companyId,int userId);
        Task<OrderMaster> GetOrderMasterAsync(long orderId);
        Task<OrderMaster> GetOrderMasterAsync(long orderId, SqlConnection conn, SqlTransaction transaction);
        Task<long> AddOrderMasterAsync(OrderMaster order);
        Task UpdateOrderMasterAsync(OrderMaster order);
        Task DeleteOrderMasterAsync(long orderId);
        Task UpdateOrderAsync(int id, string path);
    }
}
