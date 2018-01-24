using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jh.csharp.CommonLibrary
{
    class SingleWait_EventWaitHandle : EventWaitHandle
    {
        public bool IsWaiting
        {
            get
            {
                return __is_waiting;
            }

            private set
            {
                __is_waiting = value;
            }
        }
        private bool __is_waiting = false;
        public SingleWait_EventWaitHandle(bool initialState = false, EventResetMode mode = EventResetMode.AutoReset, String name = "") : base(initialState, mode, name)
        {
            __is_waiting = initialState;
        }

        public new bool Set()
        {
            if (IsWaiting)
            {
                IsWaiting = false;
                return base.Set();                
            }
            else
            {
                return false;
            }
        }

        public new bool WaitOne()
        {
            if (!IsWaiting)
            {                
                IsWaiting = true;
                return base.WaitOne();
            }
            else
            {
                return false;
            }
        }

        public new bool Reset()
        {
            IsWaiting = false;
            return base.Reset();
        }
    }
}
