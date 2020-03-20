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
    public class PosController : ControllerBase
    {
        private readonly IPoService _poService;
        private readonly IPartService _partService;
        private readonly AppSettings _appSettings;

        public PosController(IPoService poService,IPartService partService, IOptions<AppSettings> appSettings)
        {
            this._poService = poService;
            this._partService = partService;
            _appSettings = appSettings.Value;
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
                var parts = await this._partService.GetAllPartsAsync(po.CompanyId);
                if (po == null || po.poDetails == null)
                    return BadRequest("Invalid PO");
                foreach(PoDetail poDetail in po.poDetails)
                {
                    //foreach(Part tmpPart in parts)
                    //{
                    //    if(tmpPart.Id == poDetail.PartId)
                    //    {
                    //        tmpPart.partSupplierAssignments.Where(x=>x.SupplierID == po.SupplierId)
                    //    }
                    //}
                    //var part = parts.Where(x => x.partSupplierAssignments.Select(p => p.PartID == poDetail.PartId && p.SupplierID == po.SupplierId).FirstOrDefault()).FirstOrDefault();
                    var selectedParts = parts.Where(x => x.Id == poDetail.PartId);
                    if (selectedParts == null)
                    {
                        return BadRequest(string.Format("Invalid part"));                        
                    }
                    var part = selectedParts.Select(x => x.partSupplierAssignments.Where(p => p.SupplierID == po.SupplierId).FirstOrDefault()).FirstOrDefault();
                   
                    if (part == null)
                    {
                        return BadRequest(string.Format("Invalid part : {0} does not belong to this supplier", selectedParts.Select(x=>x.Code).FirstOrDefault()));
                    }
                }
                po.AccessId = Guid.NewGuid().ToString();
                await this._poService.AddPoAsync(po);

                EmailService emailService = new EmailService(_appSettings);
                emailService.SendAcknoledgePOEmail(po.CompanyName,po.SupplierName,po.ContactPersonName,_appSettings.POURL + po.AccessId,po.PoNo);
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

                var parts = await this._partService.GetAllPartsAsync(po.CompanyId);
                if (po == null || po.poDetails == null)
                    return BadRequest("Invalid PO");

                var existingPo = await this._poService.GetPoAsync(po.Id);
                if (existingPo != null && existingPo.IsClosed)
                    return BadRequest("PO is already closed.PO is not editable");
                if(existingPo !=null && existingPo.poDetails != null)
                {
                    var processedPoItems = existingPo.poDetails.Where(x => x.IsClosed || x.InTransitQty > 0 || x.ReceivedQty > 0);
                    if(processedPoItems != null && processedPoItems.Count() > 0)
                        return BadRequest("PO Detail(s) is already processed. PO is not editable");
                }

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
                    if(supplier == null)
                        return BadRequest(string.Format("Invalid part : {0} does not belong to this supplier", part.Code));

                }
                po.Id = id;
                po.AccessId = existingPo.AccessId;

                if(po.AccessId == null || po.AccessId == string.Empty || po.AccessId == "")
                    po.AccessId  = Guid.NewGuid().ToString();

                await this._poService.UpdatePoAsync(po);

                EmailService emailService = new EmailService(_appSettings);
                emailService.SendAcknoledgePOEmail(po.CompanyName, po.SupplierName, po.ContactPersonName, _appSettings.POURL + po.AccessId, po.PoNo);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }        

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Po>> Delete(long id)
        {
            try
            {
                var result = await this._poService.GetPoAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                var existingPo = await this._poService.GetPoAsync(id);
                if (existingPo != null && existingPo.IsClosed)
                    return BadRequest("PO is already closed. You can not delete this PO");
                if (existingPo != null && existingPo.poDetails != null)
                {
                    var processedPoItems = existingPo.poDetails.Where(x => x.IsClosed || x.InTransitQty > 0 || x.ReceivedQty > 0);
                    if (processedPoItems != null && processedPoItems.Count() > 0)
                        return BadRequest("PO Detail(s) is already processed. You can not delete this PO");
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