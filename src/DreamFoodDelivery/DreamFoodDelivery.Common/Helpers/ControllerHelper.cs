using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Common
{
    public static class ControllerHelper
    {
        /// <summary>
        ///  Collect data for http request
        /// </summary>
        /// <param name="message">Costom message</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ProblemDetails CollectProblemDetailsPartialContent(this string message, HttpContext context)
        {
            ProblemDetails details = new ProblemDetails()
            {
                Detail = message,
                Title = "Partial Content",
                Type = "https://tools.ietf.org/html/rfc7233#section-4.1",
                Status = StatusCodes.Status206PartialContent,
                Instance = context.Request.Path,
            };
            return details;
        }
    }
}
