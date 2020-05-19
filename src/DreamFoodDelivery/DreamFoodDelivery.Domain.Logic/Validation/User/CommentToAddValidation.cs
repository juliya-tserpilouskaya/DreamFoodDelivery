using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class CommentToAddValidation : AbstractValidator<CommentToAdd>
    {
        public CommentToAddValidation()
        {
            //RuleFor(_ => _.UserId).NotEmpty().WithMessage("You must enter user Id");
            RuleFor(_ => _.OrderId).Must(id => Guid.TryParse(id, out var _))
                .WithMessage("Order id can't parse to Guid type");
            RuleFor(_ => _.Headline).MinimumLength(3).MaximumLength(90)
                .WithMessage("Headline must contain from 3 to 90 characters.");
            RuleFor(_ => _.Content).MinimumLength(3).MaximumLength(90)
                .WithMessage("Content must contain from 3 to 90 characters.");
        }
    }
}
