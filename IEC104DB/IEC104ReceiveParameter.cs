using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UDPMatLab;

namespace IEC104DB
{
    public class IEC104ReceiveParameter : IEC104Parameter
    {
        
        [XmlIgnore]
        public List<SendingParameter> UDPparameters = new List<SendingParameter>();

        public string UDPparameterIDs
        {
            get
            {
                string result = String.Empty;
                foreach (SendingParameter param in UDPparameters)
                {
                    result += "," + ParameterUniqueID.Get(param).ToString();
                }
                return result.Substring(1);
            }
            set
            {
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
