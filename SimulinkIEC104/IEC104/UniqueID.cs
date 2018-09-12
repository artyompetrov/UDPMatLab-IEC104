using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulinkIEC104
{
    public class UniqueID
    {
        private  Dictionary<object, int> _ids = new Dictionary<object, int>();

        public  void DeleteParameter(object param)
        {
            _ids.Remove(param);
        }

        internal void Set(object parameter)
        {
            if (!_ids.ContainsKey(parameter))
            {
                int freeId = 0;
                while (_ids.ContainsValue(freeId)) freeId++;
                _ids.Add(parameter, freeId);
            }
        }

        public int Get(object parameter)
        {
            if (_ids.ContainsKey(parameter))
            {
                return _ids[parameter];
            }
            else
            {
                int freeId = 0;
                while (_ids.ContainsValue(freeId)) freeId++;

                _ids.Add(parameter, freeId);
                return freeId;
            }
        }

        internal bool Set(object parameter, int newId)
        {

            if (_ids.ContainsValue(newId))
            {
                if (_ids.ContainsKey(parameter) && _ids[parameter] != newId)
                    return false;
                else
                    return true;
            }
            else
            {
                if (_ids.ContainsKey(parameter))
                {
                    _ids[parameter] = newId;
                }
                else
                {
                    _ids.Add(parameter, newId);
                }
                return true;
            }
        }
    }
}
