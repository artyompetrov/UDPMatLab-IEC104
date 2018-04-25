using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace UDPMatLab
{
    public enum DataTypeEnum
    {
        Double, Int32, Int16
    }

    public delegate void ValueChangedHadler(Parameter data);

    public abstract class Parameter
    {

        internal ValueChangedHadler _valueChangedHadler = null;
        private object _value = null;
        internal DataTypeEnum _dataTypeEnum;
        public string OiName { get; set; }

        [XmlAttribute]
        public int ID
        {
            get
            {
                return ParameterUniqueID.Get(this);
            }
            set
            {
                ParameterUniqueID.Set(this, value);
            }

        }
        
        [XmlIgnore]
        public object Value
        {
            get
            {
                return _value;
            }
            protected set
            {
                if (_value == null || !_value.Equals(value))
                {
                    _value = value;
                    _valueChangedHadler?.Invoke(this);
                }
            }
        }

        

        public DataTypeEnum DataType
        {
            get
            {
                return _dataTypeEnum;
                
            }
            set
            {
                switch (value)
                {
                    case DataTypeEnum.Double:
                        Type = typeof(double);
                        Bits = 8;
                        break;
                    case DataTypeEnum.Int32:
                        Type = typeof(int);
                        Bits = 4;
                        break;
                    case DataTypeEnum.Int16:
                        Type = typeof(short);
                        Bits = 2;
                        break;
                    default:
                        throw new InvalidOperationException("Данный DataTypeEnum не обрабатывается");
                }

                _dataTypeEnum = value;
            }
        }

        [XmlIgnore]
        public Type Type { get; private set; }
        [XmlIgnore]
        public int Bits { get; private set; }

        internal Parameter() { }


        internal Parameter(string oiName, DataTypeEnum type)
        {
            OiName = oiName;
            DataType = type;
            ParameterUniqueID.Set(this);
        }
        
    }
    
}