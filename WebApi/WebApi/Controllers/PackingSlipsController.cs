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
using Microsoft.Extensions.Options;
using WebApi.Settings;
using WebApi.Utils;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PackingSlipsController : ControllerBase
    {
        private readonly IPackingSlipService packingSlipService;
        private readonly AppSettings _appSettings;
        private readonly ICompanyService companyService;

        public PackingSlipsController(IPackingSlipService packingSlipService, IOptions<AppSettings> appSettings, ICompanyService companyService)
        {
            this.packingSlipService = packingSlipService;
            _appSettings = appSettings.Value;
            this.companyService = companyService;
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

                return result.OrderByDescending(x => x.Id).ToList();
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

                var company = await companyService.GetCompanyAsync(packingSlip.CompanyId);
                EmailService emailService = new EmailService(_appSettings);
                emailService.SendContentEmail(company.Name,"Shipment Created: ", packingSlip.PackingSlipNo);

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("{scan}/{barcode}")]
        public async Task<ActionResult<bool>> ScanBox(string scan, string barcode)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                if (scan.ToLower() == "allowscanning")
                {
                    var result1 = await this.packingSlipService.AllowScanning(Convert.ToInt32(barcode), userId);
                    return Ok(result1);
                }

                if (scan.ToLower() == "undoverifyshipment")
                {
                    var result1 = await this.packingSlipService.UndoVerifyPackingSlipAsync(Convert.ToInt32(barcode), userId);
                    return Ok(result1);
                }

                if (scan.ToLower() == "autoscan")
                {
                    var result1 = await this.packingSlipService.ScanAutoPackingSlip(Convert.ToInt32(barcode), userId);
                    return Ok(result1);
                }

                var packingSlip = await this.packingSlipService.GetPackingSlipFromBarcodeAsync(barcode);

                if(!packingSlip.AllowScanning)
                    return StatusCode(500, "This packingslip is not ready for scanning. Please ask admin to enable it for Scanning.");
                var result = await this.packingSlipService.ScanPackingSlipBox(barcode, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("{verifyshipment}/{id}")]
        public async Task<ActionResult<bool>> VerifyShipment([FromBody] PackingSlip packingSlip)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this.packingSlipService.VerifyPackingSlipAsync(packingSlip, userId);
                return Ok(result);
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
            var accessId = Guid.NewGuid().ToString();
            var result = packingSlipService.UpdatePOSAsync(id, relativeFilePath,trackingNumber, accessId);

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