using DreamFoodDelivery.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic
{
    public interface IEmailSenderService
    {
        Result SendMailResult (string toEmailAddress, string emailTitle, string emailMsgBody);
    }
}
