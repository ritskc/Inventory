using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService reportService;
        public ReportsController(IReportService reportService)
        {
            this.reportService = reportService;
        }

        [HttpGet("{reportType}/{companyId}/{fromDate}/{toDate}")]        
        public async Task<ActionResult> Get(string reportType,int companyId, DateTime fromDate, DateTime toDate)
        {
            try
            {

                if (reportType == "purchase")
                {
                    var result = await this.reportService.GetPurchaseDataAsync(companyId,fromDate,toDate);
                    return Ok(result);
                }
                if (reportType == "purchasesummary")
                {
                    var result = await this.reportService.GetPurchaseDataSummaryAsync(companyId, fromDate, toDate);
                    return Ok(result);
                }
                else if (reportType == "sales")
                {
                    var result = await this.reportService.GetSalesDataAsync(companyId, fromDate, toDate);
                    return Ok(result);
                }
                else if (reportType == "salessummary")
                {
                    var result = await this.reportService.GetSalesDataSummaryAsync(companyId, fromDate, toDate);
                    return Ok(result);
                }
                else if (reportType == "inventoryadjustment")
                {
                    var result = await this.reportService.GetInventoryAdjustmentReport(companyId, fromDate, toDate);
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
    }
}