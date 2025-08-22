
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;

namespace MauiBankingExercise.ViewModels
{
    [QueryProperty(nameof(CustomerId), "customerId")]
    public partial class CustomerDashboardViewModel : BaseViewModel
    {
        private readonly IBankingService _bankingService;

        [ObservableProperty]
        private Customer _customer = new();

        [ObservableProperty]
        private ObservableCollection<Account> _accounts = new();

        [ObservableProperty]
        private int _customerId;

        public CustomerDashboardViewModel(IBankingService bankingService)
        {
            _bankingService = bankingService;
            Title = "Customer Dashboard";
        }

        partial void OnCustomerIdChanged(int value)
        {
            LoadCustomerData();
        }

        [RelayCommand]
        private void LoadCustomerData()
        {
            if (CustomerId == 0) return;

            try
            {
                IsLoading = true;
                Customer = _bankingService.GetCustomerWithAccounts(CustomerId);
                Accounts = new ObservableCollection<Account>(Customer.Accounts ?? new List<Account>());
                Title = $"{Customer.FirstName} {Customer.LastName}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ViewAccount(Account account)
        {
            if (account == null) return;

            await Shell.Current.GoToAsync($"//transactions?accountId={account.AccountId}");
        }
    }
}