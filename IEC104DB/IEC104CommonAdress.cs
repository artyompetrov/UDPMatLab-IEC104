using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEC104DB
{
    public class IEC104CommonAddress
    {
        public int CA { get; set; }

        public List<IEC104SendParameter> SendIOAs = new List<IEC104SendParameter>();
        public List<IEC104ReceiveParameter> ReceiveIOAs = new List<IEC104ReceiveParameter>();

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
    }
}
