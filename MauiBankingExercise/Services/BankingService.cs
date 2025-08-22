using MauiBankingExercise.Models;

namespace MauiBankingExercise.Services
{
    public class BankingService : IBankingService
    {
        private readonly DatabaseService _databaseService;

        public BankingService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public List<Customer> GetCustomers()
        {
            return _databaseService.GetCustomers();
        }

        public Customer GetCustomerWithAccounts(int customerId)
        {
            var customer = _databaseService.GetCustomerWithAccounts(customerId);
            customer.Accounts = _databaseService.GetCustomerAccounts(customerId);
            return customer;
        }

        public Account GetAccount(int accountId)
        {
            return _databaseService.GetAccount(accountId);
        }

        public List<Transaction> GetAccountTransactions(int accountId)
        {
            return _databaseService.GetAccountTransactions(accountId);
        }

        public List<TransactionType> GetTransactionTypes()
        {
            return _databaseService.GetTransactionTypes();
        }

        public bool CreateTransaction(Transaction transaction)
        {
            try
            {
                transaction.TransactionDate = DateTime.Now;
                var result = _databaseService.SaveTransaction(transaction);
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateAccountBalance(int accountId, decimal newBalance)
        {
            try
            {
                var account = _databaseService.GetAccount(accountId);
                if (account != null)
                {
                    account.AccountBalance = newBalance;
                    var result = _databaseService.UpdateAccount(account);
                    return result > 0;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}