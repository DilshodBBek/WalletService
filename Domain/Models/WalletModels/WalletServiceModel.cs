using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.WalletModels
{
    public class WalletServiceModel
    {
        public Wallet Wallet { get; set; }
        public HttpResponseMessage HttpResponse { get; set; }
    }
}
