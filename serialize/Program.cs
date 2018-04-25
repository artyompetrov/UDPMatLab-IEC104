﻿using IEC104DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UDPMatLab;

namespace Serializator
{
    class Program
    {
        static void Main(string[] args)
        {
            Matlab104Program.Settings s = new Matlab104Program.Settings();
            
            var dest = new Destination();
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

            var IEC104dest = new IEC104Connection("127.0.0.1", 2405);

            IEC104dest.CommonAdreses.Add(new IEC104CommonAddress(0));


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

            XmlSerializer formatter = new XmlSerializer(typeof(Matlab104Program.Settings));

            using (FileStream fs = new FileStream("database.xml", FileMode.Create))
            {
                formatter.Serialize(fs, s);
            }

            Console.WriteLine("Done, Press any key to close");
            Console.ReadKey();
        }
    }
}