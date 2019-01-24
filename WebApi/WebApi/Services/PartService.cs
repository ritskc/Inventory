using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.IServices;
using WebApi.Models;

namespace WebApi.Services
{
    public class PartService : IPartService
    {
        List<Part> parts = new List<Part>();
        
        private IEnumerable<Part> GetAllParts()
        {
            if (parts != null && parts.Count() > 0)
                return parts;

            parts = new List<Part>();
            for(int i=0;i<10;i++)
            {
                Part part = new Part();
                part.Id = i + 1;
                part.Name = "Part" + i + 1;
                part.Description = "PartDesc" + i + 1;
                parts.Add(part);
            }
            return parts;
        }

        public async Task<IEnumerable<Part>> GetAllPartsAsync()
        {           
            return await Task.Run(() => GetAllParts());
        }  
        
        public async Task<Part> GetPartAsync(long id)
        {
            if (parts == null || parts.Count() == 0)
                GetAllParts();
            return await Task.Run(() => parts.Where(p => p.Id == id).FirstOrDefault());
        }       

        public async Task AddPartAsync(Part part)
        {            
            await Task.Run(() => AddPart(part));
        }

        private void AddPart(Part part)
        {
            if (parts == null)
                GetAllParts();
            part.Id = this.parts.Count() + 1;
            this.parts.Add(part);
        }

        public async Task UpdatePartAsync(Part part)
        {
            await Task.Run(() => UpdatePart(part));
        }

        private void UpdatePart(Part part)
        {
            if (parts == null)
                GetAllParts();
            var oldPart = parts.Where(p => p.Id == part.Id).FirstOrDefault();            
            this.parts.Remove(oldPart);
            this.parts.Add(part);
        }
        
        public async Task<Part> DeletePartAsync(long id)
        {            
            return await Task.Run(() => DeletePart(id));
        }

        private Part DeletePart(long id)
        {
            if (parts == null)
                GetAllParts();
            var oldPart = parts.Where(p => p.Id == id).FirstOrDefault();
            this.parts.Remove(oldPart);
            return oldPart;
        }        
    }
}
