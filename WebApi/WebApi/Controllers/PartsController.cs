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
    //[Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartService _partService;

        public PartsController(IPartService partService)
        {
            this._partService = partService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<Part>>> GetParts(int companyId)
        {
            try
            {
                var result = await this._partService.GetAllPartsAsync(companyId);

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
        public async Task<ActionResult<Part>> Get(int companyId, int id)
        {
            try
            {               
                var result = await this._partService.GetPartAsync(id);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }

        [HttpGet("{companyId}/{type}/{typeId}")]
        public async Task<ActionResult<IEnumerable<Part>>> GetPartsByType(int companyId,string type, int typeId)
        {
            try
            {

                if (type == "customer")
                {
                    var result = await this._partService.GetPartByCustomerIdAsync(typeId);
                    return result.ToList();
                }
                else if (type == "supplier")
                {
                    var result = await this._partService.GetPartBySupplierIdAsync(typeId);
                    return result.ToList();
                }
                else
                    return BadRequest();
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Part part)
        {
            try
            {
                if(part == null)
                    return StatusCode(500,"invalid part");
                if (string.IsNullOrEmpty(part.Code) || string.IsNullOrEmpty(part.Description))
                    return StatusCode(500, "invalid partcode / description");
                if(part.partSupplierAssignments == null || part.partSupplierAssignments.Count() < 1 ||
                    part.partCustomerAssignments == null || part.partCustomerAssignments.Count() < 1)
                    return StatusCode(500, "invalid part - Atleast one supplier and Customer requires to create a part");
                var parts = await this._partService.GetAllPartsAsync(part.CompanyId);
                if (parts.Where(x => x.Code == part.Code).Count() > 0)
                    return StatusCode(302);

                if (part.partSupplierAssignments.Any(x=>x.MapCode == null || x.MapCode.Trim() == string.Empty))
                    return StatusCode(500, "invalid Mapcode - mapcode can not be empty");

                if (part.partCustomerAssignments.Any(x => x.MapCode == null || x.MapCode.Trim() == string.Empty))
                    return StatusCode(500, "invalid Mapcode - mapcode can not be empty");

                await this._partService.AddPartAsync(part);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Part part)
        {
            try
            {
                if (id != part.Id)
                {
                    return BadRequest();
                }

                if (part == null)
                    return StatusCode(500, "invalid part");
                if (string.IsNullOrEmpty(part.Code) || string.IsNullOrEmpty(part.Description))
                    return StatusCode(500, "invalid partcode / description");
                if (part.partSupplierAssignments == null || part.partSupplierAssignments.Count() < 1 ||
                    part.partCustomerAssignments == null || part.partCustomerAssignments.Count() < 1)
                    return StatusCode(500, "invalid part - Atleast one supplier and Customer requires to create a part");

                part.Id = id;
                await this._partService.UpdatePartAsync(part);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{type}/{typeName}/{partCode}/{rate}")]
        public async Task<IActionResult> Put(string type, string typeName, string partCode, decimal rate)
        {
            try
            {               
                if(type == "customer")
                    await this._partService.UpdatePartCustomerPriceAsync(typeName, partCode, rate);
                else if(type== "supplier" )
                    await this._partService.UpdatePartSupplierPriceAsync(typeName, partCode, rate);
                else
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        //DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Part>> Delete(int id)
        {
            try
            {
                var result = await this._partService.GetPartAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                await this._partService.DeletePartAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}