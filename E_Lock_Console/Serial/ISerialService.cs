using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Lock_Console.Serial
{
    interface ISerialService
    {
        event EventHandler<OnReceiveEventArgs> OnReceive;

        bool Send();
    }
}
