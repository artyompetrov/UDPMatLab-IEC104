using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SimulinkIEC104
{
    public delegate void IEC104ParameterValueChangedHadler(IEC104SendParameter data);


    [XmlInclude(typeof(IEC104SendParameter))]
    [XmlInclude(typeof(IEC104ReceiveParameter))]
    abstract public class IEC104Parameter
    {
        
        private float? _value = null;

        internal UniqueID _uid;
        private int _ioa;
        [XmlIgnore]
        public IEC104CommonAddress Ca;
        public abstract void ClearUDPParameter();

        private void DeleteIOA()
        {
            _uid.DeleteParameter(this);
        }

        public void GetReadyToDelete()
        {
            DeleteIOA();
            ClearUDPParameter();
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
                if (value < 0) throw new WrongDataException("Адрес объекта информации не может быть меньше 0");
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
