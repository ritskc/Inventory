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

        public OrdersController(IOrderService orderService)
        {
            this._orderService = orderService;
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