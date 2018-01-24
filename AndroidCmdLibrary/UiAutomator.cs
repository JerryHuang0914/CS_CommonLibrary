using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jh.csharp.AndroidCmdLibrary
{
    public class UiAutomator : DeviceComponent
    {
        private Device device;
        public UiAutomator(Device device)
        {
            this.device = device;
        }
    }
}
