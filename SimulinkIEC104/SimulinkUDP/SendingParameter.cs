using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SimulinkIEC104
{
    public class SendingParameter : Parameter
    {
        [XmlIgnore]
        public IEC104ReceiveParameter SourceParameter;

        public bool SetValue(float value)
        {
            //try {
            switch (_dataTypeEnum)
            {
                case DataTypeEnum.Double:
                    Value = (double)value;
                    return true;

                case DataTypeEnum.Int32:
                    Value = (int)Math.Round(value, MidpointRounding.AwayFromZero);
                    return true;

                case DataTypeEnum.Int16:
                    Value = (short)Math.Round(value, MidpointRounding.AwayFromZero);
                    return true;
            }
            //} catch { }
            return false;
        }

        public void ClearSourceParameter()
        {
            if (SourceParameter!=null)
            {
                SourceParameter.UDPparameters.Remove(this);
                SourceParameter.NotifyUDPparameterIDsChanged();
                SourceParameter = null;
            }
        }

        public bool SetValue(double value)
        {
            //try {
            switch (_dataTypeEnum)
            {
                case DataTypeEnum.Double:
                    Value = value;
                    return true;
                case DataTypeEnum.Int32:
                    Value = (int)Math.Round(value, MidpointRounding.AwayFromZero);
                    return true;
                case DataTypeEnum.Int16:
                    Value = (short)Math.Round(value, MidpointRounding.AwayFromZero);
                    return true;
            }
            //} catch { }
            return false;
        }

        public bool SetValue(int value)
        {
            //try {
            switch (_dataTypeEnum)
            {
                case DataTypeEnum.Double:
                    Value = (double)value;
                    return true;

                case DataTypeEnum.Int32:
                    Value = value;
                    return true;

                case DataTypeEnum.Int16:
                    Value = (short)value;
                    return true;
            }
            //} catch { }

            return false;
        }

        public byte[] GetBytes()
        {
            byte[] result = new byte[0];
            switch (_dataTypeEnum)
            {
                case DataTypeEnum.Int16:
                    if (Value != null)
                        result = BitConverter.GetBytes((short)Value);
                    else
                        result = BitConverter.GetBytes((short)0);
                    break;
                case DataTypeEnum.Int32:
                    if (Value != null)
                        result = BitConverter.GetBytes((int)Value);
                    else
                        result = BitConverter.GetBytes(0);
                    break;
                case DataTypeEnum.Double:
                    if (Value != null)
                        result = BitConverter.GetBytes((double)Value);
                    else
                        result = BitConverter.GetBytes(0.0);
                    break;
                default:
                    throw new InvalidOperationException("Данный DataTypeEnum не обрабатывается");
            }
            return result;
        }

        public SendingParameter(string oiName, DataTypeEnum type) : base (oiName, type)
        {

        }

        public SendingParameter() : base () { }

        internal void AddValueChangedHandler(ValueChangedHadler handler)
        {
            _valueChangedHadler += handler;
        }

        
    }

}
