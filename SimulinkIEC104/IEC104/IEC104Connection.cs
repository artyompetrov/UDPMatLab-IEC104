﻿using lib60870.CS101;
using lib60870.CS104;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SimulinkIEC104
{
    public class IEC104Connection : IEC104Destination
    {
        Connection _connection;
        private string _ip;

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

        public IEC104Connection(string name) : base(name) {  }

        public IEC104Connection(string ip, int port):base()
        {
            IP = ip;
            Port = port;
        }

        public IEC104Connection():base() { }
    }
}
