using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            this._supplierService = supplierService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers(int companyId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this._supplierService.GetAllSupplierAsync(companyId,userId); 

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
        public async Task<ActionResult<Supplier>> Get(int companyId,int id)
        {
            try
            {
                var result = await this._supplierService.GetSupplierAsync(id);

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
        public async Task<ActionResult> Post([FromBody] Supplier supplier)
        {
            try
            {
                await this._supplierService.AddSupplierAsync(supplier);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Supplier supplier)
        {
            try
            {
                if (id != supplier.Id)
                {
                    return BadRequest();
                }

                supplier.Id = id;
                await this._supplierService.UpdateSupplierAsync(supplier);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }

        //DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Supplier>> Delete(int id)
        {
            try
            {
                var result = await this._supplierService.GetSupplierAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                await this._supplierService.DeleteSupplierAsync(id);

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
