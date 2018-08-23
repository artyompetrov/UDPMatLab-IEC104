using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public class Settings
    {
        public List<Destination> UDPDestinations { get; set; } = new List<Destination>();

        public List<IEC104Destination> IEC104Destinations { get; set; } = new List<IEC104Destination>();

    }
}
