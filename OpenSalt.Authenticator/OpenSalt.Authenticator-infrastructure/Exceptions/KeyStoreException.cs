using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSalt.Authenticator_infrastructure.Exceptions
{
    public class KeyStoreException : Exception
    {
        public KeyStoreException(string message) : base(message)
        {

        }
    }
}
