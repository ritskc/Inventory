using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPartService _partService;

        public OrdersController(IOrderService orderService, IPartService partService)
        {
            this._orderService = orderService;
            this._partService = partService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<OrderMaster>>> GetPos(int companyId)
        {
            try
            {
                var result = await this._orderService.GetAllOrderMastersAsync(companyId);

                if (result == null)
                {
                    return NotFound();
                }

                return result.ToList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }

        // GET api/values/5        
        [HttpGet("{companyId}/{id}")]
        public async Task<ActionResult<OrderMaster>> Get(int companyId, int id)
        {
            try
            {
                var result = await this._orderService.GetOrderMasterAsync(id);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<long>> Post([FromBody] OrderMaster po)
        {
            try
            {
                var result= await this._orderService.AddOrderMasterAsync(po);
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] OrderMaster po)
        {
            try
            {
                if (id != po.Id)
                {
                    return BadRequest();
                }

                po.Id = id;
                var parts = await this._partService.GetAllPartsAsync(po.CompanyId);
                if (po == null || po.OrderDetails == null)
                    return BadRequest("Invalid PO");

                var existingPo = await this._orderService.GetOrderMasterAsync(po.Id);
                if (existingPo != null && existingPo.IsClosed)
                    return BadRequest("PO is already closed.PO is not editable");

                //if(existingPo !=null && existingPo.poDetails != null)
                //{
                //    var processedPoItems = existingPo.poDetails.Where(x => x.IsClosed || x.InTransitQty > 0 || x.ReceivedQty > 0);
                //    if(processedPoItems != null && processedPoItems.Count() > 0)
                //        return BadRequest("PO Detail(s) is already processed. PO is not editable");
                //}

                foreach (OrderDetail poDetail in po.OrderDetails)
                {
                    if (poDetail.Id != 0)
                    {
                        var part = parts.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                        //var part = parts.Where(x => x.partSupplierAssignments.Select(p => p.PartID == poDetail.PartId && p.SupplierID == po.SupplierId).FirstOrDefault()).FirstOrDefault();
                        if (part == null)
                        {
                            part = parts.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                            if (part == null)
                                return BadRequest(string.Format("Invalid part"));

                        }

                        var existingPartDetail = existingPo.OrderDetails.Where(x => x.Id == poDetail.Id).FirstOrDefault();
                        if (existingPartDetail.PartId != poDetail.PartId)
                            return BadRequest(string.Format("Invalid part associated in edited po line number"));

                        if (existingPartDetail.IsClosed)
                        {
                            if (poDetail.Qty != existingPartDetail.Qty)
                                return BadRequest(string.Format("Part is already closed you can not change the qty of this part", poDetail.part.Code));
                        }
                        if (poDetail.Qty < (existingPartDetail.ShippedQty))
                            return BadRequest(string.Format("Invalid part associated in edited po line number"));
                        if (poDetail.IsForceClosed)
                        {
                            poDetail.IsClosed = true;
                        }

                        if (poDetail.Qty == (existingPartDetail.ShippedQty))
                        {
                            if (!existingPartDetail.IsClosed)
                            {
                                poDetail.IsClosed = true;
                                poDetail.IsForceClosed = true;
                            }
                        }
                    }
                    //else
                    //{
                    //    var selectedParts = parts.Where(x => x.Id == poDetail.PartId);
                    //    if (selectedParts == null)
                    //    {
                    //        return BadRequest(string.Format("Invalid part"));
                    //    }
                    //    var part = selectedParts.Select(x => x.partCustomerAssignments.Where(p => p.CustomerId == po.CustomerId).FirstOrDefault()).FirstOrDefault();

                    //    if (part == null)
                    //    {
                    //        return BadRequest(string.Format("Invalid part : {0} does not belong to this customer", selectedParts.Select(x => x.Code).FirstOrDefault()));
                    //    }
                    //}

                }

                await this._orderService.UpdateOrderMasterAsync(po);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Po>> Delete(long id)
        {
            try
            {
                var result = await this._orderService.GetOrderMasterAsync(id);
                if(result.IsClosed)
                    return BadRequest("Order is already processed. You can not delete the order.");
                var processedPoItems = result.OrderDetails.Where(x => x.IsClosed || x.ShippedQty > 0 );
                if (processedPoItems != null && processedPoItems.Count() > 0)
                    return BadRequest("Order is already processed. You can not delete the order.");
               
                if (result == null)
                {
                    return NotFound();
                }

                await this._orderService.DeleteOrderMasterAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}