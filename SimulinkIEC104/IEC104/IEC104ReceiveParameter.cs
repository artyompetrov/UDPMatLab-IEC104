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
    public class IEC104ReceiveParameter : IEC104Parameter, INotifyPropertyChanged
    {
        
        [XmlIgnore]
        public List<SendingParameter> UDPparameters = new List<SendingParameter>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyUDPparameterIDsChanged()
        {
            NotifyPropertyChanged("UDPparameterIDs");
        }

        public override void SetCA(IEC104CommonAddress ca)
        {
            _uid = ca.RecieveUniqueIOA;

        }

        

        public void AddUDPparameter(SendingParameter param)
        {
            var sendingParam = param;

            if (sendingParam.SourceParameter == null)
            {
                sendingParam.SourceParameter = this;
                UDPparameters.Add(sendingParam);
            }
            else
                throw new WrongDataException("Данный параметр уже имеет свой источник");

            NotifyPropertyChanged("UDPparameterIDs");
        }


        public string UDPparameterIDs
        {
            get
            {
                if (UDPparameters.Count == 0) return string.Empty;

                string result = string.Empty;
                foreach (SendingParameter param in UDPparameters)
                {
                    result += "," + ParameterUniqueID.Get(param).ToString();
                }

                
                return result.Substring(1);
                
            }
            set
            {
                UDPparameters.Clear();
                if (value == string.Empty || value == null) return;

                string[] idS = value.Split(',');
                foreach (var id in idS)
                {
                    var param = ParameterUniqueID.GetParameterById(int.Parse(id));
                    if (param!=null)
                    {
                        if (param.GetType() != typeof(SendingParameter)) throw  new WrongDataException("Неправильный ID");

                        AddUDPparameter((SendingParameter)param);


                    }
                }

            }
        }

        public IEC104ReceiveParameter(int ioa)
        {
            IOA = ioa;
        }

        public IEC104ReceiveParameter() { }

        internal override void _valueChanged()
        {
            foreach (var udpParam in UDPparameters)
            {
                udpParam.SetValue(Value);
            }
        }

        public override void ClearUDPParameter()
        {
            foreach (var param in UDPparameters)
            {
                param.SourceParameter = null;
            }
            UDPparameters.Clear();
            NotifyPropertyChanged("UDPparameterIDs");
        }
    }
}
