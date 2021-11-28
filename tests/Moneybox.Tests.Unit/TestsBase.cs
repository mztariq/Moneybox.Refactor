using Moneybox.App;
using System;

namespace Moneybox.Tests.Unit
{
    public class TestsBase
    {
        public Account GetAccountDetails(decimal balance, decimal paidIn)
        {

            var user = new User() { Email = "test@test.com", Id = Guid.Empty, Name = "test" };
            var account = new Account()
            {
                Balance = balance,
                User = user,
                Id = Guid.Empty,
                PaidIn = paidIn,
                Withdrawn = 0m
            };

            return account;
        }

    }
}