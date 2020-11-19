using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MobilePosController : ControllerBase
    {
        private readonly IPoService _poService;        

        public MobilePosController(IPoService poService)
        {
            this._poService = poService;           
        }

        // GET: api/Todo
        [HttpGet("{companyId}")]
        public async Task<ActionResult<IEnumerable<Po>>> Get(int companyId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this._poService.GetAllOpenPosAsync(companyId, userId);

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

        // GET: api/Todo
        [HttpGet("{companyId}/{supplierId}")]
        public async Task<ActionResult<IEnumerable<Po>>> Get(int companyId, int supplierId)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                int userId = Convert.ToInt32(claimsIdentity.FindFirst(ClaimTypes.Name)?.Value);

                var result = await this._poService.GetAllPosAsync(companyId, userId, supplierId);

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
    }
}