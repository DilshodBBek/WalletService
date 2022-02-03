using Domain.Models.UserIdentityModels;
using Microsoft.AspNetCore.Mvc;

namespace WalletService.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Register([FromBody] Credentials credentials)
        {
            return View();
        }
    }
}
