using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using SimulinkIEC104;

namespace SimulinkIEC104
{
    public class IEC104SendParameter : IEC104Parameter, INotifyPropertyChanged
    {
        private IEC104ParameterValueChangedHadler _valueChangedHadler;

        public event PropertyChangedEventHandler PropertyChanged;
        

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override void SetCA(IEC104CommonAddress ca)
        {
            Ca = ca;
            _uid = ca.SendUniqueIOA;

        }

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
                        UDPParameter.LinkedParameters.Add(this);
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

        public void SetValueChangedHandler(IEC104ParameterValueChangedHadler handler, IEC104CommonAddress ca)
        {
            _valueChangedHadler = handler;
            Ca = ca;
        }

        internal override void _valueChanged()
        {
            _valueChangedHadler?.Invoke(this);
        }

        [XmlIgnore]
        public ReceivingParameter UDPParameter { get; set; }

        public void SetUDPParameter(ReceivingParameter receivingParameter)
        {
            
            UDPParameter = receivingParameter;
            UDPParameter.LinkedParameters.Add(this);
            NotifyPropertyChanged("UDPParameterID");
        }

        public override void ClearUDPParameter()
        {
            if (UDPParameter != null)
            {
                UDPParameter.LinkedParameters.Remove(this);
                UDPParameter = null;
            }

            NotifyPropertyChanged("UDPParameterID");
        }

        public IEC104SendParameter(int ioa)
        {
            IOA = ioa;
        }

        public IEC104SendParameter() { }
    }
}
