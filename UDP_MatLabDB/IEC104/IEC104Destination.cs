using lib60870.CS101;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;

namespace SimulinkIEC104
{
    [XmlInclude(typeof(IEC104Server))]
    [XmlInclude(typeof(IEC104Connection))]
    public abstract class IEC104Destination
    {
        internal ApplicationLayerParameters _alp = new ApplicationLayerParameters();
        public string Name { get; set; }

        public List<IEC104CommonAddress> CommonAdreses = new List<IEC104CommonAddress>();

        public void SubscribeOnSendingParametersChange()
        {
            foreach (IEC104CommonAddress ca in CommonAdreses)
            {
                foreach (IEC104SendParameter sp in ca.SendIOAs)
                {
                    sp.SetValueChangedHandler(Send);
                } 
                
            }
        }

        private IEC104CommonAddress GetCommonAdressByCA(int ca)
        {
            foreach (var commAdr in CommonAdreses)
            {
                if (commAdr.CA == ca) return commAdr;
            }

            return null;
        }

        internal bool _asduReceivedHandler(object parameter, ASDU asdu)
        {
            IEC104CommonAddress commAdr = GetCommonAdressByCA(asdu.Ca);
            if (commAdr == null)
            {
                Console.WriteLine("debug полчено сообщение с неизвестным CA");
                return false;
            }

            for (int i = 0; i< asdu.NumberOfElements; i++)
            {
                InformationObject io = asdu.GetElement(i);

                IEC104ReceiveParameter recievePar = commAdr.GetRecieveParameterByIOA(io.ObjectAddress);
                if (recievePar == null)
                {
                    Console.WriteLine("debug полчено сообщение с неизвестным IOA");
                    continue;
                }
                else
                {

                    switch (io.Type)
                    {
                        case TypeID.M_ME_NC_1:
                        case TypeID.M_ME_TC_1:
                        case TypeID.M_ME_TF_1:
                            Console.WriteLine("получено значение параметра " + ((MeasuredValueShort)io).Value);
                            recievePar.Value = ((MeasuredValueShort)io).Value;
                            break;
                        default:
                            Console.WriteLine("debug полчено сообщение с неизвестным типом");
                            break;
                    }
                }

            }

            return true;

        }

        internal abstract void Send(IEC104Parameter data);


    }

   

}
