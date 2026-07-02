using System.IO;
using System.Text.Json;

using IcdMapper_Excel.Services.Interfaces;
using IcdMapper_Excel.Models;

namespace IcdMapper_Excel.Services
{
    public class JasonExportService : IJsonExportService
    {
        private static readonly JsonSerializerOptions _opts = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public void Export(List<IcdField> fields, string outputPath)
        {
            File.WriteAllText(outputPath, JsonSerializer.Serialize(fields, _opts));
        }

        public string Preview(List<IcdField> fields)
        {
            return JsonSerializer.Serialize(fields, _opts);
        }
    }
}