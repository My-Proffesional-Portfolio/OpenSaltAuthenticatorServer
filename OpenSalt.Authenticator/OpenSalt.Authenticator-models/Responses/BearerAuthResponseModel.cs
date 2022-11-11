using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSalt.Authenticator_models.Responses
{
    public class BearerAuthResponseModel
    {
        public string tokenAudience { get; set; }
        public string privateKey { get; set; }
        public string tokenIssuer { get; set; }
    }
}
