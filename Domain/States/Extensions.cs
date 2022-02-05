using Domain.Models.WalletModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;

namespace Domain.States
{
    public static class Extensions
    {
        public static string GetHash(this string value)
        {
            var sha1 = new System.Security.Cryptography.SHA1Managed();
            var plaintextBytes = Encoding.UTF8.GetBytes(value);
            var hashBytes = sha1.ComputeHash(plaintextBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.AppendFormat("{0:x2}", hashByte);
            }

            var hashString = sb.ToString();
            return hashString;
        }

        public static WalletServiceModel SetResultValue(this WalletServiceModel value, HttpStatusCode statusCode = HttpStatusCode.OK, string message = "", Wallet wallet = null)
        {
            value.HttpResponse.StatusCode = statusCode;
            value.HttpResponse.ReasonPhrase = message;
            value.Wallet = wallet;
            return value;
        }
        public static ResponseCore<Wallet> SetResultValue(this ResponseCore<Wallet> value, bool isSucces = true, string message = "", int statusCode = 200, Wallet wallet = null)
        {
            value.IsSuccess = isSucces;
            value.ErrorMessage += message;
            value.Result = wallet;
            value.StatusCode = statusCode;
            return value;
        }
        public static ResponseCore<Wallet> Auth(this ResponseCore<Wallet> value, UserManager<IdentityUser> _userManager, ControllerBase controllerBase)
        {
            string userId = controllerBase.Request.Headers["X-UserId"].ToString();
            string digest = controllerBase.Request.Headers["X-Digest"].ToString();

            string bodystring = "";
            var body = controllerBase.Request.Body;
            using (StreamReader sr = new(body))
            {
                if (body.CanSeek)
                    body.Seek(0, SeekOrigin.Begin);
                if (body.CanRead)
                    bodystring = sr.ReadToEndAsync().Result;
            }
            string Hashbody = bodystring.GetHash();
            if (!Hashbody.Equals(digest))
            {
                value = value.SetResultValue(false, "Request is not valid.Body may be changed by hackers ", 404);
            }
            if (_userManager.FindByIdAsync(userId).GetAwaiter().GetResult() == null)
            {
                value = value.SetResultValue(false, "User not found", 404);
            }
            return value;
        }
    }
}


