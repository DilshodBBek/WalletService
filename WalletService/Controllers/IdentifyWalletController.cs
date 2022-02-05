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
        ResponseCore<Wallet> responseCore = new();
        public IdentifyWalletController(IIdentifyWalletService identifyWalletService, UserManager<IdentityUser> userManager)
        {
            _identifyWalletService = identifyWalletService;
            _userManager = userManager;
        }
        private Task<ResponseCore<Wallet>> GetResponse(WalletServiceModel response)
        {
            if (response.HttpResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                SetResponseValues(response.Wallet);
                return Task.FromResult(responseCore);
            }
            SetResponseValues(false, response.HttpResponse.ReasonPhrase.ToString(), (int)response.HttpResponse.StatusCode);
            return Task.FromResult(responseCore);
        }

        [HttpPost("/replenish")]
        public Task<ResponseCore<Wallet>> Replenish(ReplenishIdentifyWalletModel model)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel response = _identifyWalletService.Replenish(model.SenderUsername, model.IdentifyWalletId, model.Amount);
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
        public Task<ResponseCore<Wallet>> Balance(GetIdentifyWalletModel walletID)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel? response = _identifyWalletService.GetById(Request.Headers["X-UserId"].ToString(), walletID.IdentifyWalletId);
            return GetResponse(response);
        }

        [HttpPost("/createIdentifyWallet")]
        public Task<ResponseCore<Wallet>> CreateIdentifyWallet([FromBody] CreateWalletModel wallet)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel? response = _identifyWalletService.Add(Request.Headers["X-UserId"].ToString(), wallet.AmountOfMoney);
            return GetResponse(response);
        }
        private bool IsAuth()
        {
            responseCore= responseCore.Auth(_userManager, this);
            //string userId = Request.Headers["X-UserId"].ToString();
            //string digest = Request.Headers["X-Digest"].ToString();

            //string body = "";
            //using (StreamReader sr = new(Request.Body))
            //{
            //    if (Request.Body.CanSeek)
            //        Request.Body.Seek(0, SeekOrigin.Begin);
            //    if (Request.Body.CanRead)
            //        body = sr.ReadToEndAsync().Result;
            //}
            //string Hashbody = body.GetHash();
            //if (!Hashbody.Equals(digest))
            //{
            //    SetResponseValues(false, "Request is not valid.Body may be changed by hackers\n", 400);
            //}
            //if (_userManager.FindByIdAsync(userId).GetAwaiter().GetResult() == null)
            //{
            //    SetResponseValues(false, "User not found", 404);
            //}
            return responseCore.IsSuccess;
        }

        private void SetResponseValues(bool IsSuccess = true, string errorMessage = "", int statusCode = 200, Wallet wallet = null)
        {
            responseCore = responseCore.SetResultValue(true, errorMessage, statusCode, wallet);
            Response.StatusCode = statusCode;
        }
        private void SetResponseValues(Wallet wallet)
        {
             responseCore = responseCore.SetResultValue(true, "",200, wallet);
            Response.StatusCode = 200;
        }
    }
}
