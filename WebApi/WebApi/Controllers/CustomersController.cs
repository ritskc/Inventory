using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            this._customerService = customerService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(int companyId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this._customerService.GetAllCustomerAsync(companyId,userId);

                if (result == null)
                {
                    return NotFound();
                }

                return result.OrderBy(x=>x.Name).ToList();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
            
        }
               

        // GET api/values/5
        [HttpGet("{companyId}/{id}")]
        public async Task<ActionResult<Customer>> Get(int companyId, int id)
        {
            try
            {
                var result = await this._customerService.GetCustomerAsync(id);

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
        public async Task<ActionResult> Post([FromBody] Customer customer)
        {
            try
            {
                await this._customerService.AddCustomerAsync(customer);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            try
            {
                if (id != customer.Id)
                {
                    return BadRequest();
                }

                customer.Id = id;
                await this._customerService.UpdateCustomerAsync(customer);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }

        //DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Customer>> Delete(int id)
        {
            try
            {
                var result = await this._customerService.GetCustomerAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                await this._customerService.DeleteCustomerAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}