using OpenSalt.Authenticator_models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSalt.Authenticator_infrastructure.Interfaces
{
    public interface IAuthService
    {
        object PasswordEncrypt(string password);
        TokenResponseModel GenerateToken(List<KeyValuePair<string, string>> tokenInfo);
        BearerAuthResponseModel GetBearerAuthInformation(string clientKey);
        object PasswordDecrypt(string hashedPassword, string salt);
    }
}
