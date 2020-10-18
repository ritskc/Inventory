using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Newtonsoft.Json;
using WebApi.IServices;

namespace WebApi.Controllers
{
    //[Authorize]
    [Route("[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartService _partService;

        public PartsController(IPartService partService)
        {
            this._partService = partService;
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<Part>>> GetParts(int companyId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
                var result = await this._partService.GetAllPartsAsync(companyId,userId);

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
        public async Task<ActionResult<Part>> Get(int companyId, string id)
        {
            try
            {
                if (id.ToString() == "stock")
                {
                    var result1 = await this._partService.GetAllPartsStocksAsync(companyId);
                    return Ok(result1);
                }

                else
                {
                    var result = await this._partService.GetPartAsync(Convert.ToInt64(id));

                    if (result == null)
                    {
                        return NotFound();
                    }

                    return result;
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }


        
        [HttpGet("{companyId}/{type}/{typeId}")]
        // [HttpGet("{companyId}/{type}/{partId}")]
        public async Task<ActionResult> GetPartsByType(int companyId,string type, string typeId)
        {
            try
            {

                if (type == "customer")
                {
                    var result = await this._partService.GetPartByCustomerIdAsync(Convert.ToInt32(typeId));
                    return Ok(result);
                }
                else if (type == "supplier")
                {
                    var result = await this._partService.GetPartBySupplierIdAsync(Convert.ToInt32(typeId));
                    return Ok(result);
                }
                if (type.ToLower() == "InTransit".ToLower())
                {
                    var result = await this._partService.GetPartInTransitDetailAsync(Convert.ToInt32(typeId), companyId);
                    return Ok(result);
                }
                else if (type.ToLower() == "OpenOrder".ToLower())
                {
                    var result = await this._partService.GetPartOpenOrderDetailAsync(Convert.ToInt32(typeId), companyId);
                    return Ok(result);
                }
                else if (type.ToLower() == "SupplierOpenPO".ToLower())
                {
                    var result = await this._partService.GetPartOpenPODetailAsync(Convert.ToInt32(typeId), companyId);
                    return Ok(result);
                }
                else if (type.ToLower() == "LatestShipment".ToLower())
                {
                    var result = await this._partService.GetPartLatestShipmentAsync(Convert.ToInt32(typeId), companyId);
                    return Ok(result);
                }
                else if (type.ToLower() == "LatestReceived".ToLower())
                {
                    var result = await this._partService.GetPartLatestReceivedAsync(Convert.ToInt32(typeId), companyId);
                    return Ok(result);
                }
                else if (type.ToLower() == "WarehouseInventory".ToLower())
                {
                    var result = await this._partService.GetPartWarehouseInventoryAsync(Convert.ToInt32(typeId), companyId);
                    return Ok(result);
                }
                else if (type.ToLower() == "StockWithPrice".ToLower())
                {
                    var result = await this._partService.GetStock(Convert.ToInt32(typeId), companyId);
                    return Ok(result);
                }
                else if (type.ToLower() == "inventory".ToLower())
                {
                    var claimsIdentity = this.User.Identity as ClaimsIdentity;
                    int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
                    var result = await this._partService.GetAllPartsByDateAsync(companyId, userId, Convert.ToDateTime(typeId)); 
                    return Ok(result);
                }
                if (type.ToLower() == "warehouse".ToLower())
                {
                    var claimsIdentity = this.User.Identity as ClaimsIdentity;
                    int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);
                    var result = await this._partService.GetAllPartsbyWarehouseAsync( companyId, userId,Convert.ToInt32(typeId));
                    return Ok(result);
                }

                else
                    return BadRequest();
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }

        }        

        //// PUT api/values/5
        //[HttpGet("{companyId}/{type}/{partId}")]
        //public async Task<IActionResult> Get(int companyId, string type, int partId)
        //{
        //    try
        //    {
        //        if (type.ToLower() == "InTransit".ToLower())
        //        {
        //            var result = await this._partService.GetPartInTransitDetailAsync(partId,companyId);
        //            return Ok(result);
        //        }
        //        else if (type.ToLower() == "OpenOrder".ToLower())
        //        {
        //            var result = await this._partService.GetPartOpenOrderDetailAsync(partId, companyId);
        //            return Ok(result);
        //        }
        //        else if (type.ToLower() == "LatestShipment".ToLower())
        //        {
        //            var result = await this._partService.GetPartLatestShipmentAsync(partId, companyId);
        //            return Ok(result);
        //        }
        //        return BadRequest();

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }
        //}

        // POST api/values
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Part part)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                if (part == null)
                    return StatusCode(500,"invalid part");
                if (string.IsNullOrEmpty(part.Code) || string.IsNullOrEmpty(part.Description))
                    return StatusCode(500, "invalid partcode / description");
                if(part.partSupplierAssignments == null || part.partSupplierAssignments.Count() < 1 ||
                    part.partCustomerAssignments == null || part.partCustomerAssignments.Count() < 1)
                    return StatusCode(500, "invalid part - Atleast one supplier and Customer requires to create a part");
                var parts = await this._partService.GetAllPartsAsync(part.CompanyId,userId);
                if (parts.Where(x => x.Code == part.Code).Count() > 0)
                    return StatusCode(302);

                if (part.partSupplierAssignments.Any(x=>x.MapCode == null || x.MapCode.Trim() == string.Empty))
                    return StatusCode(500, "invalid Mapcode - mapcode can not be empty");

                if (part.partCustomerAssignments.Any(x => x.MapCode == null || x.MapCode.Trim() == string.Empty))
                    return StatusCode(500, "invalid Mapcode - mapcode can not be empty");

                await this._partService.AddPartAsync(part);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Part part)
        {
            try
            {
                if (id != part.Id)
                {
                    return BadRequest();
                }

                if (part == null)
                    return StatusCode(500, "invalid part");
                if (string.IsNullOrEmpty(part.Code) || string.IsNullOrEmpty(part.Description))
                    return StatusCode(500, "invalid partcode / description");
                if (part.partSupplierAssignments == null || part.partSupplierAssignments.Count() < 1 ||
                    part.partCustomerAssignments == null || part.partCustomerAssignments.Count() < 1)
                    return StatusCode(500, "invalid part - Atleast one supplier and Customer requires to create a part");

                part.Id = id;
                await this._partService.UpdatePartAsync(part);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id, [FromBody] List<StockPrice> stockPrices)
        {
            try
            {                
                await this._partService.SetStockPriceAsync(id,stockPrices);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("{import}/{companyId}")]
        public async Task<IActionResult> Put(int companyId, [FromBody] List<StockPrice> stockPrices)
        {
            try
            {
                await this._partService.SetStockPriceAsync(stockPrices, companyId);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }



        // PUT api/values/5
        [HttpPut("{companyId}/{type}/{typeName}/{partCode}/{rate}")]
        public async Task<IActionResult> Put(int companyId, string type, string typeName, string partCode, decimal rate)
        {
            try
            {               
                if(type == "customer")
                    await this._partService.UpdatePartCustomerPriceAsync(companyId,typeName, partCode, rate);
                else if(type== "supplier" )
                    await this._partService.UpdatePartSupplierPriceAsync(companyId,typeName, partCode, rate);
                else
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{companyId}/{type}/{partId}/{qty}")]
        public async Task<IActionResult> Put(int companyId, string type, int partId, int qty)
        {
            try
            {
                if (type.ToLower() == "OpeningQty".ToLower())
                    await this._partService.UpdateOpeningQtyByPartIdAsync(companyId, partId, qty);
                else if (type.ToLower() == "MonthlyOpeningQty".ToLower())
                    await this._partService.UpdateMonthlyOpeningQtyByPartIdAsync(companyId, partId, qty);
                else if (type.ToLower() == "QtyInHand".ToLower())
                    await this._partService.UpdateQtyInHandByPartIdAsync(companyId, partId, qty);
                else
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPost("{companyId}/{partId}/{type}/{direction}/{qty}/{note}")]
        public async Task<IActionResult> Put(int companyId, string type, int partId, int qty,string direction, string note)
        {
            try
            {  if(type == "monthly")
                {
                    if (direction.ToLower() == BusinessConstants.DIRECTION.IN.ToString().ToLower() || direction.ToLower() == BusinessConstants.DIRECTION.OUT.ToString().ToLower())
                        await this._partService.UpdateMonthlyQtyInHandByPartIdAsync(companyId, partId, qty, direction, note);
                    return Ok();
                }
                if (direction.ToLower() == BusinessConstants.DIRECTION.IN.ToString().ToLower() || direction.ToLower() == BusinessConstants.DIRECTION.OUT.ToString().ToLower())
                    await this._partService.UpdateQtyInHandByPartIdAsync(companyId, partId, qty,direction,note);
                else
                    return BadRequest();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("")]
        public async Task<IActionResult> Put([FromBody] PartTransfer partTransfer)
        {
            try
            {
                await this._partService.TransferInventoryInternallyAsync(partTransfer);
                return Ok();               
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // PUT api/values/5
        [HttpPut("{companyId}/{partCode}/{qty}")]
        public async Task<IActionResult> Put(int companyId, string partCode, int qty)
        {
            try
            {
                await this._partService.UpdateOpeningQtyByPartCodeAsync(companyId, partCode, qty);
               
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        //DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Part>> Delete(int id)
        {
            try
            {
                var result = await this._partService.GetPartAsync(id);
                if (result == null)
                {
                    return NotFound();
                }

                await this._partService.DeletePartAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}