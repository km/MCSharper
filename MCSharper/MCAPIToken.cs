using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharper
{
    class MCAPIToken
    {
        public AuthedEndpoint authedEndpoint;
        
        public MCAPIToken(AuthedEndpoint authEndpoint)
        {
            authedEndpoint = authEndpoint;
        }
    }
}
