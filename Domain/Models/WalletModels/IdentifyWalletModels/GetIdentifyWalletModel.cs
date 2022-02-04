using Newtonsoft.Json;

namespace Domain.Models.WalletModels.IdentifyWalletModels
{
    public class GetIdentifyWalletModel
    {
        [JsonProperty("identifyWalletId")]
        [JsonRequired]
        public int IdentifyWalletId { get; set; }
    }
}
