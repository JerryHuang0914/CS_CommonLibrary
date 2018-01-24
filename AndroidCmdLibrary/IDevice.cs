using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace jh.csharp.AndroidCmdLibrary
{
    public interface IDevice : IDisposable,IDeviceComponent
    {
        String Platform
        {
            get;
        }

        String ID
        {
            get;            
        }

        bool IsConnected
        {
            get;
        }
    }
}
