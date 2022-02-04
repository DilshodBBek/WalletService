using Domain.Models.TransactionModels;
using Domain.Models.WalletModels;
using Domain.Models.WalletModels.UnidentifyWalletModels;
using Domain.States;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PostgresInfrastructure.Interfaces;
using System.Net;

namespace WalletService.Controllers
{
    [ApiController]
    [Route("UnidentifyWallet")]
    public class UnidentifyWalletController : Controller
    {
        public UserManager<IdentityUser> _userManager { get; set; }
        public readonly IUnidentifyWalletService _UnidentifyWalletService;
        ResponseCore<UnidentifyWallet> responseCore = new();
        public UnidentifyWalletController(IUnidentifyWalletService UnidentifyWalletService, UserManager<IdentityUser> userManager)
        {
            _UnidentifyWalletService = UnidentifyWalletService;
            _userManager = userManager;
        }

        [HttpPost("/getUnidentifyWallet")]
        public Task<ResponseCore<UnidentifyWallet>> GetWallet(GetUnidentifyWalletModel UnidentifyWalletModel)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel response = _UnidentifyWalletService.GetById(Request.Headers["X-UserId"].ToString(), UnidentifyWalletModel.UnidentifyWalletId);
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                responseCore.ErrorMessage = "";
                responseCore.Result = response.Wallet as UnidentifyWallet;
                return Task.FromResult(responseCore);
            }
            return GetResponse(response);
        }


        private Task<ResponseCore<UnidentifyWallet>> GetResponse(WalletServiceModel response)
        {
            Response.StatusCode = (int)response.HttpResponse.StatusCode;
            responseCore.ErrorMessage = response.HttpResponse.ReasonPhrase.ToString();
            return Task.FromResult(responseCore);
        }

        [HttpPost("/replenishUnidentify")]
        public Task<ResponseCore<UnidentifyWallet>> Replenish(ReplenishUnidentifyWalletModel model)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel response = _UnidentifyWalletService.Replenish(model.SenderUsername, model.UnidentifyWalletId, model.Amount);
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                responseCore.ErrorMessage = "";
                return Task.FromResult(responseCore);
            }
            return GetResponse(response);
        }

        [HttpPost("/statisticsUnidentify")]
        public Task<ResponseCore<List<TransactionsHistoryForUnidentifyWallet>>> Statistics(GetTransactionStatistics statisticsModel)
        {
            ResponseCore<List<TransactionsHistoryForUnidentifyWallet>> responseCore1 = new();

            if (!IsAuth())
            {
                responseCore1.ErrorMessage = responseCore.ErrorMessage;
                responseCore1.IsSuccess = false;
            }
            else
            {
                responseCore.ErrorMessage = "";
                responseCore1.Result = _UnidentifyWalletService.GetStatistics(Request.Headers["X-UserId"].ToString(), statisticsModel.StartDate, statisticsModel.EndDate);
            }
            return Task.FromResult(responseCore1);
        }

        [HttpPost("/balanceUnidentify")]
        public Task<ResponseCore<UnidentifyWallet>> Balance(GetUnidentifyWalletModel walletID)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel? response = _UnidentifyWalletService.GetById(Request.Headers["X-UserId"].ToString(), walletID.UnidentifyWalletId);
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                responseCore.ErrorMessage = "";
                responseCore.Result = response.Wallet as UnidentifyWallet;
                return Task.FromResult(responseCore);
            }
            return GetResponse(response);
        }

        [HttpPost("/createUnidentifyWallet")]
        public Task<ResponseCore<UnidentifyWallet>> CreateUnidentifyWallet([FromBody] CreateWalletModel wallet)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel? response = _UnidentifyWalletService.Add(Request.Headers["X-UserId"].ToString(), wallet.AmountOfMoney);
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                responseCore.ErrorMessage = "";
                responseCore.Result = response.Wallet as UnidentifyWallet;
                return Task.FromResult(responseCore);
            }
            return GetResponse(response);
        }
        private bool IsAuth()
        {
            string userId = Request.Headers["X-UserId"].ToString();
            string digest = Request.Headers["X-Digest"].ToString();

            string body = "";
            using (StreamReader sr = new(Request.Body))
            {
                if (Request.Body.CanSeek)
                    Request.Body.Seek(0, SeekOrigin.Begin);
                if (Request.Body.CanRead)
                    body = sr.ReadToEndAsync().Result;
            }
            string Hashbody = body.GetHash();
            if (!Hashbody.Equals(digest))
            {
                responseCore.IsSuccess = false;
                responseCore.ErrorMessage += "Request is not valid.Body may be changed by hackers\n";
                Response.StatusCode = 400;
            }
            if (_userManager.FindByIdAsync(userId).GetAwaiter().GetResult() == null)
            {
                responseCore.IsSuccess = false;
                responseCore.ErrorMessage += "User not found";
                Response.StatusCode = 404;
            }
            return responseCore.IsSuccess;
        }
    }
}
