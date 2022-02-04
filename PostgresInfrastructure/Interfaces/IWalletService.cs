using Domain.Models.WalletModels;
using Domain.Models.WalletModels.IdentifyWalletModels;
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
        public bool IsWalletExist(string userId);
        public WalletServiceModel Add(string userId, int amount);
        public WalletServiceModel Replenish(string username, int walletId, int amount);
        public WalletServiceModel Remove(string userId, int Id);

        
    }
}
