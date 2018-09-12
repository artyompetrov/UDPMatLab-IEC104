using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public class IEC104CommonAddress
    {
        private int _ca;
        private UniqueID _uid;

        public int CA
        {
            get
            {
                if (_uid == null)
                {
                    return _ca;
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
                    _ca = value;
                }
                else
                {
                    _uid.Set(this, value);
                }
            }
        }

        public void SetDestination(IEC104Destination dest)
        {
            _uid = dest.Uid;
            _uid.Set(this, _ca);
        }

        public void DeleteCa()
        {
            _uid.DeleteParameter(this);
        }

        public string Name { get; set; }

        public UniqueID SendUniqueIOA = new UniqueID();
        public UniqueID RecieveUniqueIOA = new UniqueID();

        public BindingList<IEC104SendParameter> SendIOAs { get; set; }  = new BindingList<IEC104SendParameter>();
        public BindingList<IEC104ReceiveParameter> ReceiveIOAs { get; set; } = new BindingList<IEC104ReceiveParameter>();



        public IEC104ReceiveParameter GetRecieveParameterByIOA(int ioa)
        {
            foreach (var recieveIOA in ReceiveIOAs)
            {
                if (recieveIOA.IOA == ioa) return recieveIOA;
            }

            return null;
        }

        public IEC104CommonAddress() { }

        public IEC104CommonAddress(string name)
        {
            Name = name;
        }


        public override string ToString()
        {
            return Name+": "+CA.ToString();
        }
    }
}
