using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels
{
    public class PozyxException : Exception
    {
        public int RegisterHeader;

        public PozyxException()
        {
        }

        public PozyxException(int registerHeader, string message = "")
            : base(message)
        {
            this.RegisterHeader = registerHeader;
        }

        public PozyxException(string message, Exception inner)
            : base(message, inner)
        {
        }
        
    }
}
