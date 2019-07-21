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
    public class InvoicesController : ControllerBase
    {
        private readonly IPackingSlipService packingSlipService;
        
        public InvoicesController(IPackingSlipService packingSlipService)
        {
            this.packingSlipService = packingSlipService;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PackingSlip packingSlip)
        {
            try
            {
                await this.packingSlipService.CreateInvoiceAsync(packingSlip);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}