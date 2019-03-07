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
using DAL.Models;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            this._companyService = companyService;
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<IEnumerable<Company>> GetCompanys()
        {
            return await this._companyService.GetAllCompanyAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> Get(int id)
        {
            var result = await this._companyService.GetCompanyAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Company company)
        {
            await this._companyService.AddCompanyAsync(company);
            return Ok();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Company company)
        {
            if (id != company.Id)
            {
                return BadRequest();
            }

            company.Id = id;
            await this._companyService.UpdateCompanyAsync(company);

            return Ok();
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTodoItem(long id)
        {
            var result = await this._companyService.GetCompanyAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            await this._companyService.DeleteCompanyAsync(id);

            return Ok();
        }
    }
}