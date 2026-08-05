using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using IcdMapper_Excel.Models;
using IcdMapper_Excel.Services.Interfaces;
using Microsoft.Win32;

namespace IcdMapper_Excel.ViewModels
{
    public class IcdMapperViewModel : ViewModelBase
    {
        private readonly IExcelReaderService _excel;
        private readonly IMappingProfileService _profile;
        private readonly IJsonExportService _jsonExport;

        // -- status --
        private string _excelPath = "";

        private string _profileName = "New Profile";
        private int _headRowIndex = 0;
        private int _dataStartRow = 1;
        private string _statusMessage = "Load Excel to Extract Fields";
        private string _jsonPreview = "";
        private bool _hasFields;
        private string? _selectedSheetName;

        // -- properties -----

        public ObservableCollection<string> SheetNames { get; } = new();
        public string? SelectedSheetName
        {
            get => _selectedSheetName;
            set
            {
                if (SetField(ref _selectedSheetName, value))
                {
                    ReloadHeaders();
                }
            }
        }

        public bool HasSheets => SheetNames.Count > 0;
        public int SelectedSheetIndex
        {
            get
            {
                var idx = SelectedSheetName is null ? -1 : SheetNames.IndexOf(SelectedSheetName);
                return idx < 0 ? -1 : idx;
            }
        }

        public string ExcelPath
        {
            get => _excelPath;
            set
            {
                SetField(ref _excelPath, value);
                OnPropertyChanged(nameof(ExcelFileName));
            }
        }

        public string ExcelFileName => string.IsNullOrEmpty(_excelPath) ? "No File Selected" : System.IO.Path.GetFileName(_excelPath);

        public string ProfileName
        {
            get => _profileName;
            set => SetField(ref _profileName, value);
        }

        public int HeaderRowIndex
        {
            get => _headRowIndex;
            set
            {
                SetField(ref _headRowIndex, value);
                ReloadHeaders();
            }
        }

        public int DataStartRow { get => _dataStartRow; set => SetField(ref _dataStartRow, value); }
        public List<ColumnMapping> Columns { get; set; } = new();
        public DateTime LastModified { get; set; } = DateTime.Now;


        public string StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }
        public string JsonPreview { get => _jsonPreview; set => SetField(ref _jsonPreview, value); }
        public bool HasFields { get => _hasFields; set => SetField(ref _hasFields, value); }

        // -- collection -----
        public ObservableCollection<ColumnMappingViewModel> ColumnMappings { get; } = new();

        public ObservableCollection<Models.MappingProfile> SaveProfiles { get; } = new();

        private MappingProfile? _selectedProFile;

        public MappingProfile? SelectedProfile
        {
            get => _selectedProFile;
            set
            {
                SetField(ref _selectedProFile, value); if (value != null) ApplyProfile(value);
            }
        }

        // --Command-----
        public RelayCommand BrowseExcelCommand { get; }

        public RelayCommand ConvertCommand { get; }
        public RelayCommand SaveProfileCommand { get; }
        public RelayCommand DeleteProfileCommand { get; }
        public RelayCommand ExportJsonCommand { get; }

        public IcdMapperViewModel(IExcelReaderService excel, IMappingProfileService profile, IJsonExportService json)
        {
            _excel = excel;
            _profile = profile;
            _jsonExport = json;

            BrowseExcelCommand = new RelayCommand(BrowseExcel);
            ConvertCommand = new RelayCommand(Convert, () => ColumnMappings.Any());
            SaveProfileCommand = new RelayCommand(SaveProfile, () => ColumnMappings.Any());
            DeleteProfileCommand = new RelayCommand(DeleteProfile, () => SelectedProfile != null);
            ExportJsonCommand = new RelayCommand(ExportJson, () => HasFields);
        }

        // --LoadExcel
        private void BrowseExcel()
        {
            var dlg = new OpenFileDialog { Filter = "Excel Files|*.xlsx;*.xls" };
            if (dlg.ShowDialog() != true) return;

            //ExcelPath = dlg.FileName;
            //LoadSheetName();
            LoadExcelFile(dlg.FileName);
        }

        public void LoadExcelFile(string filePath)
        {
            ExcelPath = filePath;
            LoadSheetName();
        }

