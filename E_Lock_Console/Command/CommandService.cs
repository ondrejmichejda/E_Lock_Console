using System;
using System.Collections.Generic;

namespace E_Lock_Console.Command
{
    /// <summary>
    /// Service covering all commands distribution.
    /// </summary>
    public class CommandService : ICommandService, ILoggable
    {
        

        private readonly Dictionary<string, List<ICommandReceiver>> receivers = new Dictionary<string, List<ICommandReceiver>>();

        /// <inheritdoc cref="ICommandService.AvailableCommands"/>
        public Dictionary<string, string> AvailableCommands { get; } = new Dictionary<string, string>()
        {
            { "help", "help" },
            { "exit", "exit" },
            { "command1", "command1 [arg]" },
            { "command2", "command2 [arg]" }
        };

        /// <summary>
        /// Initializes Command service.
        /// </summary>
        public CommandService()
        {
            Logger.Log(this, "Initialized");
        }

        ///<inheritdoc cref="ICommandService.RegisterReceiver(string, ICommandReceiver)"/>
        public void RegisterReceiver(string command, ICommandReceiver receiver)
        {
            if (ValidateCommand(command))
            {
                lock (receivers)
                {
                    if (!receivers.ContainsKey(command))
                    {
                        receivers.Add(command, new List<ICommandReceiver>());
                    }
                    receivers[command].Add(receiver);
                    Logger.Log(this, "Receiver " + receiver.GetType().Name + " registered on command: " + command);
                }
            }

        }

        ///<inheritdoc cref="ICommandService.TrySendCommand(string, object)"/>
        public bool TrySendCommand(string command, object args)
        {
            if (ValidateCommand(command))
            {
                lock (receivers)
                {
                    if (!receivers.ContainsKey(command))  //throw new ArgumentException("Command does not exist: " + command);
                    {
                        Logger.Log(this, $"Command '{command}' not registered for listening.");
                        return false;
                    }
                    else
                    {
                        Logger.Log(this, $"Command '{command}' successfuly received.");

                        //make a copy before accessing to ensure no nullpointers.
                        var curReceivers = new List<ICommandReceiver>(receivers[command]);

                        try
                        {
                            foreach (ICommandReceiver receiver in curReceivers)
                            {
                                receiver?.OnReceiveCommand(command, args);
                            }
                        }
                        //In case the command does not exist anymore.
                        catch (Exception)
                        {
                            Logger.Log(this, $"Command '{command}' was deleted before it could receive a message.");
                            return false;
                        }
                    }
                }
                return true;
            }
            Logger.Log(this, "Command not recognized.");

            return false;
        }


        ///<inheritdoc cref="ICommandService.TryUnregisterReceiver(string, ICommandReceiver)"/>
        /// <exception cref="Exception">Thrown if the parameters are inexistent in the stored subscriptions.</exception>
        public bool TryUnregisterReceiver(string command, ICommandReceiver receiver)
        {
            try
            {
                lock (receivers)
                {
                    if (!receivers.ContainsKey(command)) throw new ArgumentException($"Command '{command}' does not exist.");
                    if (!receivers[command].Contains(receiver)) throw new ArgumentException("Receiver is not subscribed to command '{command}'.");
                    receivers[command].Remove(receiver);
                    Logger.Log(this, $"Receiver unregistered on command '{command}'.");
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            receivers.Clear();
        }

        /// <summary>
        /// Check whether command exist in command list.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>True if command recognized.</returns>
        private bool ValidateCommand(string command)
        {
            if (AvailableCommands.ContainsKey(command))
            {
                return true;
            }
            else
            {
                Logger.Log(this, $"Command '{command}' not recognized.");
                return false;
            }
        }

        public string GetName()
        {
            return "Command Service";
        }
    }
}
