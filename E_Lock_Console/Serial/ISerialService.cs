using System;
using E_Lock_Console.Command;

namespace E_Lock_Console.Serial
{
    interface ISerialService : IDisposable, ILoggable, ICommandReceiver
    {
        event EventHandler<OnReceiveEventArgs> OnReceive;

        void Send(string message);

        bool AutoConnect();
    }
}
