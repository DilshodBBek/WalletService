using Domain.Models.WalletModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.TransactionModels
{
    public class TransactionsHistoryForUnidentifyWalletransactionsHistory
    {
        /// <summary>
        /// Unidentify E-Wallet Id
        /// </summary>
        [Column("UnidentifyWalletId")]
        public UnidentifyWallet UnidentifyWalletId { get; set; }
    }
}
