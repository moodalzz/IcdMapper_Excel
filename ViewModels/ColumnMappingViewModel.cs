using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcdMapper_Excel.ViewModels
{
    public class ColumnMappingViewModel : ViewModelBase
    {
        private string? _selectedIcdProperty;

        public string ExcelHeader { get; }

        public string? SelectedIcdProperty
        {
            get => _selectedIcdProperty;
            set => SetField(ref _selectedIcdProperty, value);
        }

        public string[] IcdProperties => Models.IcdProperties.All;

        public ColumnMappingViewModel(string excelHeader, string? initialMapping = null)
        {
            ExcelHeader = excelHeader;
            _selectedIcdProperty = initialMapping ?? "(Ignore)";
        }

        public Models.ColumnMapping ToModel() => new()
        {
            // 처음거는 Model의 ExcelHeader 두번째 ExcelHeader는 지금 ViewModel의 ExcelHeader
            ExcelHeader = ExcelHeader,
            IcdProperty = SelectedIcdProperty == "(Ignore)" ? null : SelectedIcdProperty
        };
    }
}