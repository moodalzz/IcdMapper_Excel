using IcdMapper_Excel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcdMapper_Excel.Services.Interfaces
{
    public interface IExcelReaderService
    {
        List<string> ReadSheetNames(string filePath);

        List<string> ReadHeaders(string filePath, int headerRowIndex = 0, int sheetIndex = 0);

        List<string[]> ReadRows(string filePath, int dataStartRow = 1, int sheetIndex = 0);

        List<IcdField> ToIcdFields(string filePath, MappingProfile profile, int sheetIndex = 0);
    }
}