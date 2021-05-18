using System;
using System.Collections.Generic;
using E_Lock_Console.Command;

namespace E_Lock_Console
{
    public class Core : ICommandReceiver, ILoggable
    {
        private readonly ICommandService _commandService;
        private bool _shutdownToken = false;

        public Core()
        {
            _commandService = new CommandService();

            _commandService.RegisterReceiver("help", this);
            _commandService.RegisterReceiver("exit", this);
            _commandService.RegisterReceiver("command1", this);
            _commandService.RegisterReceiver("command2", this);


            Logger.Log(this, "Application started, type \'help\' to show available commands");
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
                case "command1":
                    Logger.Log(this, "Processing command1 with arguments: " + args.ToString());
                    break;
                case "command2":
                    Logger.Log(this, "Processing command2 with arguments: " + args.ToString());
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
        }
        
    }
}
