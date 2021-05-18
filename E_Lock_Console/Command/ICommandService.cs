using System;
using System.Collections.Generic;

namespace E_Lock_Console.Command
{
    /// <summary>
    /// Subscriber / Publisher pattern implementation where any class of type "ICommandReceiver" can register and receive messages.
    /// Sending messages is done using an instance of this ICommandService.
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Returns available commands.
        /// </summary>
        Dictionary<string, string> AvailableCommands { get; }

        /// <summary>
        /// Sends the command with its args to any subscriber that is registered on this command.
        /// </summary>
        /// <param name="command">The command to which the args should be sent.</param>
        /// <param name="args">Payload</param>
        /// <returns>False if no registered receiver is found on above command or command invalid.</returns>
        bool TrySendCommand(string command, object args);

        /// <summary>
        /// Registers a receiver on a specific command. Will receive messages if a message is sent to this command.
        /// </summary>
        /// <param name="command">The command to register.</param>
        /// <param name="receiver">The instance to which the message is sent if this command should be sent.</param>
        void RegisterReceiver(string command, ICommandReceiver receiver);

        /// <summary>
        /// Removes the registration of this command and receiver.
        /// </summary>
        /// <param name="command">The command to unregister.</param>
        /// <param name="receiver">The instance that should be removed.</param>
        /// <returns></returns>
        bool TryUnregisterReceiver(string command, ICommandReceiver receiver);
    }
}
