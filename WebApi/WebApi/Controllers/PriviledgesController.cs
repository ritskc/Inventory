using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PriviledgesController : ControllerBase
    {
        private readonly IPriviledgeService priviledgeService;
        public PriviledgesController(IPriviledgeService priviledgeService)
        {
            this.priviledgeService = priviledgeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserPriviledgeDetail>>> Get()
        {
            try
            {
                var result = await this.priviledgeService.GetRawPriviledgeAsync();

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

        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<UserPriviledge>>> Get(int companyId)
        {
            try
            {
                var result = await this.priviledgeService.GetAllPriviledgeAsync();

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

        [HttpGet("{companyId}/{id}")]
        public async Task<ActionResult<UserPriviledge>> Get(int companyId, int id)
        {
            try
            {
                var result = await this.priviledgeService.GetPriviledgeAsync(id);

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

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserPriviledge userPriviledge)
        {
            try
            {
                await this.priviledgeService.AddUserPriviledgeAsync(userPriviledge);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] UserPriviledge userPriviledge)
        {
            try
            {
                await this.priviledgeService.UpdateUserPriviledgeAsync(userPriviledge);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Po>> Delete(int id)
        {
            try
            {
                var result = await this.priviledgeService.GetPriviledgeAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                await this.priviledgeService.DeleteUserPriviledgeAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}