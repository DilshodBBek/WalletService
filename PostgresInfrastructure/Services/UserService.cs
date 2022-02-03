using Domain.Models;
using Microsoft.AspNetCore.Identity;
using PostgresInfrastructure.Interfaces;

namespace PostgresInfrastructure.Services
{
    public class UserService : IUserService
    {
        public readonly WalletDbContext db;
        public UserService(WalletDbContext _db)
        {
            db = _db;
        }
        public bool DeleteUser(int Id)
        {
            if (Id < 1) return false;
            var user = db.Users.FirstOrDefault(x => x.Id.Equals(Id));
            if (user == null) { return false; }
            db.Users.Remove(user);
            return true;
        }

        public IdentityUser? GetUser(int Id)
        {
            if (Id < 1) return null;

            var user = db.Users.FirstOrDefault(x => x.Id.Equals(Id));
            if (user == null) { return null; }
            return user;

        }
    }
}
