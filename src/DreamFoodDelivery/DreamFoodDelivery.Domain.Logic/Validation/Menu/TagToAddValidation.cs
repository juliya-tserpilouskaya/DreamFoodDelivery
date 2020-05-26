using DreamFoodDelivery.Domain.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DreamFoodDelivery.Domain.Logic.Validation
{
    public class TagToAddValidation : AbstractValidator<TagToAdd>
    {
        public TagToAddValidation()
        {
            RuleFor(_ => _.TagName).NotEmpty().Must(_ => !_.Any(x => Char.IsWhiteSpace(x))).WithMessage("You must enter tag");
        }
    }
}
