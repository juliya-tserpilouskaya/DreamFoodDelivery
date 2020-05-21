using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class DishToAddValidation : AbstractValidator<DishToAdd>
    {
        public DishToAddValidation()
        {
            RuleFor(_ => _.Name).MinimumLength(3).MaximumLength(90)
                .WithMessage("Name must contain from 3 to 90 characters.");
            RuleFor(_ => _.Composition).MinimumLength(10).MaximumLength(250)
                .WithMessage("Composition must contain from 10 to 250 characters.");
            RuleFor(_ => _.Description).MinimumLength(10).MaximumLength(250)
                .WithMessage("Description must contain from 10 to 250 characters.");
            RuleFor(_ => _.Weigh).MinimumLength(5).MaximumLength(250)
                .WithMessage("Weigh must contain from 5 to 250 characters.");
            RuleFor(_ => _.Cost).GreaterThan(0)
                .WithMessage("Cost must be greater than 0");
            RuleFor(_ => _.Sale).GreaterThanOrEqualTo(0)
                .WithMessage("Sale must be equal to or greater than 0");
            RuleForEach(_ => _.TagNames).SetValidator(new TagToAddValidation());
        }
    }
}
