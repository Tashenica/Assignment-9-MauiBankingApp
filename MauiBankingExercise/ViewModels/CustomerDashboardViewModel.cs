using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;

namespace MauiBankingExercise.ViewModels
{
    [QueryProperty(nameof(CustomerId), "customerId")]
    public partial class CustomerDashboardViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private Customer _customer;
        private ObservableCollection<Account> _accounts = new();
        private bool _isLoading;
        private int _customerId;

        public CustomerDashboardViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            SelectAccountCommand = new Command<Account>(OnSelectAccount);
        }

        public int CustomerId
        {
            get => _customerId;
            set
            {
                SetProperty(ref _customerId, value);
                if (value > 0)
                {
                    Task.Run(async () => await LoadCustomerDataAsync());
                }
            }
        }

        public Customer Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        public ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SelectAccountCommand { get; }

        private async Task LoadCustomerDataAsync()
        {
            IsLoading = true;
            try
            {
                Customer = await _databaseService.GetCustomerAsync(CustomerId);
                var accounts = await _databaseService.GetAccountsAsync(CustomerId);

                Accounts.Clear();
                foreach (var account in accounts)
                {
                    Accounts.Add(account);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading customer data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void OnSelectAccount(Account account)
        {
            if (account != null)
            {
                await Shell.Current.GoToAsync($"TransactionPage?accountId={account.AccountId}");
            }
        }
    }
}