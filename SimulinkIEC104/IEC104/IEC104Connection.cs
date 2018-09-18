using lib60870;
using lib60870.CS101;
using lib60870.CS104;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;

namespace SimulinkIEC104
{
    public class IEC104Connection : IEC104Destination
    {
        Connection _connection;
        private string _ip;
        Timer _retryToConnectTimer;

        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {

                if ( (IPAddress.TryParse(value, out IPAddress ip) && ip.ToString()==value) || value == "")
                {
                    _ip = value;
                }
                else throw new WrongDataException("IP адрес указан неверно");
            }
        }



        public override void Initialize()
        { 
             
            _connection = new Connection(IP, Port);
            _connection.Autostart = true;
            _connection.DebugOutput = false;
            _connection.SetASDUReceivedHandler(_asduReceivedHandler, null);
            _connection.SetConnectionHandler(OnConnection, null);
            SubscribeOnSendingParametersChange();

            _retryToConnectTimer = new Timer(5000);
            _retryToConnectTimer.Elapsed += RetryToConnect;
            Connect();
        }

        private void RetryToConnect(object sender, ElapsedEventArgs e)
        {
            Connect();
        }

        private void OnConnection(object parameter, ConnectionEvent connectionEvent)
        {
            if (connectionEvent == ConnectionEvent.OPENED)
            {
                _retryToConnectTimer.Enabled = false;
                Console.WriteLine("Соедиенение установлено с узлом " + IP + ":" + Port);
            }
            else if (connectionEvent == ConnectionEvent.CLOSED)
            {
                _retryToConnectTimer.Enabled = true;
                Console.WriteLine("Соединение с узлом " + IP + ":" + Port + " потеряно, повторное соединение через 5 секунд");
            }
        }

        private void Connect()
        {
            try
            {
                _connection.Connect();
                _retryToConnectTimer.Enabled = false;

            }
            catch (ConnectionException)
            {
                Console.WriteLine("Попытка соедениться с узлом " + IP + ":" + Port + " не удалась, повтор через 5 секунд");
                _retryToConnectTimer.Enabled = true;
            }
        }

        

        internal override void Send(IEC104SendParameter data)
        {

            ASDU asdu = new ASDU(_alp, CauseOfTransmission.SPONTANEOUS, false, false, 0, data.Ca.CA, false);
            asdu.AddInformationObject(new MeasuredValueShort(data.IOA, data.Value, new QualityDescriptor()));
            _connection.SendASDU(asdu);

        }

        public IEC104Connection(string name) : base(name) {  }

        public IEC104Connection(string ip, int port):base()
        {
            IP = ip;
            Port = port;
        }

        public IEC104Connection():base() { }
    }
}
