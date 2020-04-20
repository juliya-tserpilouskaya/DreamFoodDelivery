using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DreamFoodDelivery.Common
{
    public class LoggerAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Debug.WriteLine($"{DateTime.Now} - CLASS: {args.Method.DeclaringType.Name} \n" +
                $"METHOD: {args.Method.Name} started.");
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Debug.WriteLine($"{DateTime.Now} - {args.ReturnValue.ToString()} \n" +
                $"METHOD Finished.");
        }

        public override void OnException(MethodExecutionArgs args)
        {
            Debug.WriteLine($"{DateTime.Now} - CLASS: {args.Method.DeclaringType.Name} \n" +
                $"METHOD: {args.Method.Name} has exception: " +
                $"\n{args.Exception.Data}\n" +
                $"{args.Exception.Message}\n" +
                $"{args.Exception.InnerException?.Message}");
        }
    }
}
