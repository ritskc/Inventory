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
    public class UserReportsController : ControllerBase
    {
        private readonly IUserReportService userReportService;
        public UserReportsController(IUserReportService userReportService)
        {
            this.userReportService = userReportService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserMenuReport>>> Get()
        {
            try
            {
                var result = await this.userReportService.GetDefaultReportsAsync();

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
        public async Task<ActionResult<IEnumerable<UserMenuReport>>> Get(int companyId)
        {
            try
            {
                var result = await this.userReportService.GetReportsAsync();

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
        public async Task<ActionResult<UserMenuReport>> Get(int companyId, int id)
        {
            try
            {
                var result = await this.userReportService.GetReportsAsync();               

                if (result == null)
                {
                    return NotFound();
                }

                return result.Where(x=>x.PriviledgeId == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserMenuReport userMenuReport)
        {
            try
            {
                await this.userReportService.AddUserPriviledgeAsync(userMenuReport);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put([FromBody] UserMenuReport userMenuReport)
        {
            try
            {
                await this.userReportService.AddUserPriviledgeAsync(userMenuReport);
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
                var result = await this.userReportService.GetReportsAsync();

                if (result == null)
                {
                    return NotFound();
                }

                var resultOne= result.Where(x => x.PriviledgeId == id).FirstOrDefault();

                if (resultOne == null)
                {
                    return NotFound();
                }

                await this.userReportService.DeleteserPriviledgeAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}