using DreamFoodDelivery.Common;
using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Common
{
    public class LoggerAttribute : OnMethodBoundaryAspect
    {
        public override void OnExit(MethodExecutionArgs args)
        {
            if (args.ReturnValue is Task<string> task)
            {
                args.ReturnValue = task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        return "An error happened: " + t.Exception.Message;
                    }

                    return t.Result;
                });
            }
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            string writePath = InitMyLogger();
            string message = $"{DateTime.Now} - CLASS: {args.Method.DeclaringType.Name} \n" +
                $"METHOD: {args.Method.Name} started.";
            Debug.WriteLine(message);
            LogWrite(message, writePath);

        }

        //public override void OnExit(MethodExecutionArgs args)
        //{


        //    //var type = args.ReturnValue.GetType();

        //    //if (args.ReturnValue is null)
        //    //{
        //    string writePath = InitMyLogger();
        //    string message = $"{DateTime.Now} - {args.ReturnValue.ToString()} \n" +
        //            $"METHOD Finished.";
        //        Debug.WriteLine(message);
        //    LogWrite(message, writePath);
        //    //}

        //}

        public override void OnException(MethodExecutionArgs args)
        {
            //string writePath = InitMyLogger();
            string message = $"{DateTime.Now} - CLASS: {args.Method.DeclaringType.Name} \n" +
                $"METHOD: {args.Method.Name} has exception: " +
                $"\n{args.Exception.Data}\n" +
                $"{args.Exception.Message}\n" +
                $"{args.Exception.InnerException?.Message}";
            Debug.WriteLine(message);
            //LogWrite(message, writePath);
        }

        private string InitMyLogger()
        {
            string logDir = Path.Combine(Environment.CurrentDirectory, @"..\..\..\" + @"\Log");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            string[] myFiles = Directory.GetFiles(logDir);
            int counter = 0;
            string writePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\" + $@"\Log\log {DateTime.Now:yyyyMMdd}_{counter}.txt");
            short logFileLenght = NumberСonstants.LOGGER_FILE_SIZE;
            int[] countNum = new int[myFiles.Length];

            if (File.Exists(writePath))
            {
                for (int i = 0; i < myFiles.Length; i++)
                {
                    string[] split2 = System.IO.Path.GetFileName(myFiles[i]).Split(new char[] { '_', '.' });
                    countNum[i] = Convert.ToInt32(split2[1]);
                }
                for (int i = 0; i < countNum.Length; i++)
                {
                    if (countNum[i] > counter)
                    {
                        counter = countNum[i];
                    }
                }

                writePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\" + $@"\Log\log {DateTime.Now:yyyyMMdd}_{counter}.txt");
                FileInfo fileInfo = new FileInfo(writePath);

                if (fileInfo.Length >= logFileLenght)
                {
                    counter += 1;
                }
                writePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\" + $@"\Log\log {DateTime.Now:yyyyMMdd}_{counter}.txt");
            }
            return writePath;
        }

        private async void LogWrite(string text, string writePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
                {
                    await sw.WriteLineAsync(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
