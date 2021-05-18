using System;
namespace E_Lock_Console
{
    public static class Logger
    {
        static readonly string Delimeter = ";";

        /// <summary>
        /// Logs message with object identifier and time.
        /// </summary>
        /// <param name="logObject"></param>
        /// <param name="msg"></param>
        public static void Log(ILoggable logObject, string msg)
        {
            Console.WriteLine(DateTime.Now.ToString() + Delimeter + logObject.GetName() + Delimeter + msg);
        }
    }
}
