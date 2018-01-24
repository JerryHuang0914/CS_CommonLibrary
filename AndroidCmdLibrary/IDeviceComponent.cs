using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace jh.csharp.AndroidCmdLibrary
{
    public interface IDeviceComponent : IDisposable
    {
        MethodInfo[] Methods { get; }
        MethodInfo GetMethod(String methodName,params object[]parameters);
        Dictionary<String, IDeviceComponent> DeviceComponents { get; }
    }
}
