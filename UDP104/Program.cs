using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using System.Xml.Serialization;
using SimulinkIEC104;
using lib60870.CS104;
using lib60870.CS101;
using System.ComponentModel;

namespace Matlab104Program
{
    class Program
    {
        static System.Timers.Timer timer;
        static System.Timers.Timer timer2;

        private static string _configFileName = "database.xml";
        private static byte _debugLevel = 7;
        private static BindingList<Destination> _destinations;
        private static Settings _settings;
        static Connection con;
        static ApplicationLayerParameters alp = new ApplicationLayerParameters();
        static void Main(string[] args)
        {

            _debugMessage("XML-файл конфигурации: " + _configFileName, 1);
            _debugMessage("Уровень отладки: " + _debugLevel, 1);

            
            try
            {
                XmlSerializer formatter = new XmlSerializer(typeof(Settings));
                using (FileStream fs = new FileStream(_configFileName, FileMode.Open))
                {
                    _settings = (Settings)formatter.Deserialize(fs);
                }

                _debugMessage("Десериализация выполнена успешно", 2);
            }
            catch (Exception ex)
            {
                _debugMessage("Ошибка десериализации: " + ex.Message, 1);
                _endProgram();
            }

            _destinations = _settings.UDPDestinations;


            for (int i = 0; i < _destinations.Count; i++)
            {
                _destinations[i].AddDebugMessageHandler(_debugMessage);
                /*foreach (var data in _destinations[i].ReceivingParameters)
                {
                    data.AddValueChangedHandler(_dataReceived);
                }*/
                _destinations[i].UdpClientStart();    
            }
            
            foreach (var dest in _settings.IEC104Destinations)
            { 
                foreach (var paramGroup in dest.CommonAdreses)
                {
                    foreach (IEC104SendParameter sp in paramGroup.SendIOAs)
                    {
                        sp.SubscribeOnUDPParameterChanged();
                    }
                }
            }

            foreach (var dest in _settings.IEC104Destinations)
            {
                if (dest.GetType() == typeof(IEC104Connection))
                {
                    ((IEC104Connection)dest).Connect();


                }
            }

            /*
            timer = new System.Timers.Timer(3000);
            timer.Elapsed += _changeDataOnTimer;
            timer.Start();

            timer2 = new System.Timers.Timer(3000);
            timer2.Elapsed += _changeDataOnTimer2;
            timer2.Start();
            */



            Console.ReadKey();
        }

        private static bool _asduReceived(object parameter, ASDU asdu)
        {
            _debugMessage("asdu", 6);
            return true;
        }

        private static void _changeDataOnTimer(object sender, ElapsedEventArgs e)
        {
            _destinations[0].SendingParameters[0].SetValue(4.3f);
            _destinations[0].SendingParameters[1].SetValue(3);

            
            ASDU asdu = new ASDU(alp, TypeID.M_ME_NC_1, CauseOfTransmission.SPONTANEOUS, false, false, 2, 0, false);
            asdu.AddInformationObject(new MeasuredValueShort(1, -1, new QualityDescriptor()));


            con.SendASDU(asdu);
        }

        private static void _debugMessage(Destination destination, string message, byte debugLevel)
        {
            _debugMessage(destination.LocalPort + " -> " + destination.IP + ":" + destination.RemotePort + ": " + message, debugLevel);
        }

        private static void _changeDataOnTimer2(object sender, ElapsedEventArgs e)
        {
            _destinations[0].SendingParameters[0].SetValue(15.5);
            _destinations[0].SendingParameters[1].SetValue(5);

            ASDU asdu = new ASDU(alp, TypeID.M_ME_NC_1, CauseOfTransmission.SPONTANEOUS, false, false, 2, 0, false);
            asdu.AddInformationObject(new MeasuredValueShort(1, 1, new QualityDescriptor()));


            con.SendASDU(asdu);
        }

        private static void _dataReceived(Parameter data)
        {
            _debugMessage("Изменилось значение параметра: " + data.ID + ". Новое значение:" + data.Value, 3);
        }


        private static void _debugMessage(string message, byte level)
        {
            if (_debugLevel > 0 && _debugLevel >= level)
            {
                Console.WriteLine(message);
            }
        }

        private static void _endProgram(int code = 0)
        {
            try
            {
                

                for (int i = 0; i < _destinations.Count; i++)
                {
                    _destinations[i].UdpClientClose();
                    
                }


            } catch (Exception ex)
            {
                _debugMessage("Ошибка завершении программы: " + ex.Message, 2);
            }
            //Environment.Exit(code);
        }
    }
}
