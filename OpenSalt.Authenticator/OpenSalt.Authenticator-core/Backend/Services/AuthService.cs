using Microsoft.IdentityModel.Tokens;
using OpenSalt.Authenticator_core.Backend.Utils;
using OpenSalt.Authenticator_infrastructure.Exceptions;
using OpenSalt.Authenticator_infrastructure.Interfaces;
using OpenSalt.Authenticator_models.Responses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenSalt.Authenticator_core.Backend.Services
{
    public class AuthService : IAuthService
    {
        private string jwtKey;
        private string secretPasswordKey;
        private readonly IUtilService _utilService;

        public AuthService(IUtilService utilService)
        {
            _utilService = utilService;
            jwtKey = _utilService.GetAppSettingsConfiguration("security", "JWT_PrivateKey");
            secretPasswordKey = _utilService.GetAppSettingsConfiguration("security", "passwordPrivateKey");
        }

        /*For mock testing*/
        public AuthService()
        {
            _utilService = new Utils.UtilService();
            jwtKey = _utilService.GetAppSettingsConfiguration("security", "JWT_PrivateKey");
            secretPasswordKey = _utilService.GetAppSettingsConfiguration("security", "passwordPrivateKey");
        }

        public object PasswordEncrypt(string password)
        {

            //string secretKey = UtilService.GetAppSettingsConfiguration("security", "passwordPrivateKey");
            string secretKey = "ABC123";
            var saltValue = Guid.NewGuid().ToString();
            var saltBuffer = Encoding.UTF8.GetBytes(saltValue);
            byte[] clearBytes = Encoding.Unicode.GetBytes(password);

            string passwordHash = String.Empty;

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(secretKey, saltBuffer, 1000, HashAlgorithmName.SHA256);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    passwordHash = Convert.ToBase64String(ms.ToArray());
                }
            }

            return new { passwordHash = passwordHash, saltValue = saltValue };

        }

        /*[
           {
          "key": "userid",
           "value": "29"
         },
         {
          "key": "clientKey",
          "value": "AB6B-B4C8BFEAD9B2"
          },
          {
           "key": "roles",
                  "value": "12"
                  },
                  {
           "key": "roles",
                  "value": "120"
                  }
                  ,
                  {
           "key": "roles",
                  "value": "1255"
                  }
            ]
        */

        public TokenResponseModel GenerateToken(List<KeyValuePair<string, string>> tokenInfo)
        {
            var permClaims = new List<Claim>();

            foreach (var item in tokenInfo)
            {
                if (item.Key == "clientKey")
                    continue;
                else
                    permClaims.Add(new Claim(item.Key, item.Value));
            }
            //permClaims.Add(new Claim("companyId", userInfo.CompanyId.ToString()));
            //permClaims.Add(new Claim("email", userInfo.Email));


            var tokenAudience = "";

            if (tokenInfo.Where(f => f.Key == "clientKey").Count() == 0)
                throw new KeyStoreException("Key was not found in body, please provide a client key");
            else
            {
                var selectedKey = tokenInfo.First(f => f.Key == "clientKey").Value;
                tokenAudience = _utilService.GetAppSettingsConfiguration("clientKeys").Where(w => w.Value == selectedKey).FirstOrDefault().Key;

                if (string.IsNullOrWhiteSpace(tokenAudience))
                    throw new KeyStoreException("Specified key does not exist in keystore");

            }

            //if (permClaims.FirstOrDefault(f => f.Type == "clientKey") == null)
            //    throw new KeyStoreException("Key was not found in body");
            //else
            //{
            //    var selectedKey = permClaims.First(f => f.Type == "clientKey").Value;
            //    tokenAudience = UtilService.GetAppSettingsConfiguration("clientKeys").Where(w => w.Value == selectedKey).FirstOrDefault().Key;

            //    if (string.IsNullOrWhiteSpace(tokenAudience))
            //        throw new KeyStoreException("Specified key does not exist in keystore");

            //}

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddDays(365);

            var token = new JwtSecurityToken("e-colonies-auth",
              tokenAudience, claims: permClaims,
              null,
              expires: expiration,
              signingCredentials: credentials);



            //TODO: get time from token object
            //expiration = !generateSpecialToken ? DateTime.Now.AddMinutes(60) : DateTime.Now.AddSeconds(30);

            return new TokenResponseModel { token = new JwtSecurityTokenHandler().WriteToken(token) };

        }

        public BearerAuthResponseModel GetBearerAuthInformation(string clientKey)
        {
            var keys = _utilService.GetAppSettingsConfiguration("clientKeys");

            var selectedClientKey = keys.Where(w => w.Value == clientKey);

            if (selectedClientKey.Count() == 0)
                throw new KeyStoreException("Provided key does not match with keystore");

            return new BearerAuthResponseModel
            {
                tokenAudience = selectedClientKey.FirstOrDefault().Key,
                privateKey = jwtKey,
                tokenIssuer = "e-auth-provider"

            };
        }

        public object PasswordDecrypt(string hashedPassword, string salt)
        {
            try
            {
                //string secretKey = UtilService.GetAppSettingsConfiguration("security", "passwordPrivateKey");
                string secretKey = "ABC123";
                var saltBuffer = Encoding.UTF8.GetBytes(salt);
                hashedPassword = hashedPassword.Replace(" ", "+");
                string password = string.Empty;
                byte[] cipherBytes = Convert.FromBase64String(hashedPassword);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(secretKey, saltBuffer, 1000, HashAlgorithmName.SHA256);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        password = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return new { plainPassword = password, IsError = false, ErrorMessage = "", TechnicalMessage = "" };
            }
            catch (Exception ex)
            {
                return new { plainPassword = "", IsError = true, ErrorMessage = "Cannot decrypt given password", TechnicalMessage = ex.Message };
            }
        }

        //how to validate token!!!
        //https://stackoverflow.com/questions/50204844/how-to-validate-a-jwt-token

    }
}
