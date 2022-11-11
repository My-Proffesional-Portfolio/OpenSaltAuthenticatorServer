using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSalt.Authenticator_infrastructure.Interfaces
{
    public interface IUtilService
    {
        string GetAppSettingsConfiguration(string section, string property);
        List<KeyValuePair<string, string>> GetAppSettingsConfiguration(string section);


    }
}
