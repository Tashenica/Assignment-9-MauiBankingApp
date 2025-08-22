using MauiBankingExercise.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;

namespace MauiBankingExercise.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public async Task InitAsync()
        {
            if (_database is not null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "Banking.db");
            _database = new SQLiteAsyncConnection(databasePath);

            await _database.CreateTableAsync<Customer>();
            await _database.CreateTableAsync<Account>();
            await _database.CreateTableAsync<Transaction>();
            await _database.CreateTableAsync<TransactionType>();

            // Use existing seeder if customers don't exist
            var customerCount = await _database.Table<Customer>().CountAsync();
            if (customerCount == 0)
            {
                await SeedDataAsync();
            }
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            await InitAsync();
            return await _database.GetAllWithChildrenAsync<Customer>();
        }

        public async Task<Customer> GetCustomerAsync(int customerId)
        {
            await InitAsync();
            return await _database.GetWithChildrenAsync<Customer>(customerId);
        }

        public async Task<List<Account>> GetAccountsAsync(int customerId)
        {
            await InitAsync();
            return await _database.Table<Account>()
                .Where(a => a.CustomerId == customerId && a.IsActive)
                .ToListAsync();
        }

        public async Task<Account> GetAccountAsync(int accountId)
        {
            await InitAsync();
            return await _database.GetAsync<Account>(accountId);
        }

        public async Task<List<Transaction>> GetTransactionsAsync(int accountId)
        {
            await InitAsync();
            var transactions = await _database.Table<Transaction>()
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            // Load transaction types
            foreach (var transaction in transactions)
            {
                transaction.TransactionType = await _database.GetAsync<TransactionType>(transaction.TransactionTypeId);
            }

            return transactions;
        }

        public async Task<bool> CreateTransactionAsync(int accountId, decimal amount, string transactionType)
        {
            await InitAsync();

            try
            {
                var account = await GetAccountAsync(accountId);
                if (account == null) return false;

                var transactionTypeEntity = await _database.Table<TransactionType>()
                    .FirstOrDefaultAsync(tt => tt.Name == transactionType);

                if (transactionTypeEntity == null) return false;

                // Validate withdrawal
                if (transactionType == "Withdrawal" && account.AccountBalance < amount)
                    return false;

                // Update balance
                if (transactionType == "Deposit")
                    account.AccountBalance += amount;
                else if (transactionType == "Withdrawal")
                    account.AccountBalance -= amount;

                // Create transaction
                var transaction = new Transaction
                {
                    AccountId = accountId,
                    Amount = amount,
                    TransactionTypeId = transactionTypeEntity.TransactionTypeId,
                    TransactionDate = DateTime.Now,
                    Description = $"{transactionType} transaction"
                };

                await _database.UpdateAsync(account);
                await _database.InsertAsync(transaction);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task SeedDataAsync()
        {
            // Create transaction types
            var depositType = new TransactionType { Name = "Deposit" };
            var withdrawalType = new TransactionType { Name = "Withdrawal" };

            await _database.InsertAsync(depositType);
            await _database.InsertAsync(withdrawalType);

            // Create customers
            var customer1 = new Customer
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@bank.com",
                PhoneNumber = "555-0101",
                PhysicalAddress = "123 Main St",
                IdentityNumber = "ID001",
                Nationality = "US"
            };

            var customer2 = new Customer
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@bank.com",
                PhoneNumber = "555-0102",
                PhysicalAddress = "456 Oak Ave",
                IdentityNumber = "ID002",
                Nationality = "US"
            };

            await _database.InsertAsync(customer1);
            await _database.InsertAsync(customer2);

            // Create accounts
            var account1 = new Account
            {
                AccountNumber = "ACC001",
                CustomerId = customer1.CustomerId,
                AccountBalance = 1500.00m,
                IsActive = true,
                DateOpened = DateTime.Now.AddYears(-1)
            };

            var account2 = new Account
            {
                AccountNumber = "ACC002",
                CustomerId = customer2.CustomerId,
                AccountBalance = 2500.00m,
                IsActive = true,
                DateOpened = DateTime.Now.AddMonths(-6)
            };

            await _database.InsertAsync(account1);
            await _database.InsertAsync(account2);
        }
    }
}