using System;
using System.IO;

namespace SharpMC
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            SharpMC.Start();
            Console.ReadLine();
            SharpMC.Stop();
        }
    }
}