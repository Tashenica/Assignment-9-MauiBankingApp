using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBankingExercise.Models;
using MauiBankingExercise.Services;
using System.Collections.ObjectModel;

namespace MauiBankingExercise.ViewModels
{
    [QueryProperty(nameof(AccountId), "accountId")]
    public partial class TransactionViewModel : BaseViewModel
    {
        private readonly IBankingService _bankingService;

        [ObservableProperty]
        private Account _account = new();

        [ObservableProperty]
        private ObservableCollection<Transaction> _transactions = new();

        [ObservableProperty]
        private ObservableCollection<TransactionType> _transactionTypes = new();

        [ObservableProperty]
        private decimal _transactionAmount;

        [ObservableProperty]
        private TransactionType? _selectedTransactionType;

        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private int _accountId;

        public TransactionViewModel(IBankingService bankingService)
        {
            _bankingService = bankingService;
            Title = "Transactions";
        }

        partial void OnAccountIdChanged(int value)
        {
            LoadData();
        }

        [RelayCommand]
        private void LoadData()
        {
            if (AccountId == 0) return;

            try
            {
                IsLoading = true;
                Account = _bankingService.GetAccount(AccountId);
                var transactions = _bankingService.GetAccountTransactions(AccountId);
                Transactions = new ObservableCollection<Transaction>(transactions);

                var transactionTypes = _bankingService.GetTransactionTypes();
                TransactionTypes = new ObservableCollection<TransactionType>(transactionTypes);

                Title = $"Account: {Account.AccountNumber}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void CreateTransaction()
        {
            if (!ValidateTransaction()) return;

            try
            {
                IsLoading = true;

                var transaction = new Transaction
                {
                    AccountId = AccountId,
                    TransactionTypeId = SelectedTransactionType!.TransactionTypeId,
                    Amount = TransactionAmount,
                    Description = Description,
                    TransactionDate = DateTime.Now
                };

                var success = _bankingService.CreateTransaction(transaction);
                if (success)
                {
                    // Update account balance
                    var newBalance = SelectedTransactionType.Name == "Deposit"
                        ? Account.AccountBalance + TransactionAmount
                        : Account.AccountBalance - TransactionAmount;

                    _bankingService.UpdateAccountBalance(AccountId, newBalance);

                    // Reset form and reload data
                    TransactionAmount = 0;
                    Description = string.Empty;
                    SelectedTransactionType = null;
                    LoadData();
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidateTransaction()
        {
            if (TransactionAmount <= 0)
            {
                // Show error message
                return false;
            }

            if (SelectedTransactionType == null)
            {
                // Show error message
                return false;
            }

            if (SelectedTransactionType.Name == "Withdrawal" && TransactionAmount > Account.AccountBalance)
            {
                // Show error message - insufficient funds
                return false;
            }

            return true;
        }
    }
}