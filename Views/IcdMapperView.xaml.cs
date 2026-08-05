using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IcdMapper_Excel.Views
{
    /// <summary>
    /// IcdMapperView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class IcdMapperView : UserControl
    {
        public IcdMapperView()
        {
            InitializeComponent();
        }

        

        private static bool IsSingleExcelFile(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return false;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            return files.Length > 0 && files.Any(IsExcelExtension);
        }

        private static bool IsExcelExtension(string filePath)
        {
            var extension = System.IO.Path.GetExtension(filePath).ToLower();
            return extension == ".xlsx" || extension == ".xls";
        }

        private void RootGrid_DragEnter(object sender, DragEventArgs e)
        {
            if (IsSingleExcelFile(e))
            {
                e.Effects = DragDropEffects.Copy;
                DropOverlay.Visibility = Visibility.Visible;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void RootGrid_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = IsSingleExcelFile(e) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void RootGrid_DragLeave(object sender, DragEventArgs e)
        {
            DropOverlay.Visibility = Visibility.Collapsed;
        }

        private void RootGrid_Drop(object sender, DragEventArgs e)
        {
            DropOverlay.Visibility = Visibility.Collapsed;

            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var excelFile = files.FirstOrDefault(IsExcelExtension);
            if (excelFile is null) return;

            if (DataContext is ViewModels.IcdMapperViewModel vm)
            {
                vm.LoadExcelFile(excelFile);
            }
        }
    }
}
