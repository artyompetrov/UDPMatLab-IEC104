using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public class Settings
    {
        public List<Destination> UDPDestinations = new List<Destination>();
        public List<IEC104Destination> IEC104Destinations = new List<IEC104Destination>();
    }
}
