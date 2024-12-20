using System.Net;
using RestSharp;
using Xunit;
using ApiTestBetssonConstans;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace ApiTests
{
    public class ApiTestsBetsson : IDisposable
    {
        private readonly RestClient _client;

        public ApiTestsBetsson()
        {
            _client = new RestClient(BetssonConstans.URL);
         
            Task.Run(async () => await ResetWallet()).GetAwaiter().GetResult();
        }

        private async Task ResetWallet()
        {
            var balanceRequest = new RestRequest(BetssonConstans.Balance, Method.Get);
            var balanceResponse = await _client.ExecuteAsync(balanceRequest);
            var jsonObject = JObject.Parse(balanceResponse.Content);
            var withdrawAmount = (double)jsonObject["amount"];

            if (withdrawAmount > 0)
            {
                var withdrawRequest = new RestRequest(BetssonConstans.Withdraw, Method.Post);
                withdrawRequest.AddJsonBody(new { amount = withdrawAmount });
                var withdrawResponse = await _client.ExecuteAsync(withdrawRequest);
            }
        }

        public void Dispose()
        {           
            _client.Dispose();
        }

        #region GET

        [Fact]
        public async Task BalanceGet_StatusOk()
        {
            // Arrange
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Balance, Method.Get);

            // Act
            var response = await client.ExecuteAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            Assert.NotEmpty(response.Content);
        }
        #endregion

        #region POST

        #region Deposit
        [Fact]
        public async Task DepositPost_StatusOk()
        {
            // Arrange
            var AddedAmount = 100;
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request.AddJsonBody(new { amount = AddedAmount });

            // Act
            var response = await client.ExecuteAsync(request);
            var jsonObject = JObject.Parse(response.Content);
            var amount = (int)jsonObject["amount"];

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Content);
            Assert.NotEmpty(response.Content);
            Assert.Equal(AddedAmount, amount);
        }

        [Fact]
        public async Task DepositPost_LargeNumber_StatusBadRequest()
        {
            // Arrange
            double AddedAmount = 1000000000000000000000000000000.0;
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request.AddJsonBody(new { amount = AddedAmount });

            // Act
            var response = await client.ExecuteAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DepositPost_NegativeNumber_StatusBadRequest()
        {
            // Arrange
            var AddedAmount = -500;
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request.AddJsonBody(new { amount = AddedAmount });

            // Act
            var response = await client.ExecuteAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DepositPost_String_StatusBadRequest()
        {
            // Arrange
            var AddedAmount = "AppleTree";
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request.AddJsonBody(new { amount = AddedAmount });

            // Act
            var response = await client.ExecuteAsync(request);
            var jsonObject = JObject.Parse(response.Content);
            var errors = jsonObject["errors"];
            string errorMessage = errors["$.amount"].ToString();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(errorMessage.Contains(BetssonConstans.InvalidStringErrorMessage));
        }

        [Fact]
        public async Task DepositPost_NullValue_StatusBadRequest()
        {
            // Arrange
            var AddedAmount = BetssonConstans.Value;
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request.AddJsonBody(new { amount = AddedAmount });

            // Act
            var response = await client.ExecuteAsync(request);
            var jsonObject = JObject.Parse(response.Content);
            var errors = jsonObject["errors"];
            string errorMessage = errors["$.amount"].ToString();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(errorMessage.Contains(BetssonConstans.InvalidStringErrorMessage));
        }

        [Fact]
        public async Task DepositPost_EmptyJsonBody_StatusBadRequest()
        {
            // Arrange
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request.AddJsonBody(BetssonConstans.EmptyJsonBody);

            // Act
            var response = await client.ExecuteAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task DepositPost_EmptyJson_StatusBadRequest()
        {
            // Arrange
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request.AddJsonBody(BetssonConstans.EmptyBody);

            // Act
            var response = await client.ExecuteAsync(request);
            var jsonObject = JObject.Parse(response.Content);
            var errors = jsonObject["errors"];
            string errorMessage = errors[""]?[0]?.ToString();
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, BetssonConstans.EmptyBodyErrorMessage);
        }
        #endregion

        #region Withdraw

        [Fact]
        public async Task WithdrawPost_StatusOk()
        {
            // Arrange
            var DepositAmount = 100;
            var WithdrawAmount = 50;
            var client = new RestClient(BetssonConstans.URL);
            // request to Deposit
            var request1 = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request1.AddJsonBody(new { amount = DepositAmount });
            // request to Withdraw
            var request2 = new RestRequest(BetssonConstans.Withdraw, Method.Post);
            request2.AddJsonBody(new { amount = WithdrawAmount });

            // Act
            var response1 = await client.ExecuteAsync(request1);
            var response2 = await client.ExecuteAsync(request2);
            var jsonObject = JObject.Parse(response2.Content);
            var amount = (int)jsonObject["amount"];


            // Assert
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            Assert.NotNull(response2.Content);
            Assert.NotEmpty(response2.Content);
            Assert.Equal((DepositAmount - WithdrawAmount), amount);
        }

        [Fact]
        public async Task WithdrawPost_Decimal_StatusOk()
        {
            // Arrange
            var DepositAmount = 100;
            var WithdrawAmount = 50.5;
            var client = new RestClient(BetssonConstans.URL);
            // request to Deposit
            var request1 = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request1.AddJsonBody(new { amount = DepositAmount });
            // request to Withdraw
            var request2 = new RestRequest(BetssonConstans.Withdraw, Method.Post);
            request2.AddJsonBody(new { amount = WithdrawAmount });

            // Act
            var response1 = await client.ExecuteAsync(request1);
            var response2 = await client.ExecuteAsync(request2);
            var jsonObject = JObject.Parse(response2.Content);
            var amount = (double)jsonObject["amount"];


            // Assert
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
            Assert.NotNull(response2.Content);
            Assert.NotEmpty(response2.Content);
            Assert.Equal((DepositAmount - WithdrawAmount), amount);
        }

        [Fact]
        public async Task WithdrawPost_NegativedWithdrawa_StatusOk()
        {
            // Arrange
            var DepositAmount = 100;
            var WithdrawAmount = 150;
            var client = new RestClient(BetssonConstans.URL);
            // request to Deposit
            var request1 = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request1.AddJsonBody(new { amount = DepositAmount });
            // request to Withdraw
            var request2 = new RestRequest(BetssonConstans.Withdraw, Method.Post);
            request2.AddJsonBody(new { amount = WithdrawAmount });

            // Act
            var response1 = await client.ExecuteAsync(request1);
            var response2 = await client.ExecuteAsync(request2);
            var jsonObject = JObject.Parse(response2.Content);
            var error = (string)jsonObject["title"];            

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
            Assert.Equal(error, BetssonConstans.InvalidWithdrawErrorMessage);
        }

        [Fact]
        public async Task WithdrawPost_StringWithdrawa_StatusOk()
        {
            // Arrange
            var DepositAmount = 100;
            var WithdrawAmount = "asdfasd";
            var client = new RestClient(BetssonConstans.URL);
            // request to Deposit
            var request1 = new RestRequest(BetssonConstans.Deposit, Method.Post);
            request1.AddJsonBody(new { amount = DepositAmount });
            // request to Withdraw
            var request2 = new RestRequest(BetssonConstans.Withdraw, Method.Post);
            request2.AddJsonBody(new { amount = WithdrawAmount });

            // Act
            var response1 = await client.ExecuteAsync(request1);
            var response2 = await client.ExecuteAsync(request2);
            var jsonObject = JObject.Parse(response2.Content);
            var errors = jsonObject["errors"];
            string errorMessage = errors["$.amount"].ToString();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
            Assert.True(errorMessage.Contains(BetssonConstans.InvalidStringErrorMessage));
        }

        [Fact]
        public async Task WithdrawPost_EmptyJson_StatusBadRequest()
        {
            // Arrange
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Withdraw, Method.Post);
            request.AddJsonBody(BetssonConstans.EmptyBody);

            // Act
            var response = await client.ExecuteAsync(request);
            var jsonObject = JObject.Parse(response.Content);
            var errors = jsonObject["errors"];
            string errorMessage = errors[""]?[0]?.ToString();
            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, BetssonConstans.EmptyBodyErrorMessage);
        }

        [Fact]
        public async Task WithdrawPost_EmptyJsonBody_StatusBadRequest()
        {
            // Arrange
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Withdraw, Method.Post);
            request.AddJsonBody(BetssonConstans.EmptyJsonBody);

            // Act
            var response = await client.ExecuteAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task WithdrawPost_NullValue_StatusBadRequest()
        {
            // Arrange
            var AddedAmount = BetssonConstans.Value;
            var client = new RestClient(BetssonConstans.URL);
            var request = new RestRequest(BetssonConstans.Withdraw, Method.Post);
            request.AddJsonBody(new { amount = AddedAmount });

            // Act
            var response = await client.ExecuteAsync(request);
            var jsonObject = JObject.Parse(response.Content);
            var errors = jsonObject["errors"];
            string errorMessage = errors["$.amount"].ToString();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.True(errorMessage.Contains(BetssonConstans.InvalidStringErrorMessage));
        }
        #endregion
        #endregion
    }
}
