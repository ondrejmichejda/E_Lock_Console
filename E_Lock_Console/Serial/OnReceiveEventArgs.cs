using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Lock_Console.Serial
{
    class OnReceiveEventArgs : EventArgs
    {
        public DateTime ActualTime;
        public string Message;

        public OnReceiveEventArgs(string msg)
        {
            Message = msg;
        }
    }
}
