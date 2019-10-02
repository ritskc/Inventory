﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;
using System.IO;
using System.Net.Http;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IPackingSlipService packingSlipService;
        private readonly ISupplierInvoiceService supplierInvoice;
        public FileController(IPackingSlipService packingSlipService,
            ISupplierInvoiceService supplierInvoice)
        {
            this.packingSlipService = packingSlipService;
            this.supplierInvoice = supplierInvoice;
        }
        [HttpPost("{docType}/{id}")]
        public async Task<IActionResult> Post(string docType,int id, IFormFile file)
        {
            
            long size = file.Length;
            string type = string.Empty;
            switch (docType)
            {
                case "POS":
                    type = "POS";
                    break;

                case "Invoice":
                    type = "Invoice";
                    break;

                case "PackingSlip":
                    type = "PackingSlip";
                    break;

                case "TenPlus":
                    type = "TenPlus";
                    break;

                case "BL":
                    type = "BL";
                    break;

                default:
                    return BadRequest();                    
            }
            // full path to file in temp location
            //var filePath = Path.GetTempFileName();            
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),"Docs", type, id.ToString() + ".pdf");

            if (file.Length > 0)
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            var relativeFilePath = "Docs\\" + type + "\\" + id.ToString() + ".pdf";
            switch (docType)
            {
                case "POS":
                                       
                    var result = packingSlipService.UpdatePOSAsync(id, relativeFilePath);
                    break;               

                default:
                    var result1 = supplierInvoice.UploadFileAsync(id, type, relativeFilePath);
                    break;
            }

            

            return Ok();
        }

        
        //public static async Task<byte[]> DownloadFile(string url)
        //{
        //    using (var client = new HttpClient())
        //    {

        //        using (var result = await client.GetAsync(url))
        //        {
        //            if (result.IsSuccessStatusCode)
        //            {
        //                return await result.Content.ReadAsByteArrayAsync();
        //            }

        //        }
        //    }
        //    return null;
        //}

        //// GET: api/Todo
        //[HttpGet("{docType}/{id}")]
        //public async Task<ActionResult<byte[]>> Get(string docType,string id)
        //{
        //    string url = @"http://localhost:44390/Docs/BL/1.pdf";
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {

        //            using (var result = await client.GetAsync(url))
        //            {
        //                if (result.IsSuccessStatusCode)
        //                {
        //                    return await result.Content.ReadAsByteArrayAsync();
        //                }
        //                else
        //                    return null;

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.ToString());
        //    }

        //}

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