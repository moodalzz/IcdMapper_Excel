using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcdMapper_Excel.Models
{
    public class MappingProfile
    {
        public string ProfileName { get; set; } = "";
        public int HeaderRowIndex { get; set; } = 0;   // 0-based
        public int DataStartRow { get; set; } = 1;   // 0-based
        public List<ColumnMapping> Columns { get; set; } = new();
        public DateTime LastModified { get; set; } = DateTime.Now;
    }
}