using System;
using E_Lock_Console.Command;
using E_Lock_Console.Serial;

namespace E_Lock_Console.Arduino
{
    class ArduinoService : IArduinoService
    {
        private readonly ISerialService _serialService;
        private readonly ICommandService _commandService;

        private const char DELIMITER = ';';
        private const char END = '#';
        private const string COMMPWD = "139";

        public ArduinoService(ICommandService commandService, ISerialService serialService)
        {
            _commandService = commandService;
            _serialService = serialService;

            _commandService.RegisterReceiver("settime", this);
            _commandService.RegisterReceiver("unlock", this);
            _commandService.RegisterReceiver("ledtest", this);
            _commandService.RegisterReceiver("getlog", this);
            _commandService.RegisterReceiver("adduser", this);

        }

        public void Dispose()
        {
            _commandService.TryUnregisterReceiver("settime", this);
            _commandService.TryUnregisterReceiver("unlock", this);
            _commandService.TryUnregisterReceiver("ledtest", this);
            _commandService.TryUnregisterReceiver("getlog", this);
            _commandService.TryUnregisterReceiver("adduser", this);
        }

        public void OnReceiveCommand(string command, object args)
        {
            switch (command)
            {
                case "settime":
                    string actualTime = _buildTimeString();
                    _serialService.Send(_buildCommand(Command.SETTIME, actualTime));
                    break;
                case "unlock":
                    _serialService.Send(_buildCommand(Command.UNLOCK));
                    break;
                case "ledtest":
                    if(args == null)
                    {
                        Logger.Log(this, "Missing argument"); return;
                    }
                    _serialService.Send(_buildCommand(Command.LEDTEST, args.ToString().Substring(1, 1)));
                    break;
                case "getlog":
                    throw new NotImplementedException();
                    break;
                case "adduser":
                    _serialService.Send(_buildCommand(Command.ADDUSER));
                    break;
            }
        }


        private string _buildTimeString()
        {
            DateTime dt = DateTime.Now;
            char sep = ':';
            string result = dt.Hour.ToString() + sep;
            result += dt.Minute.ToString() + sep;
            result += (dt.Second + 1).ToString() + sep;     //communication compensation
            result += dt.Day.ToString() + sep;
            result += dt.Month.ToString() + sep;
            result += dt.Year.ToString();

            return result;
        }

        /// <summary>
        /// Build command sent over serial port
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <returns>Built command</returns>
        private string _buildCommand(Command cmd, string args = "")
        {
            return COMMPWD + DELIMITER + (int)cmd + DELIMITER + args;
        }

        public string GetName()
        {
            return "Arduino Service";
        }
    }

    public enum Command
    {
        UNDEFINED = 0,
        SETTIME = 1,
        UNLOCK = 2,
        LEDTEST = 3,
        ADDUSER = 4,
        GETLOG = 9
    }
}
