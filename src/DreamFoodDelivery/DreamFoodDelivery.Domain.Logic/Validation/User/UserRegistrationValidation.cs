using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    /////////////////////////////////////
    // For training. Сurrently unused. //
    /////////////////////////////////////

    public class UserRegistrationValidation : AbstractValidator<UserRegistration>
    {
        public UserRegistrationValidation()
        {
            RuleFor(_ => _.Email).EmailAddress();
        }
    }
}