        private void LoadSheetName()
        {
            SheetNames.Clear();
            try
            {
                foreach (var name in _excel.ReadSheetNames(ExcelPath))
                {
                    SheetNames.Add(name);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error loading Excel: {ex.Message}";
                OnPropertyChanged(nameof(HasSheets));
                return;
            }

            OnPropertyChanged(nameof(HasSheets));

            if (SheetNames.Count > 0)
            {
                if (_selectedSheetName == SheetNames[0])
                {
                    ReloadHeaders();
                }
                else
                {
                    SelectedSheetName = SheetNames[0];
                }
            }
            else
            {
                SelectedSheetName = null;
                ReloadHeaders();
            }
        }

        private void ReloadHeaders()
        {
            if (string.IsNullOrEmpty(ExcelPath) || SelectedSheetIndex < 0) return;
            try
            {
                var headers = _excel.ReadHeaders(ExcelPath, HeaderRowIndex, SelectedSheetIndex);
                ColumnMappings.Clear();
                foreach (var header in headers)
                {
                    ColumnMappings.Add(new ColumnMappingViewModel(header));
                }
                StatusMessage = $"Loaded {headers.Count} columns from sheet '{SelectedSheetName}'";
                HasFields = false;
                JsonPreview = "";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error reading headers: {ex.Message}";
            }
        }

        //--Transform----
        private void Convert()
        {
            try
            {
                var profile = BuildProfile();
                var fields = _excel.ToIcdFields(ExcelPath, profile, SelectedSheetIndex);
                JsonPreview = _jsonExport.Preview(fields);
                HasFields = fields.Count > 0;
                StatusMessage = $"Converted {fields.Count} fields from sheet '{SelectedSheetName}'";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error converting fields: {ex.Message}";
            }
        }

        //--Export-----
        private void ExportJson()
        {
            var dlg = new SaveFileDialog { Filter = "JSON Files|*.json", FileName = $"{ProfileName}.json" };
            if (dlg.ShowDialog() != true) return;

            try
            {
                var profile = BuildProfile();
                var fields = _excel.ToIcdFields(ExcelPath, profile, SelectedSheetIndex);
                _jsonExport.Export(fields, dlg.FileName);
                StatusMessage = $"Exported JSON to '{dlg.FileName}'";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error exporting JSON: {ex.Message}";
            }
        }

        //--Profile Save/Delete-----

        private void SaveProfile()
        {
            if (string.IsNullOrWhiteSpace(ProfileName))
            {
                StatusMessage = "Profile name cannot be empty.";
                return;
            }
            _profile.Save(BuildProfile());
            RefreshProfiles();
            StatusMessage = $"Profile '{ProfileName}' saved.";
        }

        private MappingProfile BuildProfile() => new()
        {
            ProfileName = ProfileName,
            HeaderRowIndex = HeaderRowIndex,
            DataStartRow = DataStartRow,
            Columns = ColumnMappings.Select(vm => vm.ToModel()).ToList()
        };

        private void RefreshProfiles()
        {
            SaveProfiles.Clear();
            foreach (var profile in _profile.LoadAll())
            {
                SaveProfiles.Add(profile);
            }
        }

        private void DeleteProfile()
        {
            if (SelectedProfile == null) return;
            if(MessageBox.Show($"Are you sure you want to delete the profile '{SelectedProfile.ProfileName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            _profile.Delete(SelectedProfile.ProfileName);
            RefreshProfiles();
            StatusMessage = $"Profile '{SelectedProfile.ProfileName}' deleted.";
        } 

        private void ApplyProfile(MappingProfile profile)
        {
            ProfileName = profile.ProfileName;
            HeaderRowIndex = profile.HeaderRowIndex;
            DataStartRow = profile.DataStartRow;

            if (!ColumnMappings.Any()) return;


            foreach (var mapping in ColumnMappings)
            {
                var saved = profile.Columns.FirstOrDefault(c => c.ExcelHeader == mapping.ExcelHeader);
                mapping.SelectedIcdProperty = saved?.IcdProperty;
            }
            StatusMessage = $"Profile '{profile.ProfileName}' applied.";
        }
    }
}