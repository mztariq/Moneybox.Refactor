
using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using Xunit;

namespace Moneybox.Tests.Unit
{
    public class WithdrawMoneyTests : TestsBase
    {
        private readonly Mock<IAccountRepository> accountRepository;
        private readonly Mock<INotificationService> notificationService;

        public WithdrawMoneyTests()
        {
            accountRepository = new Mock<IAccountRepository>(); 
            notificationService = new Mock<INotificationService>();
        }

        [Fact]
        public void When_Call_execute_Throws_No_Implementation_Exception()
        {
            var account = GetAccountDetails(500m, 0m, 1m);

            accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>()))
                .Returns(account);
            accountRepository.SetupAllProperties();
            notificationService.SetupAllProperties();

            var moneyObject = new WithdrawMoney(accountRepository.Object, notificationService.Object);
            moneyObject.Execute(Guid.NewGuid(), account.Balance);

            accountRepository.Verify(x => x.GetAccountById(It.IsAny<Guid>()), Times.Once);
            accountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void When_Insufficient_Funds_Then_Throw_Invalid_Operation_Exception(decimal balance)
        {
            var amount = 1m;
            accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>()))
                .Returns(GetAccountDetails(balance, 0m, 12m));

            var moneyObject = new WithdrawMoney(accountRepository.Object, notificationService.Object);

            Assert.Throws<InvalidOperationException>(()
                => moneyObject.Execute(Guid.NewGuid(), amount));
        }
    }
}
