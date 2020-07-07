using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Web
{
    public class MyDetails : ProblemDetails
    {
        public void GetData(string s, object s2)
        {
            Extensions.Add( s,  s2);
        }
    }
}
