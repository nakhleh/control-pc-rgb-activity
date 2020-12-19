using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using LibreHardwareMonitor.Hardware;

namespace control_rgb_activity {
    public class ComputerUpdateVisitor : IVisitor {
        public void VisitComputer(IComputer computer) {
            computer.Traverse(this);
        }
        public void VisitHardware(IHardware hw) {
            hw.Update();
            foreach (IHardware shw in hw.SubHardware) {
                shw.Accept(this);
            }
        }
        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }

    class Program {
        static void Main(string[] args) {
            Computer computer = new Computer {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                //IsNetworkEnabled = true,
                //IsStorageEnabled = true,
            };

            computer.Open();
            computer.Accept(new ComputerUpdateVisitor());
            ListPrimaryHardware(computer);
          //ListAll(computer);
            computer.Close();

            while (true) {
                computer.Open();
                computer.Accept(new ComputerUpdateVisitor());
                var stats = new Dictionary<string, float>();
                foreach (IHardware hw in computer.Hardware) {
                    if (hw.HardwareType == HardwareType.Cpu) {
                        stats.Add("cpuLoad", GetCpuLoad(hw));
                    }
                    if (hw.HardwareType == HardwareType.GpuNvidia || hw.HardwareType == HardwareType.GpuAmd) {
                        stats.Add("gpuLoad", GetGpuLoad(hw));
                    }
                    if (hw.HardwareType == HardwareType.Memory) {
                        stats.Add("memUsed", GetMemoryUsed(hw));
                    }
                }
                Console.WriteLine("CPU: {0}, GPU: {1}, RAM: {2}", stats["cpuLoad"], stats["gpuLoad"], stats["memUsed"]);
                computer.Close();
                System.Threading.Thread.Sleep(1000);
            }

        }


        static float GetCpuLoad(IHardware cpu) {
            return GetLoadValue(cpu, "Total");
        }

        static float GetGpuLoad(IHardware gpu) {
            return GetLoadValue(gpu, "Core");
        }

        static float GetMemoryUsed(IHardware ram) {
            return GetLoadValue(ram, "^Memory");
        }

        static float GetLoadValue(IHardware hw, string pattern) {
            if (hw.SubHardware.Length > 0) {
                Console.WriteLine("**WARNING: Found subhardware!");
            }
            Boolean found = false;
            float load = 0;
            foreach (ISensor sensor in hw.Sensors) {
                if (sensor.SensorType == SensorType.Load && Regex.IsMatch(sensor.Name, pattern)) {
                    found = true;
                    load = sensor.Value ?? default(float);
                }
            }
            if (! found) {
                Console.WriteLine("**WARNING: Couldn't find a load sensor matching \"{0}\"", pattern);
            }
            return load;
        }

        static void ListPrimaryHardware(Computer computer) {
            var hwlist = new List<string>();
            foreach (IHardware hw in computer.Hardware) {
                hwlist.Add($"{hw.Name} ({hw.HardwareType})");
            }
            Console.WriteLine("Found {0} hardware items: {1}", 
                hwlist.Count, String.Join(", ", hwlist));
        }

        static void ListAll(Computer computer) {
            foreach (IHardware hw in computer.Hardware) {
                Console.WriteLine("Hardware: {0}", hw.Name);
                foreach (IHardware shw in hw.SubHardware) {
                    Console.WriteLine("\tSubhardware: {0}", shw.Name);
                    foreach (ISensor sensor in shw.Sensors) {
                        Console.WriteLine("\t\tSensor: {0} ({1}), value: {2}", sensor.Name, sensor.SensorType, sensor.Value);
                    }
                }
                foreach (ISensor sensor in hw.Sensors) {
                    Console.WriteLine("\tSensor: {0} ({1}), value: {2}", sensor.Name, sensor.SensorType, sensor.Value);
                }
            }
        }
    }
}
