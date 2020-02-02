using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;
using WebApi.Utils;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierAccessController : ControllerBase
    {
        private readonly IPoService _poService;
        private readonly IPartService _partService;

        public SupplierAccessController(IPoService poService, IPartService partService)
        {
            this._poService = poService;
            this._partService = partService;
        }
        // PUT api/values/5
        [HttpPut("{acknowledge}/{id}")]
        public async Task<IActionResult> AcknowledgePO(int id, [FromBody] Po po)
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
                if (existingPo != null && existingPo.poDetails != null)
                {
                    var processedPoItems = existingPo.poDetails.Where(x => x.IsClosed || x.InTransitQty > 0 || x.ReceivedQty > 0);
                    if (processedPoItems != null && processedPoItems.Count() > 0)
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
                    if (supplier == null)
                        return BadRequest(string.Format("Invalid part : {0} does not belong to this supplier", part.Code));

                }
                po.Id = id;
                await this._poService.AcknowledgePoAsync(po);                

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}