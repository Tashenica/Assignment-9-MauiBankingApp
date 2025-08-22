using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MauiBankingExercise.Models
{
    public class Account
    {
        [PrimaryKey, AutoIncrement]
        public int AccountId { get; set; }

        public string AccountNumber { get; set; }

        [ForeignKey(typeof(Customer))]
        public int CustomerId { get; set; }

        public decimal AccountBalance { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateOpened { get; set; }

        [ManyToOne]
        public Customer Customer { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}