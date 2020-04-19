using DreamFoodDelivery.Common;
using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class OrderToStatusUpdateValidation : AbstractValidator<OrderToStatusUpdate>
    {
        public OrderToStatusUpdateValidation()
        {
            RuleFor(_ => _.Id).NotEmpty().WithMessage("You must enter order Id");
            RuleFor(_ => _.StatusIndex).GreaterThan(0).LessThan(Enum.GetNames(typeof(OrderStatuses)).Length)
                .WithMessage("StatusIndex must be in range");
        }
    }
}
