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
        [HttpGet]
        public async Task<IEnumerable<Part>> GetParts()
        {
            return  await this._partService.GetAllPartsAsync();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Part>> Get(int id)
        {
            var result = await this._partService.GetPartAsync(id);

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
            await this._partService.AddPartAsync(part);

            return NoContent();
        }
       
        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Part>> DeleteTodoItem(long id)
        {
            var result = await this._partService.GetPartAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            result = await this._partService.DeletePartAsync(id);

            return result;
        }
    }
}