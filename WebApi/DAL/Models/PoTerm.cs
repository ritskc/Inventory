using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class PoTerm
    {
        public long Id { get; set; }
        public long PoId { get; set; }
        public int SequenceNo { get; set; }
        public string Term { get; set; }
    }
}
