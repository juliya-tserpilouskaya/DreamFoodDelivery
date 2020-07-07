using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class DishByPriceValidation : AbstractValidator<DishByPrice>
    {
        public DishByPriceValidation()
        {
            RuleFor(_ => _.LowerPrice).GreaterThanOrEqualTo(0)
                .WithMessage("Lower price limit must be greater than 0");
            RuleFor(_ => _.UpperPrice).GreaterThanOrEqualTo(0)
                .WithMessage("Upper price limit must be equal to or greater than 0");
        }
    }
}
