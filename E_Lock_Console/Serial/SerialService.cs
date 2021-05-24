using System;
using System.Collections.Generic;
using System.IO.Ports;
using E_Lock_Console.Command;

namespace E_Lock_Console.Serial
{
    /// <summary>
    /// Service to process serial communication.
    /// </summary>
    class SerialService : ISerialService
    {

        public event EventHandler<OnReceiveEventArgs> OnReceive;

        private readonly ICommandService _commandService;

        private SerialPort _serialPort;

        /// <summary>
        /// Initializes serial service
        /// </summary>
        /// <param name="commandService"></param>
        public SerialService(ICommandService commandService)
        {
            _commandService = commandService;
            _serialPort = new SerialPort();
            _serialPort.BaudRate = 9600;

            _serialPort.DataReceived += _serialPort_DataReceived;

            _commandService.RegisterReceiver("connect", this);
            _commandService.RegisterReceiver("disconnect", this);
            _commandService.RegisterReceiver("portinfo", this);
        }

        /// <summary>
        /// Autoconnect com port if only 1 found
        /// </summary>
        public void AutoConnect()
        {
            Dictionary<int, string> ports = _getAvailablePorts();
            if(ports.Count == 1)
            {
                // Only 1 com port available, try to connect
                _connectPort(ports[1]);
            }
        }

        private void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Logger.Log(this, $"Serial received <<< {_serialPort.ReadLine()}");
        }

        /// <summary>
        /// Sends data over serial line
        /// </summary>
        /// <returns></returns>
        public void Send(string message)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(message);
                Logger.Log(this, $"Serial send >>> {message}");
            }
            else
            {
                Logger.Log(this, "Serial connection not initialized. Use \'connect\' first");
            }
        }

        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="e"></param>
        private void _received(OnReceiveEventArgs e)
        {
            OnReceive?.Invoke(this, e);
        }

        /// <summary>
        /// Command receiver interface method to handle registered commands.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        public void OnReceiveCommand(string command, object args)
        {
            switch (command)
            {
                case "connect":
                    Dictionary<int, string> portList = _getAvailablePorts();
                    foreach  (KeyValuePair<int,string> item in portList)
                    {
                        Console.WriteLine($"{item.Key}) {item.Value}");
                    }
                    int selectionNumber = 0;
                    ;

                    if(Int32.TryParse(Console.ReadLine(), out selectionNumber))
                    {
                        string portName;
                        if(portList.TryGetValue(selectionNumber, out portName))
                        {
                            _connectPort(portName);
                        }
                    }                 
                    break;

                case "disconnect":
                    _disconnectPort();
                    break;

                case "portinfo":
                    if (_serialPort.IsOpen)
                    {
                        Console.WriteLine("Serial port info:");
                        Console.WriteLine($"Port Name: {_serialPort.PortName}");
                        Console.WriteLine($"Baudrate: {_serialPort.BaudRate}");
                    }
                    else
                    {
                        Logger.Log(this, "Serial port not opened");
                    }
                    break;
            }
        }

        /// <summary>
        /// Returns list of available serial ports
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> _getAvailablePorts()
        {
            int i = 1;
            Dictionary<int, string> ports = new Dictionary<int, string>();
            foreach (string p in SerialPort.GetPortNames())
            {
                ports.Add(i, p);
                i++;
            }
            return ports;
        }

        /// <summary>
        /// Connects serial port.
        /// </summary>
        /// <param name="portName"></param>
        /// <returns>True if connection succesful.</returns>
        private bool _connectPort(string portName)
        {
            try
            {
                _serialPort.PortName = portName;
                _serialPort.Open();
                Logger.Log(this, $"Serial port {portName} succesfuly opened.");
                return true;
            }
            catch(Exception ex)
            {
                Logger.Log(this, $"Serial port {portName} cannot be opened: {ex.Message}");
                return false;
            }
            
        }

        /// <summary>
        /// Disconnects serial port.
        /// </summary>
        private void _disconnectPort()
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Close();
                    Logger.Log(this, $"Serial port {_serialPort.PortName} disconnected.");
                }
                catch (Exception ex)
                {
                    Logger.Log(this, $"Serial port {_serialPort.PortName} could not be closed: {ex.Message}");
                }
            }
            else
            {
                Logger.Log(this, "Nothing to disconnect...");
            }
        }

        /// <summary>
        /// Dispose this object properly.
        /// </summary>
        public void Dispose()
        {
            _commandService.TryUnregisterReceiver("connect", this);
            _commandService.TryUnregisterReceiver("disconnect", this);
            _commandService.TryUnregisterReceiver("portinfo", this);
            _disconnectPort();
        }

        public string GetName()
        {
            return "Serial Service";
        }
    }
}
