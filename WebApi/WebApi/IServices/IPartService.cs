using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.IServices
{
    public interface IPartService
    {
        IEnumerable<Part> GetAllParts();
        Part GetPart(long id);
        void AddPart(Part part);
        void UpdatePart(Part part);
        void DeletePart(long id);
    }
}
