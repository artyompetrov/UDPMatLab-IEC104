using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SimulinkIEC104
{
    public delegate void IEC104ParameterValueChangedHadler(IEC104Parameter data);


    [XmlInclude(typeof(IEC104SendParameter))]
    [XmlInclude(typeof(IEC104ReceiveParameter))]
    abstract public class IEC104Parameter
    {
        
        private float? _value = null;

        internal UniqueID _uid;
        private int _ioa;

        public void DeleteIOA()
        {
            _uid.DeleteParameter(this);
        }
        
        public int IOA
        {
            get
            {
                if (_uid == null)
                {
                    return _ioa;
                }
                else
                {
                    return _uid.Get(this);
                }
            }
            set
            {
                if (_uid == null)
                {
                    _ioa = value;
                }
                else
                {
                    _uid.Set(this, value);
                }
            }
        }

        public abstract void SetCA(IEC104CommonAddress ca);

        

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
