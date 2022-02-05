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
        ResponseCore<Wallet> responseCore = new();
        public UnidentifyWalletController(IUnidentifyWalletService UnidentifyWalletService, UserManager<IdentityUser> userManager)
        {
            _UnidentifyWalletService = UnidentifyWalletService;
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

        [HttpPost("/replenishUnidentify")]
        public Task<ResponseCore<Wallet>> Replenish(ReplenishUnidentifyWalletModel model)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel response = _UnidentifyWalletService.Replenish(model.SenderUsername, model.UnidentifyWalletId, model.Amount);
            return GetResponse(response);
        }

        [HttpPost("/statisticsUnidentify")]
        public Task<ResponseCore<List<TransactionsHistoryForUnidentifyWallet>>> Statistics(GetTransactionStatistics statisticsModel)
        {
            ResponseCore<List<TransactionsHistoryForUnidentifyWallet>> responseCore1 = new() { };
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
                responseCore1.Result = _UnidentifyWalletService.GetStatistics(Request.Headers["X-UserId"].ToString(), statisticsModel.StartDate, statisticsModel.EndDate);
            }
            return Task.FromResult(responseCore1);
        }

        [HttpPost("/balanceUnidentify")]
        public Task<ResponseCore<Wallet>> Balance(GetUnidentifyWalletModel walletID)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel? response = _UnidentifyWalletService.GetById(Request.Headers["X-UserId"].ToString(), walletID.UnidentifyWalletId);
            return GetResponse(response);
        }

        [HttpPost("/createUnidentifyWallet")]
        public Task<ResponseCore<Wallet>> CreateUnidentifyWallet([FromBody] CreateWalletModel wallet)
        {
            if (!IsAuth()) return Task.FromResult(responseCore);
            WalletServiceModel? response = _UnidentifyWalletService.Add(Request.Headers["X-UserId"].ToString(), wallet.AmountOfMoney);
            return GetResponse(response);
        }
        private bool IsAuth()
        {
            responseCore = responseCore.Auth(_userManager, this);
            return responseCore.IsSuccess;
        }

        private void SetResponseValues(bool IsSuccess = true, string errorMessage = "", int statusCode=200, Wallet wallet=null)
        {
            responseCore = responseCore.SetResultValue(true, errorMessage, statusCode, wallet);
            Response.StatusCode = statusCode;
        }
        private void SetResponseValues(Wallet wallet)
        {
            responseCore=responseCore = responseCore.SetResultValue(true, "",200, wallet);
            Response.StatusCode = 200;
        }
    }
}
