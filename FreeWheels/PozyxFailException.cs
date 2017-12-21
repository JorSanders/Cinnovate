using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels
{
    public class PozyxFailException : Exception
    {
        public int RegisterHeader;

        public PozyxFailException(int registerHeader)
            : base()
        {
            this.RegisterHeader = registerHeader;
        }

        public PozyxFailException(int registerHeader, string message = "")
            : base(message)
        {
            this.RegisterHeader = registerHeader;
        }

        public PozyxFailException(int registerHeader, string message, Exception inner)
            : base(message, inner)
        {
            this.RegisterHeader = registerHeader;
        }

        
    }
}
