using System;
namespace E_Lock_Console
{
    /// <summary>
    /// Class that require logging must implement this.
    /// </summary>
    public interface ILoggable
    {
        /// <summary>
        /// Get the name of class that implements this interface.
        /// </summary>
        /// <returns>Implementing classes name.</returns>
        string GetName();
    }
}
