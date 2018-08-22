using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SimulinkIEC104
{
    public static class ParameterUniqueID
    {
        private static Dictionary<Parameter, int> _ids = new Dictionary<Parameter, int>(); 
        

        public static int Get(Parameter parameter)
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

        internal static bool Set(Parameter parameter, int newId)
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

        internal static void Set(Parameter parameter)
        {
            if (!_ids.ContainsKey(parameter))
            {
                int freeId = 0;
                while (_ids.ContainsValue(freeId)) freeId++;
                _ids.Add(parameter, freeId);
            }
        }

        public static Parameter GetParameterById(int id)
        {
            for (int i = 0; i < _ids.Count; i++)
            {
                if(_ids.ElementAt(i).Value == id)
                {
                    return _ids.ElementAt(i).Key;
                }
            }
            return null;
        }

    }
}