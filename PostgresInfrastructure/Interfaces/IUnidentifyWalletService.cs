using Domain.Models.TransactionModels;

namespace PostgresInfrastructure.Interfaces
{
    public interface IUnidentifyWalletService : IWalletService
    {
        public List<TransactionsHistoryForUnidentifyWallet> GetStatistics(string UserId, DateTime startDate, DateTime endDate);
    }
}
