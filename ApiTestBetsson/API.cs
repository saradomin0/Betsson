using System.Net;
using RestSharp;
using Xunit;
using ApiTestBetssonConstans;

namespace ApiTests
{
    public class ApiTestsBetsson
    {

        #region GET
        [Fact]
        public async void BalanceGetRequest_ReturnOk()
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

        #region POS

        #endregion
    }
}
