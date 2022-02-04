using Domain.Models.TransactionModels;

namespace PostgresInfrastructure.Interfaces
{
    public interface IIdentifyWalletService : IWalletService
    {
        public List<TransactionsHistoryForIdentifyWallet> GetStatistics(string UserId, DateTime startDate, DateTime endDate);
    }
}
