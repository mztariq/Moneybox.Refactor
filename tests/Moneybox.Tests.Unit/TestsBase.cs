using Moneybox.App;
using System;

namespace Moneybox.Tests.Unit
{
    public class TestsBase
    {
        public static Account GetAccountDetails(decimal balance, decimal withDrawn, decimal paidIn)
        {
            var user = new User() { Email = "test@test.com", Id = Guid.NewGuid(), Name = "test" };

            return new Account(Guid.NewGuid(), user, balance, withDrawn, paidIn);
        }

    }
}