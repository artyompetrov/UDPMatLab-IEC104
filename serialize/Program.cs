using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SimulinkIEC104;

namespace Serializator
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings s = new Settings();
            
            var dest = new Destination();
            dest.Name = "Матлаб1";
            dest.IP = "10.221.0.200";
            dest.LocalPort = 15000;
            dest.RemotePort = 15001;
            dest.SendingParameters.Add(new SendingParameter("I3", DataTypeEnum.Double));

            var param1 = new ReceivingParameter("I3", DataTypeEnum.Double);
            dest.ReceivingParameters.Add(param1);

            var param2 = new ReceivingParameter("I3", DataTypeEnum.Int32);
            dest.ReceivingParameters.Add(param2);

            var param3 = new ReceivingParameter("I3", DataTypeEnum.Int32);
            dest.ReceivingParameters.Add(param3);

            var paramSend1 = new SendingParameter("I3", DataTypeEnum.Int32);
            dest.SendingParameters.Add(paramSend1);

            var paramSend2 = new SendingParameter("I3", DataTypeEnum.Int32);
            dest.SendingParameters.Add(paramSend2);
            s.UDPDestinations.Add(dest);

            var dest2 = new Destination();
            dest2.Name = "Матлаб1789789";
            dest2.IP = "10.221.0.200";
            dest2.LocalPort = 15000;
            dest2.RemotePort = 15001;

            dest2.SendingParameters.Add(new SendingParameter("I3", DataTypeEnum.Double));
            s.UDPDestinations.Add(dest2);
            


            var IEC104dest = new IEC104Connection("127.0.0.1", 2405);

            IEC104dest.CommonAdreses.Add(new IEC104CommonAddress());


            var ps1 = new IEC104SendParameter(1);
            ps1.UDPParameter = param1;
            IEC104dest.CommonAdreses[0].SendIOAs.Add(ps1);

            var ps2 = new IEC104SendParameter(2);
            ps2.UDPParameter = param2;
            IEC104dest.CommonAdreses[0].SendIOAs.Add(ps2);

            var ps3 = new IEC104SendParameter(3);
            ps3.UDPParameter = param3;
            IEC104dest.CommonAdreses[0].SendIOAs.Add(ps3);

            var pr = new IEC104ReceiveParameter(2);
            pr.UDPparameters.Add(paramSend1);
            pr.UDPparameters.Add(paramSend2);

            IEC104dest.CommonAdreses[0].ReceiveIOAs.Add(pr);

            s.IEC104Destinations.Add(IEC104dest);

            XmlSerializer formatter = new XmlSerializer(typeof(Settings));

            using (FileStream fs = new FileStream("settings.xml", FileMode.Create))
            {
                formatter.Serialize(fs, s);
            }

            Console.WriteLine("Done, Press any key to close");
            Console.ReadKey();
        }
    }
}
