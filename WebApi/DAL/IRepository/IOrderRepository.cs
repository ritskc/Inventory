using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderMaster>> GetAllOrderMastersAsync(int companyId);
        Task<OrderMaster> GetOrderMasterAsync(long orderId);
        Task AddOrderMasterAsync(OrderMaster order);
        Task UpdateOrderMasterAsync(OrderMaster order);
        Task DeleteOrderMasterAsync(long orderId);
        Task UpdateOrderAsync(int id, string path);
    }
}
