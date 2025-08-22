using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;

namespace MauiBankingExercise.ViewModels
{
    [QueryProperty(nameof(AccountId), "accountId")]
    public partial class TransactionViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private Account _account;
        private ObservableCollection<Transaction> _transactions = new();
        private decimal _transactionAmount;
        private string _selectedTransactionType = "Deposit";
        private bool _isLoading;
        private int _accountId;

        public TransactionViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            SubmitTransactionCommand = new Command(async () => await SubmitTransactionAsync(), CanSubmitTransaction);
            TransactionTypes = new List<string> { "Deposit", "Withdrawal" };
        }

        public int AccountId
        {
            get => _accountId;
            set
            {
                SetProperty(ref _accountId, value);
                if (value > 0)
                {
                    Task.Run(async () => await LoadAccountDataAsync());
                }
            }
        }

        public Account Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value);
        }

        public decimal TransactionAmount
        {
            get => _transactionAmount;
            set
            {
                SetProperty(ref _transactionAmount, value);
                ((Command)SubmitTransactionCommand).ChangeCanExecute();
            }
        }

        public string SelectedTransactionType
        {
            get => _selectedTransactionType;
            set
            {
                SetProperty(ref _selectedTransactionType, value);
                ((Command)SubmitTransactionCommand).ChangeCanExecute();
            }
        }

        public List<string> TransactionTypes { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand SubmitTransactionCommand { get; }

        private async Task LoadAccountDataAsync()
        {
            IsLoading = true;
            try
            {
                Account = await _databaseService.GetAccountAsync(AccountId);
                var transactions = await _databaseService.GetTransactionsAsync(AccountId);

                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading account data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanSubmitTransaction()
        {
            return TransactionAmount > 0 && !string.IsNullOrEmpty(SelectedTransactionType);
        }

        private async Task SubmitTransactionAsync()
        {
            if (SelectedTransactionType == "Withdrawal" && TransactionAmount > Account.AccountBalance)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Insufficient funds for withdrawal.", "OK");
                return;
            }

            IsLoading = true;
            try
            {
                var success = await _databaseService.CreateTransactionAsync(AccountId, TransactionAmount, SelectedTransactionType);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Transaction completed successfully!", "OK");
                    TransactionAmount = 0;
                    await LoadAccountDataAsync(); // Refresh data
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Transaction failed. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Transaction failed: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}