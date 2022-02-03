using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.WalletModels
{
    public class Wallet
    {
        /// <summary>
        /// E-Wallet Id 
        /// </summary>
        [Column("Id")]
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Amount of Money for E-Wallet
        /// </summary>
        [Column("AmountOfMoney")]
        public int AmountOfMoney { get; set; }

        /// <summary>
        /// User for E-Wallet
        /// </summary>
        [Column("User")]
        public IdentityUser? User { get; set; }
    }
}
