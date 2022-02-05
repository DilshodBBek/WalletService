using Domain.Models;
using Domain.Models.TransactionModels;
using Domain.Models.WalletModels;
using Domain.Models.WalletModels.UnidentifyWalletModels;
using Domain.States;
using PostgresInfrastructure.Interfaces;
using System.Net;

namespace PostgresInfrastructure.Services
{
    public class UnidentifyWalletService : IUnidentifyWalletService
    {
        private readonly WalletDbContext _walletDb;

        private WalletServiceModel Result = new()
        {
            HttpResponse = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            }
        };
        public UnidentifyWalletService(WalletDbContext walletDb)
        {
            _walletDb = walletDb;
        }

        /// <summary>
        /// Get Transactions statistics between given dates
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<TransactionsHistoryForUnidentifyWallet> GetStatistics(string UserId, DateTime startDate, DateTime endDate)
        {
            try
            {
                List<UnidentifyWallet> wallets = _walletDb.UnidentifyWallets.Where(x => x.User.Id.Equals(UserId)).ToList();
                List<TransactionsHistoryForUnidentifyWallet> transactionsHistory = new();
                foreach (var wallet in wallets)
                {
                    transactionsHistory.AddRange(_walletDb.TransactionsHistoryForUnidentifyWallet.Where(x => x.UnidentifyWallet.Id.Equals(wallet.Id) && x.TransactionDate > startDate && x.TransactionDate < endDate).ToList());
                }
                return transactionsHistory;
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Add new Unidentify Wallet to Database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="wallet"></param>
        /// <returns></returns>
        public WalletServiceModel Add(string userId, int amount)
        {
            try
            {
                if (IsHighUnidentifyWalletAmount(amount))
                    return Result;

                var user = string.IsNullOrEmpty(userId) ? null : _walletDb.Users.FirstOrDefault(x => x.Id.Equals(userId));
                if (user == null)
                {
                    SetResult(HttpStatusCode.NotFound, "User not found");
                    return Result;
                }
                UnidentifyWallet wallet = new UnidentifyWallet()
                {
                    AmountOfMoney = amount,
                    User = user
                };
                _walletDb.UnidentifyWallets.Add(wallet as UnidentifyWallet);
                _walletDb.SaveChanges();
                SetResult(HttpStatusCode.OK, "", wallet);
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

                SetResult(HttpStatusCode.OK, "", wallet);
                return Result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check user wallet exist
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsWalletExist(string userId)
        {
            return _walletDb.UnidentifyWallets.FirstOrDefault(x => x.User.Id.Equals(userId)) != null;
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
                _walletDb.SaveChanges();
                SetResult();
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
                    SetResult(HttpStatusCode.NotFound, "Wallet not found");
                    return Result;
                }
                unidentifyWallet.AmountOfMoney += amount;
                if (IsHighUnidentifyWalletAmount(unidentifyWallet.AmountOfMoney)) return Result;
                _walletDb.TransactionsHistoryForUnidentifyWallet.Add(
                    new TransactionsHistoryForUnidentifyWallet()
                    {
                        SenderUser = username,
                        TransactionAmount = amount,
                        TransactionDate = DateTime.UtcNow,
                        UnidentifyWallet = unidentifyWallet
                    });
                _walletDb.SaveChanges();
                SetResult();
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
                SetResult(HttpStatusCode.NotFound, "Unidentify Wallet not found");
                return null;
            }
            return wallet;
        }

        private bool IsHighUnidentifyWalletAmount(int amount)
        {
            if (amount >= 10000)
            {
                SetResult(HttpStatusCode.BadRequest, "Unidentify Wallet money amount cannot be higher than 10.000");
                return true;
            }
            return false;
        }
        private void SetResult(HttpStatusCode statusCode = HttpStatusCode.OK, string message = "", Wallet wallet = null)
        {
            Result = Result.SetResultValue(statusCode, message, wallet);
        }
    }
}

