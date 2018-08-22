using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public class IEC104Server
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
    }
}
