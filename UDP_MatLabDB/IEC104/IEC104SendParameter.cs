using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SimulinkIEC104;

namespace SimulinkIEC104
{
    public class IEC104SendParameter : IEC104Parameter
    {
        private IEC104ParameterValueChangedHadler _valueChangedHadler;

        public int? UDPParameterID
        {
            get
            {
                if (UDPParameter != null)
                {
                    return UDPParameter.ID;
                }
                else return null;

            }
            set
            {
                if (value != null)
                {
                    Parameter param = ParameterUniqueID.GetParameterById((int)value);
                    if (param != null)
                    {
                        UDPParameter = (ReceivingParameter)param;
                    }
                    else throw new Exception("Нет параметра с таким ID");
                }
            }
        }

        public bool SubscribeOnUDPParameterChanged()
        {

            if (UDPParameter != null)
            {
                UDPParameter.AddValueChangedHandler(_udpParamChanged);
                return true;
            }
            else
                return false;
        }

        private void _udpParamChanged(Parameter data)
        {
            switch(data.DataType)
            {
                case DataTypeEnum.Double:
                    Value = (float)(double)data.Value;
                    break;
                case DataTypeEnum.Int16:
                    Value = (float)(short)data.Value;
                    break;
                case DataTypeEnum.Int32:
                    Value = (float)(int)data.Value;
                    break;
                default: throw new ArgumentException("Данное преобразование не поддерживается");
            }
            
        }

        public void SetValueChangedHandler(IEC104ParameterValueChangedHadler handler)
        {
            _valueChangedHadler = handler;
        }

        internal override void _valueChanged()
        {
            _valueChangedHadler?.Invoke(this);
        }

        [XmlIgnore]
        public ReceivingParameter UDPParameter { get; set; }

        public IEC104SendParameter(int ioa)
        {
            IOA = ioa;
        }

        IEC104SendParameter() { }
    }
}
