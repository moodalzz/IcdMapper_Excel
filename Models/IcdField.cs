using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcdMapper_Excel.Models
{
    public class IcdField
    {
        public int Number { get; set; }
        public string Type { get; set; } = "";
        public int? TypeSize { get; set; }
        public int? ByteIndex { get; set; }

        public string Name { get; set; } = "";

        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? OffSet { get; set; }
        public double? Resolution { get; set; }
        public string? Unit { get; set; }
        public string? Description { get; set; }
    }

    public class ColumnMapping
    {
        public string ExcelHeader { get; set; } = "";
        public string? IcdProperty { get; set; }
    }

    public static class IcdProperties
    {
        public static readonly string[] All =
        [
            "(무시)",
        nameof(IcdField.Number),
        nameof(IcdField.Type),
        nameof(IcdField.TypeSize),
        nameof(IcdField.ByteIndex),
        nameof(IcdField.Name),
        nameof(IcdField.Min),
        nameof(IcdField.Max),
        nameof(IcdField.OffSet),
        nameof(IcdField.Resolution),
        nameof(IcdField.Unit),
        nameof(IcdField.Description),
    ];
    }
}