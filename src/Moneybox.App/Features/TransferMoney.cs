using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid accountHolderId, Guid accountRecipientId, decimal transferAmount)
        {
            Account accountHolder, accountRecipient;
            GetAccountDetails(accountHolderId, accountRecipientId, out accountHolder, out accountRecipient);

            accountHolder.WithdrawalAmount(transferAmount, _notificationService);

            accountRecipient.PayInAmount(transferAmount, _notificationService);
            UpdateAccounts(accountHolder, accountRecipient);
        }

        private void GetAccountDetails(Guid accountHolderId, Guid accountRecipientId, out Account accountHolder, out Account accountRecipient)
        {
            accountHolder = _accountRepository.GetAccountById(accountHolderId);
            accountRecipient = _accountRepository.GetAccountById(accountRecipientId);
        }

        private void UpdateAccounts(Account accountHolder, Account accountRecipient)
        {
            _accountRepository.Update(accountHolder);
            _accountRepository.Update(accountRecipient);
        }
    }
}
