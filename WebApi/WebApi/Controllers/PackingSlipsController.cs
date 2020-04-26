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
using System.Security.Claims;

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
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this.packingSlipService.GetAllPackingSlipsAsync(companyId,userId);

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

        // DELETE: api/Todo/5
        [HttpPut("{id}")]
        public async Task<ActionResult<long>> Put(long id, [FromBody] PackingSlip packingSlip)
        {
            try
            {
                if (id != packingSlip.Id)
                {
                    return BadRequest();
                }

                var result = await this.packingSlipService.GetPackingSlipAsync(id);
                if (result == null)
                {
                    return NotFound();
                }


                if (result.IsInvoiceCreated)
                    return BadRequest("Invoice is already created. You can not update this Packing slip");


                await this.packingSlipService.UpdatePackingSlipAsync(packingSlip);

                return id;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(long id)
        {
            try
            {
                var result = await this.packingSlipService.GetPackingSlipAsync(id);
                if (result == null)
                {
                    return NotFound();
                }


                if (result.IsPOSUploaded)
                    return BadRequest("POS is already uploaded. You can not delete this Packing slip");


                await this.packingSlipService.DeletePackingSlipAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}