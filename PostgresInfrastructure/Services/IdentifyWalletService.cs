using Domain.Models;
using Domain.Models.TransactionModels;
using Domain.Models.WalletModels;
using Domain.Models.WalletModels.IdentifyWalletModels;
using PostgresInfrastructure.Interfaces;

namespace PostgresInfrastructure.Services
{
    public class IdentifyWalletService : IIdentifyWalletService
    {
        private readonly WalletDbContext _walletDb;

        private WalletServiceModel Result = new()
        {
            HttpResponse = new HttpResponseMessage()
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
        public WalletServiceModel Add(string userId, int amount)
        {
            try
            {
                if (IsHighIdentifyWalletAmount(amount))
                    return Result;

                var user = string.IsNullOrEmpty(userId) ? null : _walletDb.Users.FirstOrDefault(x => x.Id.Equals(userId));
                if (user == null)
                {
                    Result.HttpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Result.HttpResponse.ReasonPhrase = "User not found";
                    return Result;
                }
                IdentifyWallet wallet = new()
                {
                    AmountOfMoney = amount,
                    User = user
                };
                _walletDb.IdentifyWallets.Add(wallet as IdentifyWallet);
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
        /// Check user wallet exist
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsWalletExist(string userId)
        {
            return _walletDb.IdentifyWallets.FirstOrDefault(x => x.User.Id.Equals(userId)) != null;
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
        /// Get Transactions statistics between given dates
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<TransactionsHistoryForIdentifyWallet> GetStatistics(string UserId, DateTime startDate, DateTime endDate)
        {
            try
            {
                List<IdentifyWallet> wallets = _walletDb.IdentifyWallets.Where(x => x.User.Id.Equals(UserId)).ToList();
                List<TransactionsHistoryForIdentifyWallet> transactionsHistory = new();
                foreach (var wallet in wallets)
                {
                    var sdsd = _walletDb.TransactionsHistoryForIdentifyWallets.Where(x => x.IdentifyWallet.Id.Equals(wallet.Id) && x.TransactionDate > startDate && x.TransactionDate < endDate).ToList();
                    transactionsHistory.AddRange(sdsd);
                }
                return transactionsHistory;
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
                _walletDb.SaveChanges();
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
                    Result.HttpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                    Result.HttpResponse.ReasonPhrase = "Wallet not found";
                    return Result;
                }
                IdentifyWallet.AmountOfMoney += amount;
                if (IsHighIdentifyWalletAmount(IdentifyWallet.AmountOfMoney)) return Result;
                _walletDb.TransactionsHistoryForIdentifyWallets.Add(
                   new TransactionsHistoryForIdentifyWallet
                   {
                       SenderUser = username,
                       TransactionAmount = amount,
                       TransactionDate = DateTime.UtcNow,
                       IdentifyWallet = IdentifyWallet
                   });
                _walletDb.SaveChanges();
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
                Result.HttpResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
                Result.HttpResponse.ReasonPhrase = "Identify Wallet not found";
                return null;
            }
            return wallet;
        }

        private bool IsHighIdentifyWalletAmount(int amount)
        {
            if (amount > 100000)
            {
                Result.HttpResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                Result.HttpResponse.ReasonPhrase = "Identify Wallet money amount cannot be higher than 100.000";
                return true;
            }
            return false;
        }
    }
}
