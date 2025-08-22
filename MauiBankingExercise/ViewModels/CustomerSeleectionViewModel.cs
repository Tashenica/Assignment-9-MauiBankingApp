using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;

namespace MauiBankingExercise.ViewModels
{
    public partial class CustomerSelectionViewModel : BaseViewModel
    {
        private readonly IBankingService _bankingService;

        [ObservableProperty]
        private ObservableCollection<Customer> _customers = new();

        public CustomerSelectionViewModel(IBankingService bankingService)
        {
            _bankingService = bankingService;
            Title = "Select Customer";
            LoadCustomers();
        }

        [RelayCommand]
        private void LoadCustomers()
        {
            try
            {
                IsLoading = true;
                var customers = _bankingService.GetCustomers();
                Customers = new ObservableCollection<Customer>(customers);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SelectCustomer(Customer customer)
        {
            if (customer == null) return;

            await Shell.Current.GoToAsync($"//dashboard?customerId={customer.CustomerId}");
        }
    }
}