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
        private readonly IPackingSlipService _packingSlipService;

        public OrderService(IOrderRepository orderRepository, IPartRepository partRepository, IPackingSlipService packingSlipService)
        {
            _orderRepository = orderRepository;
            _partRepository = partRepository;
            _packingSlipService = packingSlipService;
        }
        public async Task<long> AddOrderMasterAsync(OrderMaster order)
        {
            return await this._orderRepository.AddOrderMasterAsync(order);
        }

        public async Task DeleteOrderMasterAsync(long orderId)
        {
            await this._orderRepository.DeleteOrderMasterAsync(orderId);            
        }

        public async Task<IEnumerable<OrderMaster>> GetAllOrderMastersAsync(int companyId,int userId)
        {
            //return await this.orderRepository.GetAllOrderMastersAsync(companyId);

            var result = await this._orderRepository.GetAllOrderMastersAsync(companyId,userId);
            var packingSlips = await this._packingSlipService.GetAllPackingSlipsAsync(companyId,userId);
            foreach (OrderMaster pos in result)
            {
                foreach (OrderDetail poDetail in pos.OrderDetails)
                {
                    var partDetail = await this._partRepository.GetPartAsync(poDetail.PartId);
                    poDetail.part = partDetail;
                    poDetail.PackingSlipNo = "";
                    if (poDetail.ShippedQty > 0)
                    {
                        foreach (PackingSlip packingSlip in packingSlips)
                        {
                            foreach (PackingSlipDetails packingSlipDetails in packingSlip.PackingSlipDetails)
                            {
                                if (packingSlipDetails.OrderDetailId == poDetail.Id)
                                {
                                    poDetail.PackingSlipNo = packingSlip.PackingSlipNo;
                                    poDetail.ShippingDate = packingSlip.ShippingDate;
                                    break;
                                }
                            }
                        }
                    }
                   
                }
            }
            return result;
        }

        public async Task<IEnumerable<OrderMaster>> GetAllOrderMastersAsync(int companyId, int userId,int customerId)
        {
            //return await this.orderRepository.GetAllOrderMastersAsync(companyId);

            var result = await this._orderRepository.GetAllOrderMastersAsync(companyId, userId, customerId);
            var packingSlips = await this._packingSlipService.GetAllPackingSlipsAsync(companyId, userId);
            foreach (OrderMaster pos in result)
            {
                foreach (OrderDetail poDetail in pos.OrderDetails)
                {
                    var partDetail = await this._partRepository.GetPartAsync(poDetail.PartId);
                    poDetail.part = partDetail;
                    poDetail.PackingSlipNo = "";
                    if (poDetail.ShippedQty > 0)
                    {
                        foreach (PackingSlip packingSlip in packingSlips)
                        {
                            foreach (PackingSlipDetails packingSlipDetails in packingSlip.PackingSlipDetails)
                            {
                                if (packingSlipDetails.OrderDetailId == poDetail.Id)
                                {
                                    poDetail.PackingSlipNo = packingSlip.PackingSlipNo;
                                    poDetail.ShippingDate = packingSlip.ShippingDate;
                                    break;
                                }
                            }
                        }
                    }

                }
            }
            return result;
        }

        public async Task<IEnumerable<OrderMaster>> GetAllOpenOrderMastersAsync(int companyId, int userId)
        {
            var result = await this._orderRepository.GetAllOpenOrderMastersAsync(companyId, userId);
            var parts = await this._partRepository.GetAllPartsCompactAsync(companyId, userId);
            foreach (OrderMaster pos in result)
            {
                foreach (OrderDetail poDetail in pos.OrderDetails)
                {
                    var partDetail = parts.Where(x=>x.Id == poDetail.PartId).FirstOrDefault();
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
