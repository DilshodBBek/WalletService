using Domain.Models;
using Domain.Models.WalletModels;
using PostgresInfrastructure.Interfaces;

namespace PostgresInfrastructure.Services
{
    public class IdentifyWalletService : IIdentifyWalletService
    {
        private readonly WalletDbContext _walletDb;

        private WalletServiceModel Result = new WalletServiceModel()
        {
            httpResponse = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            }
        };
        public IdentifyWalletService(WalletDbContext walletDb)
        {
            _walletDb = walletDb;
        }


        /// <summary>
        /// Add new Identify Wallet to Database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public WalletServiceModel Add(string userId, Wallet wallet)
        {
            try
            {
                if (IsHighIdentifyWalletAmount(wallet))
                    return Result;

                var user = string.IsNullOrEmpty(userId) ? null : _walletDb.Users.FirstOrDefault(x => x.Id.Equals(userId));
                if (user == null)
                {
                    Result.httpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Result.httpResponse.ReasonPhrase = "User not found";
                    return Result;
                }

                wallet.User = user;
                _walletDb.IdentifyWallets.Add(wallet as IdentifyWallet);
                Result.Wallet = wallet;
                return Result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Identify Wallet By Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public WalletServiceModel GetById(string userId, int id)
        {
            try
            {
                var wallet = CheckWalletWithUserId(userId, id);
                if (wallet == null)
                    return Result;
                Result.Wallet = wallet;
                return Result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Remove Identify Wallet from database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public WalletServiceModel Remove(string userId, int Id)
        {
            try
            {
                var wallet = CheckWalletWithUserId(userId, Id);
                if (wallet == null)
                    return Result;

                _walletDb.IdentifyWallets.Remove(wallet);
                return Result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Replenish for Identify Wallet from any user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="walletId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public WalletServiceModel Replenish(string username, int walletId, int amount)
        {
            try
            {
                var IdentifyWallet = _walletDb.IdentifyWallets.FirstOrDefault(x => x.Id.Equals(walletId));
                if (IdentifyWallet == null)
                {
                    Result.httpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Result.httpResponse.ReasonPhrase = "Wallet not found";
                    return Result;
                }
                IdentifyWallet.AmountOfMoney += amount;
                if (IsHighIdentifyWalletAmount(IdentifyWallet)) return Result;
                _walletDb.TransactionsHistoryForIdentifyWallets.Add(
                   new Domain.Models.TransactionModels.TransactionsHistoryForIdentifyWallet
                    {
                        SenderUser = username,
                        TransactionAmount = amount,
                        TransactionDate = DateTime.Now,
                        IdentifyWallet = IdentifyWallet
                    });

                return Result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private IdentifyWallet? CheckWalletWithUserId(string userId, int Id)
        {
            var wallet = _walletDb.IdentifyWallets.FirstOrDefault(x => x.Id.Equals(Id) && x.User.Id.Equals(userId));
            if (wallet == null)
            {
                Result.httpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                Result.httpResponse.ReasonPhrase = "Identify Wallet not found";
                return null;
            }
            return wallet;
        }

        private bool IsHighIdentifyWalletAmount(Wallet IdentifyWallet)
        {
            if (IdentifyWallet.AmountOfMoney > 100000)
            {
                Result.httpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Result.httpResponse.ReasonPhrase = "Identify Wallet money amount cannot be higher than 100.000";
                return true;
            }
            return false;
        }
    }
}
