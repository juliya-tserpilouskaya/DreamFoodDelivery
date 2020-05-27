using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Common.Сonstants
{
    public class EmailConstants
    {
        public const string DFD_EMAIL = "DFD@gmail.com";

        public const string CONFIRM_SUBJECT = "Email confirmation";
        public const string CONFIRM_MESSAGE_PART_BEFORE_URL = "Hi,\nThanks for using Dream Food Delivery! \n\nPlease confirm your email address by ";
        public const string CONFIRM_MESSAGE_URL_TEXT = "clicking on this link. ";
        public const string CONFIRM_MESSAGE_PART_AFTER_URL = " We'll communicate with you from time to time via email " +
                                                               "so it's important that we have an up - to - date email address on your profile." +
                                                               " \nIf you did not sign up for a Dream Food Delivery account please disregard this email." +
                                                               " \n\n\nHappy eating time! \nThe Dream Food Delivery";

        public const string PASSWORD_SUBJECT = "Password reset";
        public const string PASSWORD_MESSAGE_PART_BEFORE_URL = "Hi,\nWe received a password reset request. Create a new one by ";
        public const string PASSWORD_MESSAGE_URL_TEXT = "clicking on the link.";
        public const string PASSWORD_MESSAGE_PART_AFTER_URL = "\nIf request wasn't sent by you, please ignore or delete this letter." +
                                                              "\nIf you suspect a hack, please contact us one at a time by phone." +
                                                              "\n\n\nHappy eating time! \nThe Dream Food Delivery";

        public const string PASSWORD_RESET_SUBJECT = "Password reset";
        public const string PASSWORD_RESET_LINK = "Hi, Password was successfully reset! Сlick here to navigate to the website.";
        public const string PASSWORD_RESET_MESSAGE = "\n\n\nHappy eating time! \nThe Dream Food Delivery";

        public const string ROLE_SUBJECT = "Role changed";
        public const string ROLE_MESSAGE_FOR_ADMIN = "You are administrator now! Do not reply to this message, it was generated automatically!";
        public const string ROLE_MESSAGE_FOR_USER = "You are simple user now! Do not reply to this message, it was generated automatically!";

        public const string EMAIL_SUBJECT = "Email changed";
        public const string EMAIL_MESSAGE = "\nHi, and we hope you are familiar with this email. \nOtherwise, please contact us by phone." +
                                            "\nBecause we received a request for his change of mail from this to the above. " +
                                            "\n\n\nHappy eating time! \nThe Dream Food Delivery";

        public const string NEW_ORDER_SUBJECT = "New order!";
        public const string NEW_ORDER_MESSAGE = "Hi, we have a new order!\n\n\nHappy eating time! \nThe Dream Food Delivery";
    }
}
