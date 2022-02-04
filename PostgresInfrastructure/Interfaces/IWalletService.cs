using Domain.Models.WalletModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresInfrastructure.Interfaces
{
    public interface IWalletService
    {
        public WalletServiceModel GetById(string userId, int id);
        public WalletServiceModel Add(string userId, Wallet wallet);
        public WalletServiceModel Replenish(string username, int walletId, int amount);
        public WalletServiceModel Remove(string userId, int Id);
    }
}
