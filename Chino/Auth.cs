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

        public LoggedUser loginUser(String username, String password, String customerId)
        {
            RestRequest request = new RestRequest("/auth/login", Method.POST);
            LoginRequest loginRequest = new LoginRequest();
            loginRequest.username = username;
            loginRequest.password = password;
            loginRequest.customer_id = customerId;
            request.AddJsonBody(loginRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetLoggedUserResponse loggedUserResponse = ((JObject)o["data"]).ToObject<GetLoggedUserResponse>();
                return loggedUserResponse.user;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public User checkUserStatus()
        {
            RestRequest request = new RestRequest("/auth/info", Method.GET);
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

        public LogoutResponse logoutUser(){
            RestRequest request = new RestRequest("/auth/logout", Method.POST);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<LogoutResponse>();
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
        [JsonProperty(PropertyName = "username")]
        public String username { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int expires_in { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public String user_id { get; set; }
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
