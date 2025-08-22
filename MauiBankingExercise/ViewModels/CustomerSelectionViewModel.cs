using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;

namespace MauiBankingExercise.ViewModels
{
    public partial class CustomerSelectionViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Customer> _customers = new();
        private bool _isLoading;

        public CustomerSelectionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            SelectCustomerCommand = new Command<Customer>(OnSelectCustomer);
        }

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SelectCustomerCommand { get; }

        public async Task LoadCustomersAsync()
        {
            IsLoading = true;
            try
            {
                var customers = await _databaseService.GetCustomersAsync();
                Customers.Clear();
                foreach (var customer in customers)
                {
                    Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                // Handle error
                System.Diagnostics.Debug.WriteLine($"Error loading customers: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void OnSelectCustomer(Customer customer)
        {
            if (customer != null)
            {
                await Shell.Current.GoToAsync($"CustomerDashboard?customerId={customer.CustomerId}");
            }
        }
    }
}