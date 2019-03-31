﻿using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IPoRepository
    {
        Task<IEnumerable<Po>> GetAllPosAsync(int companyId);
        Task<Po> GetPoAsync(long poId);
        Task AddPoAsync(Po po);
        Task UpdatePoAsync(Po po);
        Task<int> DeletePoAsync(long poId);
    }
}
