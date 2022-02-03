using Domain.Models.WalletModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.TransactionModels
{
    public class TransactionsHistoryForUnidentifyWallet : TransactionsHistory
    {
        /// <summary>
        /// Unidentify E-Wallet Id
        /// </summary>
        [Column("UnidentifyWallet")]
        [ForeignKey("UnidentifyWalletFK")]

        public UnidentifyWallet UnidentifyWallet { get; set; }
    }
}
