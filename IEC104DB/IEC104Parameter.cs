using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IEC104DB
{
    public delegate void IEC104ParameterValueChangedHadler(IEC104Parameter data);


    [XmlInclude(typeof(IEC104SendParameter))]
    [XmlInclude(typeof(IEC104ReceiveParameter))]
    abstract public class IEC104Parameter
    {
        
        private float? _value = null;

        public int IOA { get; set; }

        [XmlIgnore]
        public float Value
        {
            get
            {
                if (_value == null)
                    return 0;
                else
                    return (float)_value;
            }

            set
            {
                if (_value == value) return;
                _value = value;
                _valueChanged();
            }
        }

        internal abstract void _valueChanged();
    }
}
