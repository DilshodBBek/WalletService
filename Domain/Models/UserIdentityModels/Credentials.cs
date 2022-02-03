using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.UserIdentityModels
{
    public class Credentials
    {
        /// <summary>
        /// Username for authorization
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Username for authorization
        /// </summary>
        public string? Password { get; set; }
    }
}
