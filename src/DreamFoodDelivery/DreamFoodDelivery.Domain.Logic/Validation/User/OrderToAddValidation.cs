using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class OrderToAddValidation : AbstractValidator<OrderToAdd>
    {
        public OrderToAddValidation()
        {
            //RuleFor(_ => _.BasketId).NotEmpty().WithMessage("You must enter basket Id");
            When(_ => _.IsInfoFromProfile == false, () => {
                RuleFor(_ => _.Address).MinimumLength(3).MaximumLength(90)
                    .WithMessage("Address must contain from 3 to 90 characters.");
                RuleFor(_ => _.PhoneNumber).Length(10, 13)
                    .WithMessage("Phone must contain from 10 to 13 characters.");
                RuleFor(_ => _.Name).MinimumLength(3).MaximumLength(90)
                    .WithMessage("Name must contain from 3 to 90 characters.");
                RuleFor(_ => _.Surname).MinimumLength(3).MaximumLength(90)
                    .WithMessage("Surname must contain from 3 to 90 characters.");
            });
        }
    }
}
