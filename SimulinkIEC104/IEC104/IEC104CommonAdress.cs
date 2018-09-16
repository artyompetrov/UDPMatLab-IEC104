using System.ComponentModel;
using System.Windows.Forms;

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
                if (value < 0) throw new WrongDataException("Общий адрес не может быть меньше 0");
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
            _uid = dest.CAUid;
            _uid.Set(this, _ca);
        }

        public void GetReadyToDelete()
        {
            _uid.DeleteParameter(this);
            foreach (var send in SendIOAs)
            {
                send.GetReadyToDelete();

            }
            foreach (var recieve in ReceiveIOAs)
            {

                recieve.GetReadyToDelete();

            }
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
