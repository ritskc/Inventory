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
    public class SupplierInvoiceController : ControllerBase
    {
        private readonly ISupplierInvoiceService supplierInvoiceService;
        private readonly ICompanyService companyService;

        public SupplierInvoiceController(ISupplierInvoiceService supplierInvoiceService,
            ICompanyService companyService)
        {
            this.supplierInvoiceService = supplierInvoiceService;
            this.companyService = companyService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<SupplierInvoice>>> GetPos(int companyId)
        {
            try
            {
                var result = await this.supplierInvoiceService.GetAllSupplierInvoicesAsync(companyId);

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
        public async Task<ActionResult<SupplierInvoice>> Get(int companyId, int id)
        {
            try
            {
                var result = await this.supplierInvoiceService.GetSupplierInvoiceAsync(id);

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
        [HttpPost("{step}")]
        public async Task<ActionResult<SupplierInvoice>> Post(int step,[FromBody] SupplierInvoice supplierInvoice)
        {
            try
            {
                var company = await this.companyService.GetCompanyByNameAsync(supplierInvoice.CompanyName);
                supplierInvoice.CompanyId = company.Id;

                var invoice = await this.supplierInvoiceService.GetSupplierInvoiceAsync(supplierInvoice.InvoiceNo);

                //var invoice = invoices.Where(x => x.InvoiceNo == supplierInvoice.InvoiceNo).FirstOrDefault();
                if(!(invoice == null || invoice.supplierInvoiceDetails == null || invoice.supplierInvoiceDetails.Count == 0))
                    return StatusCode(500, "Invoice already uploaded");
                if (step == 1)
                {
                    var result = await this.supplierInvoiceService.GetSupplierInvoicePODetailAsync(supplierInvoice);
                    return result;
                }
                else
                {
                    var result = await this.supplierInvoiceService.AddSupplierInvoiceAsync(supplierInvoice);
                    return result;
                }                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // POST api/values
        [HttpPost("receive/{id}")]
        public async Task<ActionResult> Post(long id)
        {
            try
            {
                var result = await this.supplierInvoiceService.GetSupplierInvoiceAsync(id);
                if (result == null)
                    return NotFound();

                if (result.IsInvoiceReceived)
                    return StatusCode(500, "Invoice already received");

                await this.supplierInvoiceService.ReceiveSupplierInvoiceAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // POST api/values
        [HttpPost("receive/box/{barcode}")]
        public async Task<ActionResult> Post(string barcode)
        {
            try
            {
                await this.supplierInvoiceService.ReceiveBoxInvoiceAsync(barcode);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        //DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await this.supplierInvoiceService.DeleteSupplierInvoiceAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}