using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderMaster>> GetAllOrderMastersAsync(int companyId,int userId);
        Task<OrderMaster> GetOrderMasterAsync(long orderId);
        Task<long> AddOrderMasterAsync(OrderMaster order);
        Task UpdateOrderMasterAsync(OrderMaster order);
        Task DeleteOrderMasterAsync(long orderId);
        Task UpdateOrderAsync(int id, string path);
    }
}
