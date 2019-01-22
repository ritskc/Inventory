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
        public void AddPart(Part part)
        {
            throw new NotImplementedException();
        }

        public void DeletePart(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Part> GetAllParts()
        {
            List<Part> parts = new List<Part>();
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

        public Part GetPart(long id)
        {
            throw new NotImplementedException();
        }

        public void UpdatePart(Part part)
        {
            throw new NotImplementedException();
        }
    }
}
