using Domain.Models.TransactionModels;
using Domain.Models.WalletModels.IdentifyWalletModels;
using Domain.Models.WalletModels.UnidentifyWalletModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models
{
    public class WalletDbContext : IdentityDbContext<IdentityUser>
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
        {
        }

        public DbSet<TransactionsHistoryForIdentifyWallet> TransactionsHistoryForIdentifyWallets { get; set; }
        public DbSet<TransactionsHistoryForUnidentifyWallet> TransactionsHistoryForUnidentifyWallet { get; set; }
        public DbSet<IdentifyWallet> IdentifyWallets { get; set; }
        public DbSet<UnidentifyWallet> UnidentifyWallets { get; set; }
    }
}
