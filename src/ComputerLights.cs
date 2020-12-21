using System;
using System.Collections.Generic;
using RGB.NET.Core;
using RGB.NET.Devices.Asus;
using RGB.NET.Devices.CoolerMaster;
using RGB.NET.Devices.Corsair;
using RGB.NET.Devices.DMX;
using RGB.NET.Devices.Debug;
using RGB.NET.Devices.Logitech;
using RGB.NET.Devices.Msi;
using RGB.NET.Devices.Novation;
using RGB.NET.Devices.Razer;
using RGB.NET.Devices.SteelSeries;

namespace control_rgb_activity {
    public class ComputerLights {
        private RGBSurface surface;

        public ComputerLights() {
            this.surface = RGBSurface.Instance;
            this.surface.Exception += (args) => Console.WriteLine(args.Exception.Message);
            LoadDevices();
        }

        private void LoadDevices() {
            this.surface.LoadDevices(AsusDeviceProvider.Instance);
            this.surface.LoadDevices(CoolerMasterDeviceProvider.Instance);
            this.surface.LoadDevices(CorsairDeviceProvider.Instance);
            this.surface.LoadDevices(DMXDeviceProvider.Instance);
            this.surface.LoadDevices(DebugDeviceProvider.Instance);
            this.surface.LoadDevices(LogitechDeviceProvider.Instance);
            this.surface.LoadDevices(MsiDeviceProvider.Instance);
            this.surface.LoadDevices(NovationDeviceProvider.Instance);
            this.surface.LoadDevices(RazerDeviceProvider.Instance);
            this.surface.LoadDevices(SteelSeriesDeviceProvider.Instance);
        }

        public void PrintDeviceInfo() {
            var hwlist = new List<string>();
            foreach (var device in this.surface.Devices) {
                hwlist.Add(device.ToString());
            }
            Console.WriteLine("Found {0} RGB devices: {1}", 
                hwlist.Count, String.Join(", ", hwlist));
        }

    }
}