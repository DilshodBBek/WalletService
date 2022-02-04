using Domain.Models;
using Domain.Models.TransactionModels;
using Domain.Models.WalletModels;
using Domain.Models.WalletModels.UnidentifyWalletModels;
using PostgresInfrastructure.Interfaces;

namespace PostgresInfrastructure.Services
{
    public class UnidentifyWalletService : IUnidentifyWalletService
    {
        private readonly WalletDbContext _walletDb;

        private WalletServiceModel Result = new()
        {
            HttpResponse = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK
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
                    Result.HttpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Result.HttpResponse.ReasonPhrase = "User not found";
                    return Result;
                }
                UnidentifyWallet wallet = new UnidentifyWallet()
                {
                    AmountOfMoney = amount,
                    User = user
                };
                _walletDb.UnidentifyWallets.Add(wallet as UnidentifyWallet);
                _walletDb.SaveChanges();
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
        /// Check user wallet exist
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsWalletExist(string userId)
        {
            var b= _walletDb.UnidentifyWallets.FirstOrDefault(x => x.User.Id.Equals(userId)) != null;
            return b;
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
                    Result.HttpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Result.HttpResponse.ReasonPhrase = "Wallet not found";
                    return Result;
                }
                unidentifyWallet.AmountOfMoney += amount;
                if (IsHighUnidentifyWalletAmount(unidentifyWallet.AmountOfMoney)) return Result;
                _walletDb.TransactionsHistoryForUnidentifyWallet.Add(
                    new TransactionsHistoryForUnidentifyWallet()
                    {
                        SenderUser = username,
                        TransactionAmount = amount,
                        TransactionDate = DateTime.Now,
                        UnidentifyWallet = unidentifyWallet
                    });
                _walletDb.SaveChanges();
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
                Result.HttpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                Result.HttpResponse.ReasonPhrase = "Unidentify Wallet not found";
                return null;
            }
            return wallet;
        }

        private bool IsHighUnidentifyWalletAmount(int amount)
        {
            if (amount > 10000)
            {
                Result.HttpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Result.HttpResponse.ReasonPhrase = "Unidentify Wallet money amount cannot be higher than 10.000";
                return true;
            }
            return false;
        }
    }
}

