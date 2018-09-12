using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Serialization;

namespace SimulinkIEC104
{
    public delegate void DebugMessageHadler(Destination destination, string message, byte debugLevel);

    public class Destination
    {
        
        private int _remotePort;
        private int _localPort;
        private IPAddress _ip;
        private IPEndPoint _ipendpoint;
        private int? _receivingPacketSize = null;
        private Thread _receivingThread;
        private bool _stopReceive = false;


        public Destination() { }
        public Destination(string name)
        {
            Name = name;
        }


        public string Name { get; set; }

        public int LocalPort
        {
            get
            {
                return _localPort;
            }
            set
            {
                if (_localPort == value) return;

                if (_udpClient != null)
                {
                    _udpClient.Close();
                    _udpClient = null;
                }

                _localPort = value;
            }
        }

        public int RemotePort
        {
            get
            {
                return _remotePort;
            }
            set
            {
                if (_remotePort == value) return;

                if (_ipendpoint != null) _ipendpoint = null;

                _remotePort = value;

            }
        }

        public string IP
        {
            get
            {
                if (_ip != null)
                    return _ip.ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (IP == value) return;

                if (IPAddress.TryParse(value, out _ip))
                {
                    _ipendpoint = null;
                }
                else
                    throw new ArgumentException("Данную строку невозможно преобразовать в IP адресс");

            }
        }

        private DebugMessageHadler _debugMessageHadler = null;

        public void AddDebugMessageHandler(DebugMessageHadler handler)
        {
            this._debugMessageHadler += handler;
        }

        private void _debugMessage(string message, byte debugLevel)
        {
            _debugMessageHadler?.Invoke(this, message, debugLevel);
        }


        [XmlIgnore]
        public int ReceivingPacketSize
        {
            get
            {
                if (!_receivingPacketSize.HasValue)
                {
                    _receivingPacketSize = 0;
                    for (int i = 0; i < ReceivingParameters.Count; i++)
                        _receivingPacketSize += ReceivingParameters[i].Bits;
                }
                return (int)_receivingPacketSize;
            }
        }

        [XmlIgnore]
        private IPEndPoint IPEndPoint
        {
            get
            {
                if (_ipendpoint == null)
                {
                    try
                    {
                        if (RemotePort != 0)
                            _ipendpoint = new IPEndPoint(IPAddress.Parse(IP), RemotePort);
                    }
                    catch { }
                }

                return _ipendpoint;
            }
        }

        
        private UdpClient _udpClient;
        
        public bool UdpClientStart()
        {
            
            try
            {
                if (_udpClient != null)
                {
                    _udpClient.Close();
                    _udpClient = null;
                }
                if (LocalPort != 0)
                {
                    _udpClient = new UdpClient(LocalPort);
                    
                }

            }
            catch { _udpClient = null; }

            if (_udpClient != null)
            {
                if (ReceivingParameters.Count > 0) _receivingThread = new Thread(_receivingMethod);

                foreach (var param in SendingParameters)
                {
                    param.AddValueChangedHandler(_resendAllSendingParameters);
                }

                _stopReceive = false;
                _receivingThread.Start();
                return true;
            }
            else
                return false;
        }

        private void _resendAllSendingParameters(Parameter data)
        {
            byte[] result = new byte[0];
            foreach (var param in SendingParameters)
            {
                byte[] paramBytes = param.GetBytes();
                byte[] summ = new byte[result.Length + paramBytes.Length];
                result.CopyTo(summ, 0);
                paramBytes.CopyTo(summ, result.Length);
                result = summ;
            }
            
            int sended1 =_udpClient.Send(result, result.Length, IPEndPoint);/*
            int sended2 = _udpClient.Send(result, result.Length, IPEndPoint);
            int sended3 = _udpClient.Send(result, result.Length, IPEndPoint);*/

            if (sended1 == result.Length/* || sended2 == result.Length || sended3 == result.Length*/)
            {
                _debugMessage("Отправлен пакет на узел: " + IPEndPoint.ToString(), 4);
            }
            else
            {
                _debugMessage("Не удалось отправить пакет на узел: " + IPEndPoint.ToString(), 2);
            }
        }

        private void _receivingMethod()
        {
            _debugMessage("Поток получения запущен " + LocalPort, 7);
            
                IPEndPoint ipendpoint = null;
                byte[] message;
                int currentPosition;
                while (!_stopReceive)
                {
                    ipendpoint = null;
                    message = null;

                while (message == null)
                {
                    try
                    {
                        message = _udpClient.Receive(ref ipendpoint);
                        if (!ipendpoint.Equals(IPEndPoint))
                        {
                            _debugMessage("Полученно сообщение с неизвестного сокета, сообщение проигнорированно", 5);
                            message = null;
                        }
                    }
                    catch (SocketException)
                    {
                        _udpClient.Close();
                        _udpClient = new UdpClient(LocalPort);
                        message = null;
                    }
                }

                    if (message.Length != ReceivingPacketSize)
                    {
                        _debugMessage(LocalPort + " " + ipendpoint.Address + ":" + ipendpoint.Port +
                            " Принят пакет неверного размера, принято: " + message.Length + ", а ожидалось: " + ReceivingPacketSize, 3);
                    }
                    else
                    {
                        currentPosition = 0;
                        for (int i = 0; i < ReceivingParameters.Count; i++)
                        {
                            ReceivingParameters[i].SetValueFromBytes(message, currentPosition);

                            currentPosition += ReceivingParameters[i].Bits;
                        }
                    }
                }
            
        }

        public void UdpClientClose()
        {
            _stopReceive = true;
            if (_udpClient != null) _udpClient.Close();
            _udpClient = null;
            if (_receivingThread != null) _receivingThread.Join();
        }

        public override string ToString()
        {
            return Name + " " + (_ip == null ? "noIP": _ip.ToString())+":"+_remotePort;
        }

        public BindingList<ReceivingParameter> ReceivingParameters { get; set; } = new BindingList<ReceivingParameter>();
        public BindingList<SendingParameter> SendingParameters { get; set; } = new BindingList<SendingParameter>();
    }
}
