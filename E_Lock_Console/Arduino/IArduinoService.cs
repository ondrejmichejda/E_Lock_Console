using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Lock_Console.Command;

namespace E_Lock_Console.Arduino
{
    interface IArduinoService : IDisposable, ILoggable, ICommandReceiver
    {

    }
}
