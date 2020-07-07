using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic
{
    public interface ITokenValidator
    {
        ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey);
    }
}
