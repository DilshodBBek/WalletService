using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.WalletModels.IdentifyWalletModels
{
    public class ReplenishIdentifyWalletModel: GetIdentifyWalletModel
    {
        [JsonProperty("amount")]
        [JsonRequired]
        public int Amount { get; set; }

        [JsonProperty("senderUsername")]
        [JsonRequired]
        public string? SenderUsername { get; set; }
    }
}
