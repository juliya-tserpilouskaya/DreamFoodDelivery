using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class ReviewToUpdateValidation : AbstractValidator<ReviewToUpdate>
    {
        public ReviewToUpdateValidation()
        {
            RuleFor(_ => _.Id).Must(id => Guid.TryParse(id, out var _))
                .WithMessage("Review id can't parse to Guid type");
            RuleFor(_ => _.Headline).MinimumLength(3).MaximumLength(90)
                .WithMessage("Headline must contain from 3 to 90 characters.");
            RuleFor(_ => _.Content).MinimumLength(3).MaximumLength(511)
                .WithMessage("Content must contain from 3 to 511 characters.");
        }
    }
}
