using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace dev.jerry_h.pc_tools.CommonLibrary
{
    public class JHWaitoneEventHandle
    {
        public bool IsWaiting
        {
            get { return waiting; }
        }
        private bool waiting = false;
        private EventWaitHandle handler;

        public JHWaitoneEventHandle(bool initialStatus)
        {
            handler = new AutoResetEvent(initialStatus);
        }

        public bool WaitOne()
        {
            if (!waiting && handler!=null)
            {
                waiting = true;
                try
                {
                    return handler.WaitOne();
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool WaitOne(int millisecondsTimeout)
        {
            if (!waiting && handler != null)
            {
                waiting = true;
                try
                {
                    return handler.WaitOne(millisecondsTimeout);
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public bool Set()
        {
            if (waiting)
            {
                waiting = false;
                return handler.Set();
            }
            else
            {
                return false;
            }
        }

        public bool Reset()
        {
            waiting = false;
            return handler.Reset();
        }
    }
}
