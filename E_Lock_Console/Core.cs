using System;
using System.Collections.Generic;
using E_Lock_Console.Arduino;
using E_Lock_Console.Command;
using E_Lock_Console.Serial;

namespace E_Lock_Console
{
    public class Core : ICommandReceiver, ILoggable
    {
        private readonly ICommandService _commandService;
        private readonly ISerialService _serialService;
        private readonly IArduinoService _arduinoService;

        private bool _shutdownToken = false;

        public Core()
        {
            _commandService = new CommandService();
            _serialService = new SerialService(_commandService);
            _arduinoService = new ArduinoService(_commandService, _serialService);

            _commandService.RegisterReceiver("help", this);
            _commandService.RegisterReceiver("exit", this);

            Logger.Log(this, "Application started, type \'help\' to show available commands");

            _serialService.AutoConnect();
        }

        public string GetName()
        {
            return "Core";
        }

        public void OnReceiveCommand(string command, object args)
        {
            switch (command)
            {
                case "help":
                    _printCommands();
                    break;
                case "exit":
                    _shutdown();
                    break;
            }
        }

        public void Run()
        {
            while (!_shutdownToken)
            {
                string command = Console.ReadLine();
                if (command != null)
                {
                    object args = null;
                    string[] commandSplit = command.Split(' ');
                    command = commandSplit[0];
                    args = commandSplit.Length > 1 ? commandSplit[1] : null;
                    if (!_commandService.TrySendCommand(command, args)) _printCommands();
                }
            }
        }

        private void _printCommands()
        {
            Logger.Log(this, "Available commands:");
            foreach (KeyValuePair<string, string> command in _commandService.AvailableCommands)
            {   
                Logger.Log(this, command.Value);
            }
        }

        private void _shutdown()
        {
            _shutdownToken = true;
            _arduinoService?.Dispose();
            _serialService?.Dispose();
            _commandService?.Dispose();
        }
        
    }
}
