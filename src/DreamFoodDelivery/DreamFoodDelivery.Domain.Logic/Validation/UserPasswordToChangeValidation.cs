using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class UserPasswordToChangeValidation : AbstractValidator<UserPasswordToChange>
    {
        public UserPasswordToChangeValidation()
        {
            RuleFor(_ => _.IdFromIdentity).NotEmpty().WithMessage("You must enter identity Id");
        }
    }
}
