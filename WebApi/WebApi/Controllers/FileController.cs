﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using WebApi.Utils;
using WebApi.Settings;
using Microsoft.Extensions.Options;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IPackingSlipService packingSlipService;
        private readonly IMasterPackingSlipService masterPackingSlipService;
        private readonly ISupplierInvoiceService supplierInvoice;
        private readonly IOrderService orderService;
        private readonly ILogger<FileController> logger;
        private readonly AppSettings _appSettings;
        private readonly ICompanyService companyService;

       

        public FileController(IPackingSlipService packingSlipService,
            IMasterPackingSlipService masterPackingSlipService,
            ISupplierInvoiceService supplierInvoice,
            IOrderService orderService,
            ILogger<FileController> logger,
            IOptions<AppSettings> appSettings,
            ICompanyService companyService)
        {
            this.packingSlipService = packingSlipService;
            this.masterPackingSlipService = masterPackingSlipService;
            this.supplierInvoice = supplierInvoice;
            this.orderService = orderService;
            this.logger = logger;
            this.companyService = companyService;
            this._appSettings = appSettings.Value;
        }

        [HttpPost("{docType}/{id}")]
        public async Task<IActionResult> Post(string docType, int id, IFormFile file)
        {
            logger.LogInformation("post file api called");

            long size = file.Length;
            string type = string.Empty;
            switch (docType.ToLower())
            {               
                case "invoice":
                    type = "Invoice";
                    break;

                case "packingslip":
                    type = "PackingSlip";
                    break;

                case "tenplus":
                    type = "TenPlus";
                    break;

                case "bl":
                    type = "BL";
                    break;

                case "customerorder":
                    type = "CustomerOrder";
                    break;

                default:
                    return BadRequest();
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Docs", type, id.ToString() + ".pdf");

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            var relativeFilePath = "Docs\\" + type + "\\" + id.ToString() + ".pdf";
            if(type == "CustomerOrder")
                await orderService.UpdateOrderAsync(id, relativeFilePath);
            else            
                await supplierInvoice.UploadFileAsync(id, type, relativeFilePath);

            return Ok();
        }
        //docType = POS / MasterPOS
        [HttpPost("{docType}/{id}/{trackingNumber}")]
        public async Task<IActionResult> Post(string docType, int id, IFormFile file, string trackingNumber)
        {
            logger.LogInformation("post file api called");

            long size = file.Length;
            string type = docType;
            
            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Docs", type, id.ToString() + ".pdf");

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            var relativeFilePath = "Docs\\" + type + "\\" + id.ToString() + ".pdf";
            if (docType.ToLower() == "POS".ToLower())
            {
                var accessId = Guid.NewGuid().ToString();
                await packingSlipService.UpdatePOSAsync(id, relativeFilePath, trackingNumber, accessId);

                EmailService emailService = new EmailService(_appSettings);
                var packingSlip = await packingSlipService.GetPackingSlipAsync(id);
                var company = await companyService.GetCompanyAsync(packingSlip.CompanyId);
                emailService.SendPOSEmail(company.Name, packingSlip.CustomerDetail.ContactPersonName,  _appSettings.CustomerAccessURL + docType + "/" +  accessId, packingSlip.PackingSlipNo, packingSlip.CustomerDetail.EmailAddress);

            }
            else if (docType.ToLower() == "MasterPOS".ToLower())
            {
                var accessId = Guid.NewGuid().ToString();
                await masterPackingSlipService.UpdatePOSAsync(id, relativeFilePath, trackingNumber, accessId);
                EmailService emailService = new EmailService(_appSettings);
                var packingSlip = await masterPackingSlipService.GetMasterPackingSlipAsync(id);
                var company = await companyService.GetCompanyAsync(packingSlip.CompanyId);
                emailService.SendPOSEmail(company.Name, packingSlip.PackingSlips.FirstOrDefault().CustomerDetail.ContactPersonName, _appSettings.CustomerAccessURL + docType + "/" + accessId, packingSlip.MasterPackingSlipNo, packingSlip.PackingSlips.FirstOrDefault().CustomerDetail.EmailAddress);

            }

            return Ok();
        }        

        // GET: api/Todo
        [HttpGet("{docType}/{id}")]
        public async Task<IActionResult> Download(string docType,string id)
        {
            string filename = id+".pdf";
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "docs", docType, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}