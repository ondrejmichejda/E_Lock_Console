using System;
namespace E_Lock_Console.Command
{
    /// <summary>
    /// Any class that should be able to reeive commands must implement this.
    /// This method is called if a command for this implementing instance is found.
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// Is called if command is registered to this instance.
        /// </summary>
        /// <param name="command">The Command registered to this instane.</param>
        /// <param name="args">The payload.</param>
        void OnReceiveCommand(string command, object args);
    }
}
