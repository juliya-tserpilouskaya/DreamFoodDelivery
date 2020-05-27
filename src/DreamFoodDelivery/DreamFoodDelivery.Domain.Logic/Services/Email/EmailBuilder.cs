using DreamFoodDelivery.Common;
using DreamFoodDelivery.Common.Сonstants;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Domain.Logic.Services
{
    public class EmailBuilder : IEmailBuilder
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSenderService _emailSender;
        public EmailBuilder(UserManager<User> userManager, IEmailSenderService emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [LoggerAttribute]
        public async Task<Result> SendConfirmMessage(User user, string callBackUrl, CancellationToken cancellationToken = default)
        {
            var t = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var token = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(t));

            var emailForSending = user.Email;

            var subject = EmailConstants.CONFIRM_SUBJECT;

            var finalUrl = callBackUrl + $"?userId={user.Id}&token={token}";

            var message = $"{EmailConstants.CONFIRM_MESSAGE_PART_BEFORE_URL}, <a href='{finalUrl}'>{EmailConstants.CONFIRM_MESSAGE_URL_TEXT}</a> {EmailConstants.CONFIRM_MESSAGE_PART_AFTER_URL}";

            return await _emailSender.SendEmailAsync(emailForSending, subject, message, cancellationToken);
        }

        [LoggerAttribute]
        public async Task<Result> SendEmailWithLinkAsync(string email, string subject, string link, string message, string callBackUrl = null, CancellationToken cancellationToken = default)
        {
            string finalMessage;

            if (string.IsNullOrEmpty(callBackUrl))
            {
                finalMessage = message;
            }
            else
            {
                finalMessage = $"<a href='{callBackUrl}'>{link}</a>, {message}";
            }

            return await _emailSender.SendEmailAsync(email, subject, finalMessage, cancellationToken);
        }

        [LoggerAttribute]
        public async Task<Result> SendPasswordResetMessageAsync(User user, string callBackUrl, CancellationToken cancellationToken = default)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var codedToken = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(token));

            var emailForSending = user.Email;

            var subject = EmailConstants.PASSWORD_SUBJECT;

            var finalUrl = callBackUrl + $"?userId={user.Id}&token={codedToken}";

            var message = $"{EmailConstants.PASSWORD_MESSAGE_PART_BEFORE_URL}, <a href='{finalUrl}'>{EmailConstants.PASSWORD_MESSAGE_URL_TEXT}</a> {EmailConstants.PASSWORD_MESSAGE_PART_AFTER_URL}";

            return await _emailSender.SendEmailAsync(emailForSending, subject, message, cancellationToken);
        }
    }
}
