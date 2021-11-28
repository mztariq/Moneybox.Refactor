using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using System;
using Xunit;

namespace Moneybox.Tests.Unit
{
    public class TransferMoneyTests : TestsBase
    {
        private readonly Mock<IAccountRepository> accountRepository;
        private readonly Mock<INotificationService> notificationService;

        public TransferMoneyTests()
        {
            accountRepository = new Mock<IAccountRepository>(); 
            notificationService = new Mock<INotificationService>();
        }

        [Fact]
        public void When_Insufficient_Funds_Then_Throw_Invalid_Operation_Exception()
        {
            var amount = 1m;
            accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>()))
                .Returns(new Account());

            var moneyObject = new TransferMoney(accountRepository.Object, notificationService.Object);

            Assert.Throws<InvalidOperationException>(() 
                => moneyObject.Execute(Guid.NewGuid(), Guid.NewGuid(), amount));
        }


        [Fact]
        public void When_PayIn_Limit_Reached_Then_Throw_Invalid_Operation_Exception()
        {
            var amount = -1m;
            var account = GetAccountDetails(500m, 4003m);
            accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>()))
                .Returns(account);

            
            notificationService.Setup(x => x.NotifyFundsLow(It.IsAny<string>())).Verifiable();
            notificationService.Setup(x => x.NotifyApproachingPayInLimit(account.User.Email)).Verifiable();

            var moneyObject = new TransferMoney(accountRepository.Object, notificationService.Object);

            Assert.Throws<InvalidOperationException>(()
                => moneyObject.Execute(Guid.NewGuid(), Guid.NewGuid(), amount));
        }


        [Fact]
        public void When_From_Balance_Is_Less_500_Notify_Low_Funds()
        {
            var account = GetAccountDetails(500m, 1m);

            accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>()))
                .Returns(account);
            accountRepository.SetupAllProperties();
            notificationService.SetupAllProperties();

            var moneyObject = new TransferMoney(accountRepository.Object, notificationService.Object);
            moneyObject.Execute(Guid.NewGuid(), Guid.NewGuid(), account.Balance);

            notificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Once);
            accountRepository.Verify(x => x.GetAccountById(It.IsAny<Guid>()), Times.Exactly(2));
            accountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));
        }


        [Fact]
        public void When_PayInLimit_Minus_PaidIn_Is_Less_500_Notify_Low_Funds()
        {
            var account = GetAccountDetails(3m, 3800m);

            accountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>()))
                .Returns(account);
            accountRepository.SetupAllProperties();
            notificationService.SetupAllProperties();

            var moneyObject = new TransferMoney(accountRepository.Object, notificationService.Object);
            moneyObject.Execute(Guid.NewGuid(), Guid.NewGuid(), account.Balance);

            notificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Once);
            accountRepository.Verify(x => x.GetAccountById(It.IsAny<Guid>()), Times.Exactly(2));
            accountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));
        }



        [Fact]
        public void Notification_Method_Called_Atleast_Once() 
        {
            //Arrange
            var mock = new Mock<INotificationService>();

            mock.SetupAllProperties();

            var sut = mock.Object;

            //Act
            sut.NotifyFundsLow("zee");

            mock.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Once);
        }
    }
}
