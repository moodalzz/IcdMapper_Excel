using IcdMapper_Excel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcdMapper_Excel.Services.Interfaces
{
    public interface INavigationService
    {
        ViewModelBase? CurrentViewModel { get; }
        
        event Action? CurrentViewModelChanged;

        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;

        void NavigateTo(Type viewModelType);
    }
}
