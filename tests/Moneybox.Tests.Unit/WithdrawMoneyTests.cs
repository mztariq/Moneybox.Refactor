
using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using Xunit;

namespace Moneybox.Tests.Unit
{
    public class WithdrawMoneyTests
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
            var moneyObject = new WithdrawMoney(accountRepository.Object, notificationService.Object);

            Assert.Throws<NotImplementedException>(() 
                => moneyObject.Execute(Guid.NewGuid(), 0m));
        }
    }
}
