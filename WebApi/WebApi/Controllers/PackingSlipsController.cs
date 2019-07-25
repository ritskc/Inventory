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
    public class PackingSlipsController : ControllerBase
    {
        private readonly IPackingSlipService packingSlipService;
        public PackingSlipsController(IPackingSlipService packingSlipService)
        {
            this.packingSlipService = packingSlipService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<PackingSlip>>> GetPackingSlips(int companyId)
        {
            try
            {
                var result = await this.packingSlipService.GetAllPackingSlipsAsync(companyId);

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
        public async Task<ActionResult<PackingSlip>> Get(int companyId, int id)
        {
            try
            {
                var result = await this.packingSlipService.GetPackingSlipAsync(id);

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
        public async Task<ActionResult<Int32>> Post([FromBody] PackingSlip packingSlip)
        {
            try
            {
                var result = await this.packingSlipService.AddPackingSlipAsync(packingSlip);
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }        
    }
}