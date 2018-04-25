using lib60870.CS101;
using lib60870.CS104;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace IEC104DB
{
    public class IEC104Connection : IEC104Destination
    {
        Connection _connection;
        private int _port = 2404;
        private string _ip;

        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {

                if (IPAddress.TryParse(value, out IPAddress ip) || value == "")
                {
                    _ip = value;
                }
                else throw new ArgumentException("IP адрес указан неверно");
            }
        }

        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (value > 0)
                {
                    _port = value;
                }
                else throw new ArgumentException("Порт задан неверно");
            }
        }

        public void Connect()
        { 
            
            _connection = new Connection(IP, 2405);
            _connection.Autostart = true;
            _connection.DebugOutput = false;
            _connection.SetASDUReceivedHandler(_asduReceivedHandler, null);

            _connection.Connect();

            SubscribeOnSendingParametersChange();
        }

        

        internal override void Send(IEC104Parameter data)
        {
            try { 
            ASDU asdu = new ASDU(_alp, CauseOfTransmission.SPONTANEOUS, false, false, 0, 0, false);
            asdu.AddInformationObject(new MeasuredValueShort(data.IOA, data.Value, new QualityDescriptor()));
            _connection.SendASDU(asdu);
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEC104Connection(string ip, int port)
        {
            IP = ip;
            Port = port;
        }

        public IEC104Connection() { }
    }
}
