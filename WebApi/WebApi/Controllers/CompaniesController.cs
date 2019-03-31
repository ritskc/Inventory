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
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanys()
        {
            try
            {
                var result = await this._companyService.GetAllCompanyAsync();

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
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> Get(int id)
        {
            try
            {
                var result = await this._companyService.GetCompanyAsync(id);

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
        public async Task<ActionResult> Post([FromBody] Company company)
        {
            try
            {
                await this._companyService.AddCompanyAsync(company);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Company company)
        {
            try
            {
                if (id != company.Id)
                {
                    return BadRequest();
                }

                company.Id = id;
                await this._companyService.UpdateCompanyAsync(company);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var result = await this._companyService.GetCompanyAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                await this._companyService.DeleteCompanyAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}