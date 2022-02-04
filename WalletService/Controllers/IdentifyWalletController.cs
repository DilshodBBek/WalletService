using Domain.Models.TransactionModels;
using Domain.Models.WalletModels;
using Domain.Models.WalletModels.IdentifyWalletModels;
using Domain.States;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PostgresInfrastructure.Interfaces;
using System.Net;

namespace WalletService.Controllers
{
    [ApiController]
    [Route("IdentifyWallet")]
    public class IdentifyWalletController : Controller
    {
        public UserManager<IdentityUser> _userManager { get; set; }
        public readonly IIdentifyWalletService _identifyWalletService;
        ResponseCore<IdentifyWallet> responseCore = new();
        public IdentifyWalletController(IIdentifyWalletService identifyWalletService, UserManager<IdentityUser> userManager)
        {
            _identifyWalletService = identifyWalletService;
            _userManager = userManager;
        }

        [HttpPost("/getWallet")]
        public Task<ResponseCore<IdentifyWallet>> GetWallet(GetIdentifyWalletModel identifyWalletModel)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel response = _identifyWalletService.GetById(Request.Headers["X-UserId"].ToString(), identifyWalletModel.IdentifyWalletId);
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                responseCore.ErrorMessage = "";
                responseCore.Result = response.Wallet as IdentifyWallet;
                return Task.FromResult(responseCore);
            }
            return GetResponse(response);
        }


        private Task<ResponseCore<IdentifyWallet>> GetResponse(WalletServiceModel response)
        {
            Response.StatusCode = (int)response.HttpResponse.StatusCode;
            responseCore.ErrorMessage = response.HttpResponse.ReasonPhrase.ToString();
            return Task.FromResult(responseCore);
        }

        [HttpPost("/replenish")]
        public Task<ResponseCore<IdentifyWallet>> Replenish(ReplenishIdentifyWalletModel model)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel response = _identifyWalletService.Replenish(model.SenderUsername, model.IdentifyWalletId, model.Amount);
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                responseCore.ErrorMessage = "";
                return Task.FromResult(responseCore);
            }
            return GetResponse(response);
        }

        [HttpPost("/statistics")]
        public Task<ResponseCore<List<TransactionsHistoryForIdentifyWallet>>> Statistics(GetTransactionStatistics statisticsModel)
        {
            ResponseCore<List<TransactionsHistoryForIdentifyWallet>> responseCore1 = new();

            if (!IsAuth())
            {
                responseCore1.ErrorMessage = responseCore.ErrorMessage;
                responseCore1.IsSuccess = false;
            }
            else if (statisticsModel.StartDate > statisticsModel.EndDate)
            {
                responseCore1.ErrorMessage = "Start date must be lower than end date";
                responseCore1.IsSuccess = false;
            }
            else
            {
                responseCore.ErrorMessage = "";
                responseCore1.Result = _identifyWalletService.GetStatistics(Request.Headers["X-UserId"].ToString(), statisticsModel.StartDate, statisticsModel.EndDate);
            }
            return Task.FromResult(responseCore1);
        }

        [HttpPost("/balance")]
        public Task<ResponseCore<IdentifyWallet>> Balance(GetIdentifyWalletModel walletID)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel? response = _identifyWalletService.GetById(Request.Headers["X-UserId"].ToString(), walletID.IdentifyWalletId);
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                responseCore.ErrorMessage = "";
                responseCore.Result = response.Wallet as IdentifyWallet;
                return Task.FromResult(responseCore);
            }
            return GetResponse(response);
        }

        [HttpPost("/createIdentifyWallet")]
        public Task<ResponseCore<IdentifyWallet>> CreateIdentifyWallet([FromBody] CreateWalletModel wallet)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel? response = _identifyWalletService.Add(Request.Headers["X-UserId"].ToString(), wallet.AmountOfMoney);
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                responseCore.ErrorMessage = "";
                responseCore.Result = response.Wallet as IdentifyWallet;
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
