using MauiBankingExercise.Models;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensionsAsync.Extensions;

namespace MauiBankingExercise.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        // INCREMENT THIS NUMBER WHENEVER YOU CHANGE THE SEED DATA
        private const int DB_VERSION = 2; // Change to 3, 4, 5 etc. when you update seed data

        public async Task InitAsync()
        {
            if (_database is not null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "Banking.db");

#if DEBUG
            // DEVELOPMENT ONLY: This will reset database when version changes
            var versionFile = Path.Combine(FileSystem.AppDataDirectory, "db_version.txt");
            var currentVersion = File.Exists(versionFile) ?
                int.Parse(File.ReadAllText(versionFile)) : 0;

            if (currentVersion < DB_VERSION)
            {
                // Delete old database if version changed
                if (File.Exists(databasePath))
                {
                    File.Delete(databasePath);
                }
                // Save new version
                File.WriteAllText(versionFile, DB_VERSION.ToString());
            }
#endif

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

        // ADD THIS METHOD: Manual reset for development
        public async Task ResetDatabaseAsync()
        {
            await InitAsync();

            // Delete all data in order (due to foreign keys)
            await _database.DeleteAllAsync<Transaction>();
            await _database.DeleteAllAsync<Account>();
            await _database.DeleteAllAsync<Customer>();
            await _database.DeleteAllAsync<TransactionType>();

            // Re-seed with new data
            await SeedDataAsync();
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
                FirstName = "Tashenica",
                LastName = "Tulley",
                Email = "tashenicaklientjie@gmail.com",
                PhoneNumber = "081 811 8484",
                PhysicalAddress = "14 Gold Str",
                IdentityNumber = "ID119",
                Nationality = "RSA"
            };

            var customer2 = new Customer
            {
                FirstName = "Emilio",
                LastName = "Moses",
                Email = "emiliomoses0@gmail.com",
                PhoneNumber = "084 484 8375",
                PhysicalAddress = "45 Oak Ave",
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