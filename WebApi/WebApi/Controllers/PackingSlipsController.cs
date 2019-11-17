using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;
using Microsoft.AspNetCore.Http.Extensions;

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

        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id,IFormFile file,string trackingNumber)
        {
            long size = file.Length;

            // full path to file in temp location
            //var filePath = Path.GetTempFileName();            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),"POS", id.ToString()+ "_POS.pdf");

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }        
            
            var relativeFilePath = "Docs\\POS\\" + id.ToString() + "_POS.pdf";

            var result = packingSlipService.UpdatePOSAsync(id, relativeFilePath,trackingNumber);

            return Ok();
        }        
    }
}