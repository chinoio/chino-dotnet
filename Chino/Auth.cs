using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class Auth
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Auth(RestClient client) {
            this.client = client;
        }

        public LoggedUser loginUserWithPassword(string username, string password, string appId, string appSecret)
        {
            var grantType = "password";
            RestRequest request = new RestRequest("/auth/token", Method.POST);
            client.RemoveDefaultParameter("Authorization");
            var tot = appId + ":" + appSecret;
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(tot);
            string encodedText = Convert.ToBase64String(bytesToEncode);
            client.AddDefaultHeader("Authorization", "Basic " + encodedText);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddParameter("grant_type", grantType);
            request.AddParameter("username", username);
            request.AddParameter("password", password);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<LoggedUser>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public User checkUserStatus()
        {
            RestRequest request = new RestRequest("/users/me", Method.GET);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetUserResponse userResponse = ((JObject)o["data"]).ToObject<GetUserResponse>();
                return userResponse.user;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String logoutUser(string token, string appId, string appSecret){
            RestRequest request = new RestRequest("/auth/revoke_token/", Method.POST);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddParameter("token", token);
            request.AddParameter("client_id", appId);
            request.AddParameter("client_secret", appSecret);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return (String)o["result"];
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }
    }

    public class LoggedUser
    {
        [JsonProperty(PropertyName = "access_token")]
        public String access_token { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public String token_type { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int expires_in { get; set; }
        [JsonProperty(PropertyName = "refresh_token")]
        public String refresh_token { get; set; }
        [JsonProperty(PropertyName = "scope")]
        public String scope { get; set; }
    }

    public class LoginRequest
    {
        [JsonProperty(PropertyName = "username")]
        public String username { get; set; }
        [JsonProperty(PropertyName = "password")]
        public String password { get; set; }
        [JsonProperty(PropertyName = "customer_id")]
        public String customer_id { get; set; }
    }

    public class GetLoggedUserResponse
    {
        [JsonProperty(PropertyName = "user")]
        public LoggedUser user { get; set; }
    }

    public class Logout
    {
        [JsonProperty(PropertyName = "username")]
        public String username { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public String user_id { get; set; }
    }

    public class LogoutResponse
    {
        [JsonProperty(PropertyName = "logout")]
        public Logout logout { get; set; }
    }
}
