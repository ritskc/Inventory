﻿using System;
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

        public SupplierInvoiceController(ISupplierInvoiceService supplierInvoiceService)
        {
            this.supplierInvoiceService = supplierInvoiceService;
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
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SupplierInvoice supplierInvoice)
        {
            try
            {
                await this.supplierInvoiceService.AddSupplierInvoiceAsync(supplierInvoice);
                return Ok();
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
                await this.supplierInvoiceService.ReceiveSupplierInvoiceAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}