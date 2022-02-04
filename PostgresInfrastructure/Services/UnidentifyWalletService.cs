using Domain.Models;
using Domain.Models.TransactionModels;
using Domain.Models.WalletModels;
using PostgresInfrastructure.Interfaces;

namespace PostgresInfrastructure.Services
{
    public class UnidentifyWalletService : IUnidentifyWalletService
    {
        private readonly WalletDbContext _walletDb;

        private WalletServiceModel Result = new WalletServiceModel()
        {
            httpResponse = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            }
        };
        public UnidentifyWalletService(WalletDbContext walletDb)
        {
            _walletDb = walletDb;
        }


        /// <summary>
        /// Add new Unidentify Wallet to Database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public WalletServiceModel Add(string userId, Wallet wallet)
        {
            try
            {
                if (IsHighUnidentifyWalletAmount(wallet))
                    return Result;

                var user = string.IsNullOrEmpty(userId) ? null : _walletDb.Users.FirstOrDefault(x => x.Id.Equals(userId));
                if (user == null)
                {
                    Result.httpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Result.httpResponse.ReasonPhrase = "User not found";
                    return Result;
                }

                wallet.User = user;
                _walletDb.UnidentifyWallets.Add(wallet as UnidentifyWallet);
                Result.Wallet = wallet;
                return Result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Unidentify Wallet By Id
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
        /// Remove Unidentify Wallet from database
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

                _walletDb.UnidentifyWallets.Remove(wallet);
                return Result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Replenish for Unidentify Wallet from any user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="walletId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public WalletServiceModel Replenish(string username, int walletId, int amount)
        {
            try
            {
                var unidentifyWallet = _walletDb.UnidentifyWallets.FirstOrDefault(x => x.Id.Equals(walletId));
                if (unidentifyWallet == null)
                {
                    Result.httpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Result.httpResponse.ReasonPhrase = "Wallet not found";
                    return Result;
                }
                unidentifyWallet.AmountOfMoney += amount;
                if (IsHighUnidentifyWalletAmount(unidentifyWallet)) return Result;
                _walletDb.TransactionsHistoryForUnidentifyWallet.Add(
                    new TransactionsHistoryForUnidentifyWallet()
                    {
                        SenderUser = username,
                        TransactionAmount = amount,
                        TransactionDate = DateTime.Now,
                        UnidentifyWallet = unidentifyWallet
                    });

                return Result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private UnidentifyWallet? CheckWalletWithUserId(string userId, int Id)
        {
            var wallet = _walletDb.UnidentifyWallets.FirstOrDefault(x => x.Id.Equals(Id) && x.User.Id.Equals(userId));
            if (wallet == null)
            {
                Result.httpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                Result.httpResponse.ReasonPhrase = "Unidentify Wallet not found";
                return null;
            }
            return wallet;
        }

        private bool IsHighUnidentifyWalletAmount(Wallet unidentifyWallet)
        {
            if (unidentifyWallet.AmountOfMoney > 10000)
            {
                Result.httpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Result.httpResponse.ReasonPhrase = "Unidentify Wallet money amount cannot be higher than 10.000";
                return true;
            }
            return false;
        }
    }
}

