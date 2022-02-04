using Newtonsoft.Json;

namespace Domain.Models.WalletModels.UnidentifyWalletModels
{
    public class GetUnidentifyWalletModel
    {
        [JsonProperty("unidentifyWalletId")]
        [JsonRequired]
        public int UnidentifyWalletId { get; set; }
    }
}
