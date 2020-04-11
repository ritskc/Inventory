using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserReport
    {
        public int Id { get; set; }
        public int ReportId { get; set; }        
        public string ColumnName { get; set; }
        public string ColumnDisplayName { get; set; }
        public bool IsVisible { get; set; }
    }
}
