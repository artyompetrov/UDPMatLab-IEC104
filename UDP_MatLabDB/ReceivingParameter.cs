using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UDPMatLab
{
    public class ReceivingParameter : Parameter
    {
        [XmlIgnore]
        List<ILinkedParameter> LinkedParameters = new List<ILinkedParameter>();

        public bool SetValueFromBytes(byte[] bytes, int startIndex)
        {
                switch (_dataTypeEnum)
                {
                    case DataTypeEnum.Int16:
                        Value = BitConverter.ToInt16(bytes, startIndex);
                        break;
                    case DataTypeEnum.Int32:
                        Value = BitConverter.ToInt32(bytes, startIndex);
                        break;
                    case DataTypeEnum.Double:
                        Value = BitConverter.ToDouble(bytes, startIndex);
                        break;
                    default:
                        throw new InvalidOperationException("Данный DataTypeEnum не обрабатывается");
                }
 

            return true;
        }

        public ReceivingParameter() { }

        public ReceivingParameter(string oiName, DataTypeEnum type) : base (oiName, type)
        {
            
        }

        public void AddValueChangedHandler(ValueChangedHadler handler)
        {
            _valueChangedHadler += handler;
        }
    }
}
