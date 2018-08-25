using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public class Settings
    {
        public BindingList<Destination> UDPDestinations { get; set; } = new BindingList<Destination>();

        public BindingList<IEC104Destination> IEC104Destinations { get; set; } = new BindingList<IEC104Destination>();

    }
}
