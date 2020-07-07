using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class DishToUpdateValidation : AbstractValidator<DishToUpdate>
    {
        public DishToUpdateValidation()
        {
            RuleFor(_ => _.Id).NotEmpty().WithMessage("You must enter Id");
            RuleFor(_ => _.Name).MinimumLength(3).MaximumLength(90)
                .WithMessage("Name must contain from 3 to 90 characters.");
            RuleFor(_ => _.Composition).MinimumLength(10).MaximumLength(250)
                .WithMessage("Composition must contain from 10 to 250 characters.");
            RuleFor(_ => _.Description).MinimumLength(10).MaximumLength(250)
                .WithMessage("Description must contain from 10 to 250 characters.");
            RuleFor(_ => _.Weight).MinimumLength(3).MaximumLength(250)
                .WithMessage("Weight must contain from 5 to 250 characters.");
            RuleFor(_ => _.Price).GreaterThan(0)
                .WithMessage("Price must be greater than 0");
            RuleFor(_ => _.Sale).GreaterThanOrEqualTo(0)
                .WithMessage("Sale must be equal to or greater than 0");
            RuleForEach(_ => _.TagNames).SetValidator(new TagToAddValidation());
        }
    }
}