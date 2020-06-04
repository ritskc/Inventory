using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.IServices;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerAccessController : ControllerBase
    {
        private readonly IPackingSlipService packingSlipService;
        private readonly IMasterPackingSlipService masterPackingSlipService;
        public CustomerAccessController(IPackingSlipService packingSlipService, IMasterPackingSlipService masterPackingSlipService)
        {
            this.packingSlipService = packingSlipService;
            this.masterPackingSlipService = masterPackingSlipService;
        }
        [HttpGet("{docType}/{accessId}")]
        public async Task<IActionResult> Download(string docType, string accessId)
        {
            int id =0;
            if (docType.ToLower() == "POS".ToLower())
            {
                id = await this.packingSlipService.GetIdByAccessIdAsync(accessId);
            }
            else if (docType.ToLower() == "MasterPOS".ToLower())
            {
                id = await this.masterPackingSlipService.GetIdByAccessIdAsync(accessId);
            }
            string filename = id.ToString() + ".pdf";
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