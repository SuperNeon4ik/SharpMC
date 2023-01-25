using System;
using SharpMC.Enums;

namespace SharpMC
{
    public class Logger
    {
        public string Name { get; }
        
        public Logger(string name)
        {
            Name = name;
        }

        public void Log(LogLevel level, string m)
        {
            Console.ResetColor();
            if (level == LogLevel.INFO || level == LogLevel.DEBUG) Console.ForegroundColor = ConsoleColor.Gray;
            else if (level == LogLevel.WARN) Console.ForegroundColor = ConsoleColor.Yellow;
            else if (level == LogLevel.ERROR) Console.ForegroundColor = ConsoleColor.Red;

            var now = DateTime.Now;
            Console.WriteLine("[{0:dd.MM.yyyy HH:mm:ss}] [{1}] [{2}] {3}", now, Name, level, m);
        }
    }
}