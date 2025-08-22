using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MauiBankingExercise.Models
{
    public class TransactionType
    {
        [PrimaryKey, AutoIncrement]
        public int TransactionTypeId { get; set; }

        public string Name { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}