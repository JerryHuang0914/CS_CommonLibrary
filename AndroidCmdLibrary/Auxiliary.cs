using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace jh.csharp.AndroidCmdLibrary
{
    public class Auxiliary : DeviceComponent
    {
        private Device device = null;
        public Auxiliary(Device device)
        {
            this.device = device;
        }


        public override void Dispose()
        {
            base.Dispose();
        }

        public void CaptureScreen_InsLib(Object path)
        {
            ADB_Process.CaptureScreen(device.ID, Convert.ToString(path));
        }

        public void StartLogcatProcess(String folderOnDevice)
        {
            StartLogcatProcess(folderOnDevice, false, false);
        }

        public void StartLogcatProcess(String folderOnDevice, bool isRecordRadioLog, bool isRecordEventLog)
        {
            ADB_Process.StartLogcatProcess(device.ID, folderOnDevice, isRecordRadioLog, isRecordEventLog);
        }

        public void KillLogcatprocess()
        {
            ADB_Process.KillLogcatProcess(device.ID);
        }

        public string GetScreenResolution_InsLib()
        {
            String stdOutput = "", stdError = "";
            String cmd = "-s " + device.ID + " shell dumpsys window";
            ADB_Process.RunAdbCommand(cmd, out stdOutput,out stdError);
            Regex rgx = new Regex(@"(.+|)((i|I)(n|N)(i|I)(t|T)=)(?<Resolution>(\s|)(\d+)(\s|)(x|X)(\s|)(\d+)(\s|))(.+|)");
            Match m = rgx.Match(stdOutput);
            if (m.Success)
            {
                stdOutput = m.Groups["Resolution"].Value.Replace(" ", "");
            }
            else
            {
                stdOutput = "UNKNOW";
            }
            return stdOutput;
        }   

    //    public ScreenResolution GetScreenResolution()
    //    {
    //        String result = "";
    //        String cmd = "-s " + device.ID + " shell dumpsys window";
    //        ADB_Process.RunAdbCommand(cmd, ref result);
    //        Regex rgx = new Regex(@"(.+|)((i|I)(n|N)(i|I)(t|T)=)(?<Resolution>(\s|)(\d+)(\s|)(x|X)(\s|)(\d+)(\s|))(.+|)");
    //        Match m = rgx.Match(result);
    //        if (m.Success)
    //        {
    //            String resolutionStr = m.Groups["Resolution"].Value.Replace(" ", "");
    //            try{
    //                String[] splitedStrs = resolutionStr.ToLower().Split('x');
    //                return new ScreenResolution(Convert.ToInt32(splitedStrs[0]), Convert.ToInt32(splitedStrs[1]));
    //            }
    //            catch{
    //                return new ScreenResolution(0,0);
    //            }
    //        }
    //        else
    //        {
    //            return new ScreenResolution(0,0);
    //        }           
    //    }
        
    }

    //public class ScreenResolution
    //{
    //    public int Width = 0;
    //    public int Height = 0;

    //    public ScreenResolution()
    //    {

    //    }
    //    public ScreenResolution(int width,int height)
    //    {
    //        Width = width;
    //        Height = height;
    //    }

    //    override public String ToString()
    //    {
    //        return Width.ToString()+"x"+Height.ToString();
    //    }
    //}
}
