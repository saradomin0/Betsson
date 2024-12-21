using Xunit;
using Moq;
using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Data.Models;

namespace Betsson.OnlineWallets.UnitTest
{
    public class UnitTestOnlineWallets
    {
        #region GetBalanceAsync

        [Fact]
        public async Task GetBalanceAsync_ReturnZero_WithoutTransaction()
        {
            //Arrange
            var mockRepository = new Mock<IOnlineWalletRepository>();
            mockRepository.Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                  .ReturnsAsync((OnlineWalletEntry?)null);

            var service = new OnlineWalletService(mockRepository.Object);

            //Act
            var balance = await service.GetBalanceAsync();

            // Assert
            Assert.NotNull(balance);
            Assert.Equal(0, balance.Amount);
        }

        [Fact]
        public async Task GetBalanceAsync_ReturnCorrectBalance_WithTransaction()
        {
            // Arrange
            var mockRepository = new Mock<IOnlineWalletRepository>();
            mockRepository.Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                          .ReturnsAsync(new OnlineWalletEntry
                          {
                              Amount = 100,
                              BalanceBefore = 50
                          });

            var service = new OnlineWalletService(mockRepository.Object);

            // Act
            var balance = await service.GetBalanceAsync();

            // Assert
            Assert.NotNull(balance);
            Assert.Equal(150, balance.Amount);
        }
        #endregion

        #region DepositFundsAsync

        #endregion

        #region WithdrawFundsAsync

        #endregion
    }
}