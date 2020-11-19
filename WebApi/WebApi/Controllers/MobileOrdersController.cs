using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MobileOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public MobileOrdersController(IOrderService orderService)
        {
            this._orderService = orderService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<OrderMaster>>> Get(int companyId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
                var result = await this._orderService.GetAllOpenOrderMastersAsync(companyId, userId);

                if (result == null)
                {
                    return NotFound();
                }

                return result.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }

        // GET: api/Todo
        [HttpGet("{companyId}/{customerId}")]
        public async Task<ActionResult<IEnumerable<OrderMaster>>> Get(int companyId,int customerId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
                var result = await this._orderService.GetAllOrderMastersAsync(companyId, userId, customerId);

                if (result == null)
                {
                    return NotFound();
                }

                return result.OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }
    }
}