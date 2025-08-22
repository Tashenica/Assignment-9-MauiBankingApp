using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MauiBankingExercise.Models
{
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhysicalAddress { get; set; }
        public string IdentityNumber { get; set; }
        public string Nationality { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Account> Accounts { get; set; } = new List<Account>();

        [Ignore]
        public string FullName => $"{FirstName} {LastName}";
    }
}