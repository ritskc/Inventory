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
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<IEnumerable<Part>> GetParts(int companyId)
        {
            return  await this._partService.GetAllPartsAsync(companyId);
        }

        // GET api/values/5
        [HttpGet("{companyId}/{id}")]
        public async Task<ActionResult<Part>> Get(int companyId,int id)
        {
            var result = await this._partService.GetPartAsync(companyId,id);

            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Part part)
        {
             await this._partService.AddPartAsync(part);
             return NoContent();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Part part)
        {
            if (id != part.Id)
            {
                return BadRequest();
            }

            part.Id = id;
            await this._partService.UpdatePartAsync(part);

            return NoContent();
        }

        // DELETE: api/Todo/5
        [HttpGet("{companyId}/{id}")]
        public async Task<ActionResult<Part>> DeleteTodoItem(int comnpanyId,long id)
        {
            var result = await this._partService.GetPartAsync(comnpanyId,id);
            if (result == null)
            {
                return NotFound();
            }

            await this._partService.DeletePartAsync(id);

            return result;
        }
    }
}