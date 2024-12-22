using Xunit;
using Moq;
using Betsson.OnlineWallets.Services;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Exceptions;

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

        [Fact]
        public async Task DepositFundsAsync_UpdateBalanceCorrectly()
        {
            //Arrane
            var mockRepository = new Mock<IOnlineWalletRepository>();
            mockRepository.Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry
                {
                    Amount = 200,
                    BalanceBefore = 50
                });

            mockRepository.Setup(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
                .Returns(Task.CompletedTask);

            var service = new OnlineWalletService(mockRepository.Object);
            var deposit = new Deposit
            {
                Amount = 150
            };

            //Act
            var newBalance = await service.DepositFundsAsync(deposit);

            //Assert
            Assert.NotNull(newBalance);
            Assert.Equal(400, newBalance.Amount);
        }

        #endregion

        #region WithdrawFundsAsync

        [Fact]
        public async Task WithdrawFundsAsync_ThrowException()
        {
            //Arrane
            var mockRepository = new Mock<IOnlineWalletRepository>();
            mockRepository.Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry
                {
                    Amount = 25,
                    BalanceBefore = 25
                });

            var service = new OnlineWalletService(mockRepository.Object);
            var withdraw = new Withdrawal
            {
                Amount = 150
            };

            //Act & Assert
            await Assert.ThrowsAsync<InsufficientBalanceException>(() => service.WithdrawFundsAsync(withdraw));           
        }

        [Fact]
        public async Task WithdrawFundsAsync_UpdateBalanceCorrectly()
        {
            //Arrane
            var mockRepository = new Mock<IOnlineWalletRepository>();
            mockRepository.Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                .ReturnsAsync(new OnlineWalletEntry
                {
                    Amount = 100,
                    BalanceBefore = 50
                });
            mockRepository.Setup(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()))
                .Returns(Task.CompletedTask);

            var service = new OnlineWalletService(mockRepository.Object);
            var withdraw = new Withdrawal
            {
                Amount = 50
            };

            //Act
            var newBalance = await service.WithdrawFundsAsync(withdraw);

            //Assert
            Assert.NotNull(newBalance);
            Assert.Equal(100, newBalance.Amount);
        }
        #endregion
    }
}