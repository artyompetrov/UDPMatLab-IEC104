using lib60870.CS101;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace SimulinkIEC104
{
    [XmlInclude(typeof(IEC104Server))]
    [XmlInclude(typeof(IEC104Connection))]
    public abstract class IEC104Destination 
    {
        internal ApplicationLayerParameters _alp = new ApplicationLayerParameters();
        private string _name;
        private int _port = 2404;
        [XmlIgnore]
        public UniqueID CAUid = new UniqueID();
        abstract public void Initialize();

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
                else throw new WrongDataException("Порт задан неверно");
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
                
            }
         
        } 

        public IEC104Destination() { }
        public IEC104Destination(string name)
        {
            Name = name;
        }

        public BindingList<IEC104CommonAddress> CommonAdreses { get; set; } = new BindingList<IEC104CommonAddress>();

        public void SubscribeOnSendingParametersChange()
        {
            foreach (IEC104CommonAddress ca in CommonAdreses)
            {
                foreach (IEC104SendParameter sp in ca.SendIOAs)
                {
                    sp.SetValueChangedHandler(Send, ca);
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
                            Console.WriteLine("debug полчено сообщение с неизвестным типом "+ io.Type.ToString());
                            break;
                    }
                }

            }

            return true;

        }

        internal abstract void Send(IEC104SendParameter data);

        public override string ToString()
        {
            return Name;
        }
    }

   

}
