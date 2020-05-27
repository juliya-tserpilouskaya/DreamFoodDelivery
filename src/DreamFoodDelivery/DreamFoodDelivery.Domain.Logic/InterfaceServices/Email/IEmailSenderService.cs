using DreamFoodDelivery.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IEmailSenderService
    {
        Task<Result> SendEmailAsync(string email,
                            string subject,
                            string message,
                            CancellationToken cancellationToken = default);
    }
}
