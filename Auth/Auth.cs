using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace InstaSharp {
    public class Auth {

        InstagramConfig _config;


        

        

        public enum Scope {
            basic,
            comments,
            relationships,
            likes
        }

        public Auth(InstagramConfig config) {
            _config = config;
        }

        public static string AuthLink(string instagramOAuthURI, string clientId, string callbackURI, List<Scope> scopes) {
            StringBuilder scope = new StringBuilder();
            scopes.ForEach(s => {
                if (scope.Length > 0) scope.Append("+");
                scope.Append(s);
            });

            return string.Format("{0}/authorize/?client_id={1}&redirect_uri={2}&response_type=code&scope={3}", new object[] {
                instagramOAuthURI,
                clientId, 
                callbackURI, 
                scope.ToString()
            });
        }

        public AuthInfo RequestToken(string code)
        {
            AuthInfo info = new AuthInfo();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(info.GetType());
            
            
            var parameters = new Dictionary<string, string>();
            parameters.Add("client_id", _config.ClientId);
            parameters.Add("client_secret", _config.ClientSecret);
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("redirect_uri", _config.RedirectURI);
            parameters.Add("code", code);

            var result = HttpClient.POST(_config.OAuthURI + "/access_token", parameters);
           // MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(result))
            //var ret = serializer.ReadObject(ms) as AuthInfo;
            var ret = JsonConvert.DeserializeObject<AuthInfo>(result);
            //.Close();
            return (AuthInfo) ret;
        }
    }
}
