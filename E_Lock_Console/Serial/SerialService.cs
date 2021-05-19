using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace E_Lock_Console.Serial
{
    class SerialService : ISerialService
    {

        public event EventHandler<OnReceiveEventArgs> OnReceive;

        public SerialService()
        {
            Task t1 = new Task(_run);
            //t1.Start();
        }

        public bool Send()
        {
            throw new NotImplementedException();
        }

        private void _run()
        {
            int i = 0;
            while (true)
            {
                if(i > 10)
                {
                    OnReceiveEventArgs args = new OnReceiveEventArgs("message 1");
                    _received(args);
                    i = 0;
                }
                i++;
                Thread.Sleep(200);
            }
        }

        private void _received(OnReceiveEventArgs e)
        {
            OnReceive?.Invoke(this, e);
        }
    }
}
