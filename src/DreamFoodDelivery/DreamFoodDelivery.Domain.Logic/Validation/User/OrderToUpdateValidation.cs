﻿using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class OrderToUpdateValidation : AbstractValidator<OrderToUpdate>
    {
        public OrderToUpdateValidation()
        {
            RuleFor(_ => _.Id).Must(id => Guid.TryParse(id, out var _))
                .WithMessage("Order id can't parse to Guid type");
            RuleFor(_ => _.Address).MinimumLength(3).MaximumLength(90)
                .WithMessage("Address must contain from 3 to 90 characters.");
            RuleFor(_ => _.PhoneNumber).Length(10,13)
                .WithMessage("Phone must contain from 10 to 13 characters.");
            RuleFor(_ => _.Name).MinimumLength(3).MaximumLength(90)
                .WithMessage("Name must contain from 3 to 90 characters.");
            RuleFor(_ => _.Surname).MinimumLength(3).MaximumLength(90)
                .WithMessage("Surname must contain from 3 to 90 characters.");
        }
    }
}
