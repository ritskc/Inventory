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
    public class EntityTrackerController : ControllerBase
    {
        private readonly IEntityTrackerService  entityTrackerService;

        public EntityTrackerController(IEntityTrackerService entityTrackerService)
        {
            this.entityTrackerService = entityTrackerService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<EntityTracker>>> GetAllEntityTracker(int companyId)
        {
            try
            {
                var result = await this.entityTrackerService.GetAllEntityTrackerAsync(companyId); ;

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
        public async Task<ActionResult<EntityTracker>> Get(int companyId, string finYear)
        {
            try
            {
                var result = await this.entityTrackerService.GetEntityTrackerAsync(companyId, finYear);

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
        public async Task<ActionResult> Post([FromBody] EntityTracker entityTracker)
        {
            try
            {
                await this.entityTrackerService.AddEntityTrackerAsyncAsync(entityTracker);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}