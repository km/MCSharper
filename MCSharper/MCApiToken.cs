using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharper
{
    class MCApiToken
    {
        public AuthedEndpoint AE;
        public MCApiToken(AuthedEndpoint ae)
        {
            AE = ae;
        }
    }
}
