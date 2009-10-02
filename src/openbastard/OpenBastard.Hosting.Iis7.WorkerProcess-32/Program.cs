using System;
using System.Diagnostics;

namespace OpenBastard.Hosting.Iis7.WorkerProcess
{
    public static class WorkerProcess32
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