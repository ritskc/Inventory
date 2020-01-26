using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.IRepository
{
    public interface IReportRepository
    {
        List<PackingSlipReport> GetPackingSlipReport(long id);
        List<PackingSlipReport> GetMasterPackingSlipReport(long id);
    }
}
