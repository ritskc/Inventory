using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using WebApi.IServices;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            this._supplierService = supplierService;
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<IEnumerable<Supplier>> GetSuppliers()
        {
            return await this._supplierService.GetAllSupplierAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> Get(int id)
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
        public async Task<ActionResult<Supplier>> DeleteTodoItem(long id)
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
