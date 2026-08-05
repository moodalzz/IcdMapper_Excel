using IcdMapper_Excel.Services.Interfaces;
using IcdMapper_Excel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcdMapper_Excel.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewModelBase? CurrentViewModel { get; set; }

        public event Action? CurrentViewModelChanged;
        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase => NavigateTo(typeof(TViewModel));

        public async void NavigateTo(Type viewModelType)
        {
            var viewModel = _serviceProvider.GetService(viewModelType) as ViewModelBase ?? throw new InvalidOperationException($"ViewModel of type {viewModelType.Name} is not registered in the service provider.");
            if(viewModel is IAsyncInitialize init)
            {
                await init.InitializeAsync();
            }
            CurrentViewModel = viewModel;
            CurrentViewModelChanged?.Invoke();

        }
    }
}
