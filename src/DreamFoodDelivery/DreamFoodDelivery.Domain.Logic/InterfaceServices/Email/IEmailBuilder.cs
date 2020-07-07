using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.InterfaceServices
{
    public interface IEmailBuilder
    {
        Task<Result> SendConfirmMessage(AppUser user, string callBackUrl, CancellationToken cancellationToken = default);
        Task<Result> SendPasswordResetMessageAsync(AppUser user, string callBackUrl, CancellationToken cancellationToken = default);

        Task<Result> SendEmailWithLinkAsync(string email, string subject, string link, string message, string callBackUrl = null, CancellationToken cancellationToken = default);
    }
}
