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
    public class MobileSupplierInvoiceController : ControllerBase
    {
        private readonly ISupplierInvoiceService supplierInvoiceService;        

        public MobileSupplierInvoiceController(ISupplierInvoiceService supplierInvoiceService)
        {
            this.supplierInvoiceService = supplierInvoiceService;           
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<SupplierInvoice>>> GetPos(int companyId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this.supplierInvoiceService.GetAllUnReceipveSupplierInvoicesAsync(companyId, userId);

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