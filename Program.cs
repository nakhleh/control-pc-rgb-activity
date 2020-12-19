using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace control_rgb_activity {
    class Program {
        static void Main(string[] args) {
            var activity = new ComputerActivity();
            activity.ListPrimaryHardware();
            activity.ListAll();
            while (true) {
                var stats = activity.GetActivity();
                Console.WriteLine("CPU: {0}, GPU: {1}, RAM: {2}", stats["cpuLoad"], stats["gpuLoad"], stats["memUsed"]);
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
