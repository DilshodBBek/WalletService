using Newtonsoft.Json;

namespace Domain.Models.WalletModels
{
    public class GetTransactionStatistics
    {
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
    }
}
