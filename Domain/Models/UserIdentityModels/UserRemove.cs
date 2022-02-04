using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.UserIdentityModels
{
    public class UserRemove
    {
        [JsonProperty("userId")]
        [JsonRequired]
        public string UserId { get; set; }
    }
}
