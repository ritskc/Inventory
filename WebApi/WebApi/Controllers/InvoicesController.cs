using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly ILogger<InvoicesController> logger;
        private readonly IPackingSlipService packingSlipService;
        
        public InvoicesController(IPackingSlipService packingSlipService,
            ILogger<InvoicesController> logger)
        {
            this.packingSlipService = packingSlipService;
            this.logger = logger;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PackingSlip packingSlip)
        {
            try
            {
                logger.LogInformation("customer invoice created");
                await this.packingSlipService.CreateInvoiceAsync(packingSlip);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError("Error in invoice creation {0} ",ex.StackTrace);
                return StatusCode(500, ex.ToString());
            }
        }
    }
}