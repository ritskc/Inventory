using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task<IEnumerable<Supplier>> GetSuppliers(int companyId) => await this._supplierService.GetAllSupplierAsync(companyId);

        // GET api/values/5
        [HttpGet("{companyId}/{id}")]
        public async Task<ActionResult<Supplier>> Get(int companyId,int id)
        {
            var result = await this._supplierService.GetSupplierAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Supplier supplier)
        {
            await this._supplierService.AddSupplierAsync(supplier);
            return NoContent();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return BadRequest();
            }

            supplier.Id = id;
            await this._supplierService.UpdateSupplierAsync(supplier);

            return NoContent();
        }

        //DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Supplier>> DeleteTodoItem(int id)
        {
            var result = await this._supplierService.GetSupplierAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            await this._supplierService.DeleteSupplierAsync(id);

            return result;
        }
    }
}
