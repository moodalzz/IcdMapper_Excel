using IcdMapper_Excel.Models;
using IcdMapper_Excel.Services.Interfaces;
using OfficeOpenXml;
using System.IO;
using System.Security.Cryptography;

namespace IcdMapper_Excel.Services
{
    public class ExcelReaderService : IExcelReaderService
    {
        public ExcelReaderService()
        {
            ExcelPackage.License.SetNonCommercialPersonal("IcdMapper_Excel");
        }

        public List<string> ReadSheetNames(string filePath)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            return package.Workbook.Worksheets.Select(ws => ws.Name).ToList();
        }

        public List<string> ReadHeaders(string filePath, int headerRowIndex = 0, int sheetIndex = 0)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var sheet = package.Workbook.Worksheets[sheetIndex];
            int colCount = sheet.Dimension?.Columns ?? 0;
            int excelRow = headerRowIndex + 1;
            var headers = new List<string>();
            for (int i = 0; i <= colCount; i++)
            {
                var val = sheet.Cells[excelRow, i].Text.Trim();
                headers.Add(string.IsNullOrEmpty(val) ? $"Column {i}" : val);
            }
            return headers;
        }

        public List<string[]> ReadRows(string filePath, int dataStartRow = 1, int sheetIndex = 0)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var sheet = package.Workbook.Worksheets[sheetIndex];
            int rowCount = sheet.Dimension?.Rows ?? 0;
            int colCount = sheet.Dimension?.Columns ?? 0;
            var rows = new List<string[]>();
            for (int i = dataStartRow + 1; i <= rowCount; i++)
            {
                var rowValues = new string[colCount];
                for (int j = 1; j <= colCount; j++)
                {
                    rowValues[j - 1] = sheet.Cells[i, j].Text.Trim();
                }
                if (rowValues.All(string.IsNullOrWhiteSpace)) continue;
                rows.Add(rowValues);
            }
            return rows;
        }

        public List<IcdField> ToIcdFields(string filePath, MappingProfile profile, int sheetIndex = 0)
        {
            var rows = ReadRows(filePath, profile.DataStartRow, sheetIndex);
            var fields = new List<IcdField>();
            foreach (var row in rows)
            {
                var field = new IcdField();
                for (int i = 0; i < profile.Columns.Count && i < row.Length; i++)
                {
                    var map = profile.Columns[i];
                    if (string.IsNullOrEmpty(map.IcdProperty) || map.IcdProperty == "(Ignore)") continue;
                    var raw = row[i];
                    ApplyValue(field, map.IcdProperty, raw);
                }
                if (!string.IsNullOrWhiteSpace(field.Name))
                {
                    fields.Add(field);
                }
            }
            return fields;
        }

        private static void ApplyValue(IcdField field, string property, string raw)
        {
            switch (property)
            {
                case nameof(IcdField.Number): field.Number = ParseInt(raw) ?? 0; break;
                case nameof(IcdField.Type): field.Type = raw; break;
                case nameof(IcdField.TypeSize): field.TypeSize = ParseInt(raw); break;
                case nameof(IcdField.ByteIndex): field.ByteIndex = ParseInt(raw); break;
                case nameof(IcdField.Name): field.Name = raw; break;
                case nameof(IcdField.Min): field.Min = ParseDouble(raw); break;
                case nameof(IcdField.Max): field.Max = ParseDouble(raw); break;
                case nameof(IcdField.OffSet): field.OffSet = ParseDouble(raw); break;
                case nameof(IcdField.Resolution): field.Resolution = ParseDouble(raw); break;
                case nameof(IcdField.Unit): field.Unit = raw; break;
                case nameof(IcdField.Description): field.Description = raw; break;
            }
        }

        private static int? ParseInt(string s) => int.TryParse(s, out var v) ? v : null;

        private static double? ParseDouble(string s) => double.TryParse(s, System.Globalization.NumberStyles.Any,
                                                            System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;
    }
}