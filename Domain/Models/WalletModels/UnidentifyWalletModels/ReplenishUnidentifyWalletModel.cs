using Newtonsoft.Json;

namespace Domain.Models.WalletModels.UnidentifyWalletModels
{
    public class ReplenishUnidentifyWalletModel : GetUnidentifyWalletModel
    {
        [JsonProperty("amount")]
        [JsonRequired]
        public int Amount { get; set; }

        [JsonProperty("senderUsername")]
        [JsonRequired]
        public string SenderUsername { get; set; }
    }
}
