using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jh.csharp.AndroidCmdLibrary
{
    public class ProductInfo : DeviceComponent
    {
        private Device device;
        public readonly String Name = "";
        public readonly String Brand = "";
        public readonly String Board = "";
        public readonly String Model = "";
        public readonly String CPU_ABI = "";
        public readonly String CPU_ABI2 = "";
        public readonly String Manufacturer = "";
        public readonly String Language = "";
        public readonly String Region = "";
        public readonly String LibPath = "";
        public readonly String SolutionVendor = "";
        public ProductInfo(Device device)
        {
            try
            {
                this.device = device;
                String stdOutput = "", stdError = "";
                ADB_Process.RunAdbCommand(" -s " + device.ID + " shell cat /system/build.prop | grep \"product\"", out stdOutput,out stdError, false);
                foreach (String lineTemp in stdOutput.Split('\n'))
                {
                    String line = lineTemp.Trim();
                    try
                    {
                        if (line.Contains("model"))
                        {
                            Model = line.Split('=')[1];
                        }
                        else if (line.Contains("brand"))
                        {
                            Brand = line.Split('=')[1];
                        }
                        else if (line.Contains("name"))
                        {
                            Name = line.Split('=')[1];
                        }
                        else if (line.Contains("board"))
                        {
                            Board = line.Split('=')[1];
                        }
                        else if (line.Contains(".abi2="))
                        {
                            CPU_ABI2 = line.Split('=')[1];
                        }
                        else if (line.Contains(".abi="))
                        {
                            CPU_ABI = line.Split('=')[1];
                        }
                        else if (line.Contains("manufacturer"))
                        {
                            Manufacturer = line.Split('=')[1];
                        }
                        else if (line.Contains("language"))
                        {
                            Language = line.Split('=')[1];
                        }
                        else if (line.Contains("region"))
                        {
                            Region = line.Split('=')[1];
                        }
                    }
                    catch
                    {

                    }
                }
                ADB_Process.RunAdbCommand(" -s " + device.ID + " shell cat /system/build.prop | grep \"rild.libpath\"", out LibPath,out stdError, false);
                LibPath = LibPath.Trim();
                stdOutput = LibPath.ToLower();
                if (stdOutput.Length > 0)
                {
                    if (stdOutput.Contains("qc") || stdOutput.Contains("qualcomm"))
                    {
                        SolutionVendor = "Qualcomm";
                    }
                    else if(stdOutput.Contains("mtk") || stdOutput.Contains("mediatek")){
                        SolutionVendor="MTK";
                    }
                }
            }
            catch
            {

            }
        }
    }
}
