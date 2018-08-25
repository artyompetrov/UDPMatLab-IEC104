using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SimulinkIEC104;

namespace SimulinkIEC104
{
    public class IEC104ReceiveParameter : IEC104Parameter
    {
        
        [XmlIgnore]
        public List<SendingParameter> UDPparameters = new List<SendingParameter>();

        public string UDPparameterIDs
        {
            get
            {
                if (UDPparameters.Count == 0) return string.Empty;

                string result = string.Empty;
                foreach (SendingParameter param in UDPparameters)
                {
                    result += "," + ParameterUniqueID.Get(param).ToString();
                }
                return result.Substring(1);
            }
            set
            {
                UDPparameters.Clear();
                if (value == string.Empty || value == null) return;

                string[] idS = value.Split(',');
                foreach (var id in idS)
                {
                    var param = ParameterUniqueID.GetParameterById(int.Parse(id));
                    if (param!=null)
                    {
                        UDPparameters.Add((SendingParameter)param);
                    }
                }

            }
        }

        public IEC104ReceiveParameter(int ioa)
        {
            IOA = ioa;
        }

        public IEC104ReceiveParameter() { }

        internal override void _valueChanged()
        {
            foreach (var udpParam in UDPparameters)
            {
                udpParam.SetValue(Value);
            }
        }
    }
}
