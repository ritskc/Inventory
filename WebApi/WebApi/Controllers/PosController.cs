﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly ICompanyService companyService;

        public PosController(IPoService poService,IPartService partService, IOptions<AppSettings> appSettings, ICompanyService companyService)
        {
            this._poService = poService;
            this._partService = partService;
            _appSettings = appSettings.Value;
            this.companyService = companyService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<Po>>> GetPos(int companyId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result= await this._poService.GetAllPosAsync(companyId,userId);

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
        public async Task<ActionResult<Po>> Get(int companyId, int id)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this._poService.GetPoAsync(id,userId);

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
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
                
                var parts = await this._partService.GetAllPartsAsync(po.CompanyId,userId);
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

                var company = await this.companyService.GetCompanyAsync(po.CompanyId);
                po.CompanyName = company.Name;

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
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
                
                if (id != po.Id)
                {
                    return BadRequest();
                }

                var parts = await this._partService.GetAllPartsAsync(po.CompanyId,userId);
                if (po == null || po.poDetails == null)
                    return BadRequest("Invalid PO");

                var existingPo = await this._poService.GetPoAsync(po.Id,userId);
                if (existingPo != null && existingPo.IsClosed)
                    return BadRequest("PO is already closed.PO is not editable");



                var poDetails = existingPo.poDetails.Where(x => x.IsClosed == false).FirstOrDefault();
                if (poDetails == null)
                    return BadRequest("PO is already closed.PO is not editable");
                
                //if(existingPo !=null && existingPo.poDetails != null)
                //{
                //    var processedPoItems = existingPo.poDetails.Where(x => x.IsClosed || x.InTransitQty > 0 || x.ReceivedQty > 0);
                //    if(processedPoItems != null && processedPoItems.Count() > 0)
                //        return BadRequest("PO Detail(s) is already processed. PO is not editable");
                //}

                foreach (PoDetail poDetail in po.poDetails)
                {
                    if (poDetail.Id != 0)
                    {
                        var part = parts.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                        //var part = parts.Where(x => x.partSupplierAssignments.Select(p => p.PartID == poDetail.PartId && p.SupplierID == po.SupplierId).FirstOrDefault()).FirstOrDefault();
                        if (part == null)
                        {
                            part = parts.Where(p => p.Id == poDetail.PartId).FirstOrDefault();
                            if (part == null)
                                return BadRequest(string.Format("Invalid part"));

                        }

                        var existingPartDetail = existingPo.poDetails.Where(x => x.Id == poDetail.Id).FirstOrDefault();
                        if(existingPartDetail.PartId != poDetail.PartId)
                            return BadRequest(string.Format("Invalid part associated in edited po line number"));

                        if(existingPartDetail.IsClosed)
                        {
                            if(poDetail.Qty != existingPartDetail.Qty)
                                return BadRequest(string.Format("Part is already closed you can not change the qty of this part", poDetail.part.Code));
                        }
                        if (poDetail.Qty < (existingPartDetail.ReceivedQty + existingPartDetail.InTransitQty))
                            return BadRequest(string.Format("Invalid part associated in edited po line number"));
                        if(poDetail.IsForceClosed)
                        {
                            poDetail.IsClosed = true;
                        }

                        if (poDetail.Qty == (existingPartDetail.ReceivedQty + existingPartDetail.InTransitQty))
                        {
                            if(!existingPartDetail.IsClosed)
                            {
                                poDetail.IsClosed = true;
                                poDetail.IsForceClosed = true;
                            }
                        }
                    }
                    else
                    {
                        var selectedParts = parts.Where(x => x.Id == poDetail.PartId);
                        if (selectedParts == null)
                        {
                            return BadRequest(string.Format("Invalid part"));
                        }
                        var part = selectedParts.Select(x => x.partSupplierAssignments.Where(p => p.SupplierID == po.SupplierId).FirstOrDefault()).FirstOrDefault();

                        if (part == null)
                        {
                            return BadRequest(string.Format("Invalid part : {0} does not belong to this supplier", selectedParts.Select(x => x.Code).FirstOrDefault()));
                        }                        
                    }

                }

               

                po.Id = id;
                po.AccessId  = Guid.NewGuid().ToString();

                await this._poService.UpdatePoAsync(po);

                var company = await this.companyService.GetCompanyAsync(po.CompanyId);
                po.CompanyName = company.Name;

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
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this._poService.GetPoAsync(id,userId);
                if (result == null)
                {
                    return NotFound();
                }

                var existingPo = await this._poService.GetPoAsync(id,userId);
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

        [HttpPut("{acknowledge}/{id}")]
        public async Task<ActionResult<PoAccessResponse>> AcknowledgePO(int id)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var po = await this._poService.GetPoAsync(id, userId);
                if (po != null && po.IsClosed)
                    return BadRequest("PO is already closed.PO is not editable");
                if (po != null && po.poDetails != null)
                {
                    var processedPoItems = po.poDetails.Where(x => x.IsClosed || x.InTransitQty > 0 || x.ReceivedQty > 0);
                    if (processedPoItems != null && processedPoItems.Count() > 0)
                        return BadRequest("PO Detail(s) is already processed. PO is not editable");
                }


                //po.Id = id;
                string AccessId = Guid.NewGuid().ToString();
                var poAccessResponse = new PoAccessResponse();
                poAccessResponse.AccessId = AccessId;
                await this._poService.AcknowledgePoAsync(id,AccessId);

                //var company = await this.companyService.GetCompanyAsync(po.CompanyId);
                //po.CompanyName = company.Name;

                //EmailService emailService = new EmailService(_appSettings);
                //emailService.SendNotifyAcknoledgePOEmail(po.CompanyName, po.SupplierName, po.PoNo);

                return Ok(poAccessResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}