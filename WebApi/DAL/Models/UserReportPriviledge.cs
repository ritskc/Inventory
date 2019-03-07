using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserReportPriviledge
    {
        public int Id { get; set; }
        //public int PriviledgeId { get; set; }
        public int ReportId { get; set; }
        //public string ReportName { get; set; }
        public int ReportColumnId { get; set; }
        public string ColumnName { get; set; }
        public string DisplayName { get; set; }
        public bool View { get; set; }
        public bool Edit { get; set; }
        public bool Sort { get; set; }
        public bool SortOrder { get; set; }
        public int ColumnTypeId { get; set; }
        public int ColumnSequence { get; set; }
    }
}
