using lib60870.CS101;
using lib60870.CS104;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public class IEC104Server : IEC104Destination
    {
        Server _server;

        public IEC104Server(string name) : base(name) { }

        public IEC104Server() : base() { }

        internal override void Send(IEC104SendParameter data)
        {
            
            ASDU asdu = new ASDU(_alp, CauseOfTransmission.SPONTANEOUS, false, false, 0, data.Ca.CA, false);
            asdu.AddInformationObject(new MeasuredValueShort(data.IOA, data.Value, new QualityDescriptor()));
            _server.EnqueueASDU(asdu);
        }

        public override void Initialize()
        {
            _server = new Server(Port);
            _server.DebugOutput = false;
            _server.SetASDUHandler(ServerAsduReceivedHandler, null);

            SubscribeOnSendingParametersChange();
            _server.Start();
        }

        private bool ServerAsduReceivedHandler(object parameter, IMasterConnection connection, ASDU asdu)
        {            
            return _asduReceivedHandler(parameter, asdu);
        }
    }
}
