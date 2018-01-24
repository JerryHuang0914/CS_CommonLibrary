using System;

namespace jh.csharp.AndroidCmdLibrary
{
    public class Actions : DeviceComponent
    {
        private Device device;
        public Actions(Device device)
        {
            this.device = device;
        }

        public void Click_InsLib(object X, object Y)
        {
            Click(Convert.ToInt32(X), Convert.ToInt32(Y));
        }

        public void Click(int X, int Y)
        {
            String cmd = "-s " + device.ID + " shell input tap " + X.ToString() + " " + Y.ToString();
            ADB_Process.RunAdbCommand(cmd);
        }
        
        public void Swip_InsLib(object fromX, object formY, object toX, object toY)
        {
            Swip(Convert.ToInt32(fromX),
                 Convert.ToInt32(formY),
                 Convert.ToInt32(toX),
                 Convert.ToInt32(toY));
        }
        public void Swip(int fromX, int fromY, int toX, int toY)
        {
            throw (new NotImplementedException());
        }

        public void Swip_InsLib(object fromX, object formY, object toX, object toY, object duration)
        {
            Swip(Convert.ToInt32(fromX),
                 Convert.ToInt32(formY),
                 Convert.ToInt32(toX),
                 Convert.ToInt32(toY),
                 Convert.ToInt32(duration));
        }
        public void Swip(int fromX, int fromY, int toX, int toY, int duration)
        {
            throw (new NotImplementedException());
        }

        public void KeyPress_InsLib(object keycode)
        {
            KeyPress(Convert.ToInt32(keycode));
        }
        public void KeyPress(int keycode)
        {
            String cmd = "-s " + device.ID + " shell input keyevent " + keycode.ToString();
            ADB_Process.RunAdbCommand(cmd);
        }

        internal void KeyPress_HOME()
        {
            KeyPress_InsLib(AndroidKeyEvents.KEYCODE_HOME);
        }
        internal void KeyPress_BACK()
        {
            KeyPress_InsLib(AndroidKeyEvents.KEYCODE_BACK);
        }
        internal void KeyPress_MENU()
        {
            KeyPress_InsLib(AndroidKeyEvents.KEYCODE_MENU);
        }
        internal void KeyPress_CALL()
        {
            KeyPress_InsLib(AndroidKeyEvents.KEYCODE_CALL); 
        }
        internal void KeyPress_ENDCALL()
        {
            KeyPress_InsLib(AndroidKeyEvents.KEYCODE_ENDCALL);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
