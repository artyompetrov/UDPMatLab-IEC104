using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulinkIEC104
{
    class WrongDataException : Exception
    {
        public WrongDataException() { }

        public WrongDataException(string message) : base(message) { }

    }
}
