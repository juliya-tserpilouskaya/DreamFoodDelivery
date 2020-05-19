using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class TagToUpdateValidation : AbstractValidator<TagToUpdate>
    {
        public TagToUpdateValidation()
        {
            RuleFor(_ => _.Id).Must(id => Guid.TryParse(id, out var _))
                .WithMessage("Tag id can't parse to Guid type");
            //RuleFor(_ => _.IndexNumber).GreaterThanOrEqualTo(0)
            //    .WithMessage("Index number must be equal to or greater than 0");
            RuleFor(_ => _.TagName).NotEmpty().WithMessage("You must enter tag");
        }
    }
}
