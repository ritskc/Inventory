using System;
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
    public class PosController : ControllerBase
    {
        private readonly IPoService _poService;

        public PosController(IPoService poService)
        {
            this._poService = poService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<Po>>> GetPos(int companyId)
        {
            try
            {
                var result= await this._poService.GetAllPosAsync(companyId);

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
        public async Task<ActionResult<Po>> Get(int companyId, int id)
        {
            try
            {
                var result = await this._poService.GetPoAsync(id);

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
        public async Task<ActionResult> Post([FromBody] Po po)
        {
            try
            {
                await this._poService.AddPoAsync(po);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Po po)
        {
            try
            {
                if (id != po.Id)
                {
                    return BadRequest();
                }

                po.Id = id;
                await this._poService.UpdatePoAsync(po);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // DELETE: api/Todo/5
        [HttpGet("{companyId}/{id}")]
        public async Task<ActionResult<Po>> Delete(long id)
        {
            try
            {
                var result = await this._poService.GetPoAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                await this._poService.DeletePoAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}