using SQLite;
using SQLiteNetExtensions.Extensions;
using MauiBankingExercise.Models;

namespace MauiBankingExercise.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _database;
        private readonly string _databasePath;

        public DatabaseService()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "banking.db");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            _database = new SQLiteConnection(_databasePath);

            // Create all tables
            _database.CreateTable<Bank>();
            _database.CreateTable<Customer>();
            _database.CreateTable<CustomerType>();
            _database.CreateTable<Account>();
            _database.CreateTable<AccountType>();
            _database.CreateTable<Transaction>();
            _database.CreateTable<TransactionType>();
            _database.CreateTable<Auth>();
            _database.CreateTable<AuthType>();
            _database.CreateTable<Asset>();
            _database.CreateTable<AssetType>();

            SeedData();
        }

        private void SeedData()
        {
            // Check if data already exists
            if (_database.Table<Customer>().Count() > 0) return;

            // Seed Bank
            var bank = new Bank
            {
                BankName = "Demo Bank",
                BankAddress = "123 Banking Street",
                BranchCode = "001",
                ContactPhoneNumber = "555-0123",
                ContactEmail = "info@demobank.com",
                IsActive = true,
                OperatingHours = "9AM-5PM"
            };
            _database.Insert(bank);

            // Seed Customer Types
            var customerTypes = new[]
            {
                new CustomerType { Name = "Individual" },
                new CustomerType { Name = "Business" }
            };
            _database.InsertAll(customerTypes);

            // Seed Account Types
            var accountTypes = new[]
            {
                new AccountType { Name = "Savings" },
                new AccountType { Name = "Checking" },
                new AccountType { Name = "Credit" }
            };
            _database.InsertAll(accountTypes);

            // Seed Transaction Types
            var transactionTypes = new[]
            {
                new TransactionType { Name = "Deposit" },
                new TransactionType { Name = "Withdrawal" },
                new TransactionType { Name = "Transfer" }
            };
            _database.InsertAll(transactionTypes);

            // Seed Customers
            var customers = new[]
            {
                new Customer
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@email.com",
                    PhoneNumber = "555-1234",
                    PhysicalAddress = "123 Main St",
                    IdentityNumber = "1234567890",
                    CustomerTypeId = 1,
                    BankId = bank.BankId,
                    GenderTypeId = 1,
                    RaceTypeId = 1,
                    Nationality = "USA",
                    MaritalStatusId = 1,
                    EmploymentStatusId = 1
                },
                new Customer
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@email.com",
                    PhoneNumber = "555-5678",
                    PhysicalAddress = "456 Oak Ave",
                    IdentityNumber = "0987654321",
                    CustomerTypeId = 1,
                    BankId = bank.BankId,
                    GenderTypeId = 2,
                    RaceTypeId = 1,
                    Nationality = "USA",
                    MaritalStatusId = 2,
                    EmploymentStatusId = 2
                },
                new Customer
                {
                    FirstName = "Michael",
                    LastName = "Johnson",
                    Email = "michael.johnson@email.com",
                    PhoneNumber = "555-9999",
                    PhysicalAddress = "789 Pine St",
                    IdentityNumber = "1122334455",
                    CustomerTypeId = 1,
                    BankId = bank.BankId,
                    GenderTypeId = 1,
                    RaceTypeId = 2,
                    Nationality = "USA",
                    MaritalStatusId = 1,
                    EmploymentStatusId = 1
                }
            };
            _database.InsertAll(customers);

            // Get inserted customers to get their IDs
            var insertedCustomers = _database.Table<Customer>().ToList();

            // Seed Accounts
            var accounts = new[]
            {
                // John Doe's accounts
                new Account
                {
                    AccountNumber = "SAV001",
                    AccountTypeId = 1, // Savings
                    IsActive = true,
                    CustomerId = insertedCustomers[0].CustomerId,
                    DateOpened = DateTime.Now.AddMonths(-12),
                    AccountBalance = 5500.00m
                },
                new Account
                {
                    AccountNumber = "CHK001",
                    AccountTypeId = 2, // Checking
                    IsActive = true,
                    CustomerId = insertedCustomers[0].CustomerId,
                    DateOpened = DateTime.Now.AddMonths(-8),
                    AccountBalance = 1200.50m
                },
                // Jane Smith's accounts
                new Account
                {
                    AccountNumber = "SAV002",
                    AccountTypeId = 1, // Savings
                    IsActive = true,
                    CustomerId = insertedCustomers[1].CustomerId,
                    DateOpened = DateTime.Now.AddMonths(-24),
                    AccountBalance = 8750.25m
                },
                new Account
                {
                    AccountNumber = "CHK002",
                    AccountTypeId = 2, // Checking
                    IsActive = true,
                    CustomerId = insertedCustomers[1].CustomerId,
                    DateOpened = DateTime.Now.AddMonths(-18),
                    AccountBalance = 2100.75m
                },
                // Michael Johnson's accounts
                new Account
                {
                    AccountNumber = "SAV003",
                    AccountTypeId = 1, // Savings
                    IsActive = true,
                    CustomerId = insertedCustomers[2].CustomerId,
                    DateOpened = DateTime.Now.AddMonths(-6),
                    AccountBalance = 3200.00m
                }
            };
            _database.InsertAll(accounts);

            // Seed some sample transactions
            var insertedAccounts = _database.Table<Account>().ToList();
            var sampleTransactions = new[]
            {
                // Transactions for first account
                new Transaction
                {
                    TransactionTypeId = 1, // Deposit
                    AccountId = insertedAccounts[0].AccountId,
                    TransactionDate = DateTime.Now.AddDays(-30),
                    Amount = 1000.00m,
                    Description = "Initial deposit"
                },
                new Transaction
                {
                    TransactionTypeId = 2, // Withdrawal
                    AccountId = insertedAccounts[0].AccountId,
                    TransactionDate = DateTime.Now.AddDays(-15),
                    Amount = 200.00m,
                    Description = "ATM withdrawal"
                },
                new Transaction
                {
                    TransactionTypeId = 1, // Deposit
                    AccountId = insertedAccounts[0].AccountId,
                    TransactionDate = DateTime.Now.AddDays(-7),
                    Amount = 500.00m,
                    Description = "Payroll deposit"
                }
            };
            _database.InsertAll(sampleTransactions);
        }

        // CRUD Operations
        public List<Customer> GetCustomers()
        {
            return _database.GetAllWithChildren<Customer>();
        }

        public Customer GetCustomerWithAccounts(int customerId)
        {
            return _database.GetWithChildren<Customer>(customerId);
        }

        public List<Account> GetCustomerAccounts(int customerId)
        {
            return _database.Table<Account>()
                .Where(a => a.CustomerId == customerId)
                .ToList();
        }

        public Account GetAccount(int accountId)
        {
            return _database.GetWithChildren<Account>(accountId);
        }

        public List<Transaction> GetAccountTransactions(int accountId)
        {
            return _database.GetAllWithChildren<Transaction>()
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        public List<TransactionType> GetTransactionTypes()
        {
            return _database.Table<TransactionType>().ToList();
        }

        public int SaveTransaction(Transaction transaction)
        {
            if (transaction.TransactionId != 0)
                return _database.Update(transaction);
            else
                return _database.Insert(transaction);
        }

        public int UpdateAccount(Account account)
        {
            return _database.Update(account);
        }
    }
}