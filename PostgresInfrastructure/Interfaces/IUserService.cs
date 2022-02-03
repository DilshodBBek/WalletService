using Domain.Models.UserIdentityModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresInfrastructure.Interfaces
{
    public interface IUserService
    {
        public IdentityUser? GetUser(int Id);

        public bool DeleteUser(int Id);
    }
}
