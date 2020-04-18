using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class TagToAddValidation : AbstractValidator<TagToAdd>
    {
        public TagToAddValidation()
        {
            RuleFor(_ => _.IndexNumber).GreaterThanOrEqualTo(0)
                .WithMessage("Index number must be equal to or greater than 0");
        }
    }
}
