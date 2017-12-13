using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.Interfaces
{
    public interface IConnection
    {
        bool connected { get; }

        byte[] ReadWrite(byte[] request, int length);

        void Write(byte[] request);

        Task Connect ();
    }
}
