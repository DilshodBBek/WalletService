using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.WalletModels
{
    public class CreateWalletModel
    {
        [JsonProperty("amountOfMoney")]
        public int AmountOfMoney { get; set; }

    }
}
