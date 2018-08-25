using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public class IEC104Server : IEC104Destination
    {
        private int _port = 2404;
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

        public IEC104Server(string name) : base(name) { }

        public IEC104Server() : base() { }

        internal override void Send(IEC104Parameter data)
        {
            throw new NotImplementedException();
        }
    }
}
