using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.TransactionModels
{
    public class TransactionsHistory
    {
        /// <summary>
        /// Transaction Id
        /// </summary>
        [Column("Id")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// User who send money
        /// </summary>
        [Column("SenderUser")]
        public string SenderUser { get; set; }

        /// <summary>
        /// Amount of Money for Transaction
        /// </summary>
        [Column("TransactionAmount")]
        public int TransactionAmount { get; set; }

        /// <summary>
        /// Transaction date
        /// </summary>
        [Column("TransactionDate")]
        public DateTime TransactionDate { get; set; }
    }
}
