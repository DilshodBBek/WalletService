using Domain.Models.WalletModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.TransactionModels
{
    public class TransactionsHistoryForIdentifyWallet : TransactionsHistory
    {
        /// <summary>
        /// Identify E-Wallet Id
        /// </summary>
        [Column("IdentifyWallet")]
        [ForeignKey("IdentifyWalletFK")]
        public IdentifyWallet IdentifyWallet { get; set; }
    }
}
