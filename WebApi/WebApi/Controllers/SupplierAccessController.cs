using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApi.IServices;
using WebApi.Settings;
using WebApi.Utils;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SupplierAccessController : ControllerBase
    {
        private readonly IPoService _poService;
        private readonly IPartService _partService;
        private readonly AppSettings _appSettings;
        private readonly ICompanyService companyService;

        public SupplierAccessController(IPoService poService, IPartService partService, IOptions<AppSettings> appSettings, ICompanyService companyService)
        {
            this._poService = poService;
            this._partService = partService;
            _appSettings = appSettings.Value;
            this.companyService = companyService;
        }
        // PUT api/values/5
        [HttpPut("{acknowledge}/{id}")]
        public async Task<IActionResult> AcknowledgePO(string id, [FromBody] Po po)
        {
            try
            {                
                if (id != po.AccessId)
                {
                    return BadRequest();
                }

                var parts = await this._partService.GetAllPartsAsync(po.CompanyId,1);
                if (po == null || po.poDetails == null)
                    return BadRequest("Invalid PO");

                var existingPo = await this._poService.GetPoAsync(po.Id,1);
                if (existingPo != null && existingPo.IsClosed)
                    return BadRequest("PO is already closed.PO is not editable");
                //if (existingPo != null && existingPo.poDetails != null)
                //{
                //    var processedPoItems = existingPo.poDetails.Where(x => x.IsClosed || x.InTransitQty > 0 || x.ReceivedQty > 0);
                //    if (processedPoItems != null && processedPoItems.Count() > 0)
                //        return BadRequest("PO Detail(s) is already processed. PO is not editable");
                //}

                foreach (PoDetail poDetail in po.poDetails)
                {
                    var part = parts.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                    //var part = parts.Where(x => x.partSupplierAssignments.Select(p => p.PartID == poDetail.PartId && p.SupplierID == po.SupplierId).FirstOrDefault()).FirstOrDefault();
                    if (part == null)
                    {
                        part = parts.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                        if (part == null)
                            return BadRequest(string.Format("Invalid part"));

                    }
                    var supplier = part.partSupplierAssignments.Where(x => x.SupplierID == po.SupplierId).FirstOrDefault();
                    if (supplier == null)
                        return BadRequest(string.Format("Invalid part : {0} does not belong to this supplier", part.Code));

                }
                //po.Id = id;
                await this._poService.AcknowledgePoAsync(po);

                var company = await this.companyService.GetCompanyAsync(po.CompanyId);
                po.CompanyName = company.Name;

                EmailService emailService = new EmailService(_appSettings);
                emailService.SendNotifyAcknoledgePOEmail(po.CompanyName, po.SupplierName,  po.PoNo);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // GET api/values/5        
        [HttpGet("{id}")]
        public async Task<ActionResult<Po>> Get(string id)
        {
            try
            {
                var result = await this._poService.GetPoByAccessIdAsync(id,1);

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
    }
}