using MauiBankingExercise.Models;

namespace MauiBankingExercise.Services
{
    public interface IBankingService
    {
        List<Customer> GetCustomers();
        Customer GetCustomerWithAccounts(int customerId);
        List<Transaction> GetAccountTransactions(int accountId);
        List<TransactionType> GetTransactionTypes();
        bool CreateTransaction(Transaction transaction);
        bool UpdateAccountBalance(int accountId, decimal newBalance);
        Account GetAccount(int accountId);
    }
}