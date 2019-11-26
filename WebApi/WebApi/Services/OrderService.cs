using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.IRepository;
using DAL.Models;
using WebApi.IServices;

namespace WebApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IPartRepository _partRepository;

        public OrderService(IOrderRepository orderRepository, IPartRepository partRepository)
        {
            _orderRepository = orderRepository;
            _partRepository = partRepository;
        }
        public async Task AddOrderMasterAsync(OrderMaster order)
        {
            await this._orderRepository.AddOrderMasterAsync(order);
        }

        public async Task DeleteOrderMasterAsync(long orderId)
        {
            await this._orderRepository.DeleteOrderMasterAsync(orderId);            
        }

        public async Task<IEnumerable<OrderMaster>> GetAllOrderMastersAsync(int companyId)
        {
            //return await this.orderRepository.GetAllOrderMastersAsync(companyId);

            var result = await this._orderRepository.GetAllOrderMastersAsync(companyId);
            foreach (OrderMaster pos in result)
            {
                foreach (OrderDetail poDetail in pos.OrderDetails)
                {
                    var partDetail = await this._partRepository.GetPartAsync(poDetail.PartId);
                    poDetail.part = partDetail;
                }
            }
            return result;
        }

        public async Task<OrderMaster> GetOrderMasterAsync(long orderId)
        {
            //return await this._orderRepository.GetOrderMasterAsync(orderId);

            var result = await this._orderRepository.GetOrderMasterAsync(orderId);
            foreach (OrderDetail poDetail in result.OrderDetails)
            {
                var partDetail = await this._partRepository.GetPartAsync(poDetail.PartId);
                poDetail.part = partDetail;
            }
            return result;
        }

        public async Task UpdateOrderAsync(int id, string path)
        {
            await this._orderRepository.UpdateOrderAsync(id,path);
        }

        public async Task UpdateOrderMasterAsync(OrderMaster order)
        {
            await this._orderRepository.UpdateOrderMasterAsync(order);
        }
    }
}
