using System;
using System.Diagnostics;
using System.Threading;

namespace OpenBastard.Hosting.Iis7
{
    public static class WorkerProcess64
    {
        public static Iis7Server Server { get; set; }
        static void Main(string[] args)
        {
            //Debugger.Launch();

            Server = Iis7Starter.Start(args);
            Server.Start();
            Console.WriteLine("Ready");
            Console.ReadLine();
        }
    }
}