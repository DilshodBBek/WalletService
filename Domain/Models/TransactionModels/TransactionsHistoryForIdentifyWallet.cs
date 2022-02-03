using Domain.Models.WalletModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.TransactionModels
{
    public class TransactionsHistoryForIdentifyWallet: TransactionsHistory
    {
        /// <summary>
        /// Identify E-Wallet Id
        /// </summary>
        [Column("IdentifyWalletId")]
        public IdentifyWallet IdentifyWalletId { get; set; }
    }
}
