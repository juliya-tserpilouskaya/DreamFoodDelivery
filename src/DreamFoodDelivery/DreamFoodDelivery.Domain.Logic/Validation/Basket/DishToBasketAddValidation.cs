using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class DishToBasketAddValidation : AbstractValidator<DishToBasketAdd>
    {
        public DishToBasketAddValidation()
        {
            RuleFor(_ => _.DishId).Must(id => Guid.TryParse(id, out var _))
                .WithMessage("Dish id can't parse to Guid type");
            RuleFor(_ => _.Quantity).GreaterThanOrEqualTo(1).WithMessage("Quantity must be greater than or equal to 1");
        }
    }
}
