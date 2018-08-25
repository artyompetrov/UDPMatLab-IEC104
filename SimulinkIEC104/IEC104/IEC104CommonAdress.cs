using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public class IEC104CommonAddress
    {
        public int CA { get; set; }
        public string Name { get; set; }

        public List<IEC104SendParameter> SendIOAs { get; set; }  = new List<IEC104SendParameter>();
        public List<IEC104ReceiveParameter> ReceiveIOAs { get; set; } = new List<IEC104ReceiveParameter>();

        public IEC104CommonAddress(int ca)
        {
            CA = ca;
        }

        public IEC104ReceiveParameter GetRecieveParameterByIOA(int ioa)
        {
            foreach (var recieveIOA in ReceiveIOAs)
            {
                if (recieveIOA.IOA == ioa) return recieveIOA;
            }

            return null;
        }

        public IEC104CommonAddress() { }
        public IEC104CommonAddress(string name, int ca)
        {
            Name = name;
            CA = ca;
        }
        public IEC104CommonAddress(string name)
        {
            Name = name;
        }


        public override string ToString()
        {
            return Name+": "+CA.ToString();
        }
    }
}
