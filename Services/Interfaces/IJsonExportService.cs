using IcdMapper_Excel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcdMapper_Excel.Services.Interfaces
{
    public interface IJsonExportService
    {
        void Export(List<IcdField> fields, string outputPath);

        string Preview(List<IcdField> fields);
    }
}