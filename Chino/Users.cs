using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class Users
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Users(RestClient client) {
            this.client = client;
        }

        public GetUsersResponse list(String userSchemaId, int offset)
        {
            RestRequest request = new RestRequest("/user_schemas/" + userSchemaId + "/users?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetUsersResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public User read(String userId)
        {
            RestRequest request = new RestRequest("/users/" + userId, Method.GET);
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


        public User create(String username, String password, Dictionary<String, Object> attributes, String userSchemaId)
        {
            RestRequest request = new RestRequest("/user_schemas/" + userSchemaId + "/users", Method.POST);
            CreateUserRequest userRequest = new CreateUserRequest();
            userRequest.username = username;
            userRequest.password = password;
            userRequest.attributes = attributes;
            request.AddJsonBody(userRequest);
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

        public User update(String username, String password, Dictionary<String, Object> attributes, String userId)
        {
            RestRequest request = new RestRequest("/users/" + userId, Method.PUT);
            CreateUserRequest userRequest = new CreateUserRequest();
            userRequest.username = username;
            userRequest.password = password;
            userRequest.attributes = attributes;
            request.AddJsonBody(userRequest);
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

        public User updateSomeFields(String userId, Dictionary<String, Object> attributes)
        {
            RestRequest request = new RestRequest("/users/" + userId, Method.PATCH);
            PatchUserRequest userRequest = new PatchUserRequest();
            userRequest.attributes = attributes;
            request.AddJsonBody(userRequest);
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

        public String delete(string userId, bool force)
        {
            RestRequest request;
            if (force)
            {
                request = new RestRequest("/users/" + userId + "?force=true", Method.DELETE);
            }
            else
            {
                request = new RestRequest("/users/" + userId, Method.DELETE);
            }
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

    }

    //Below there are the classes used to get and upload data
    public class User
    {
        [JsonProperty(PropertyName = "username")]
        public String username { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public String user_id { get; set; }
        [JsonProperty(PropertyName = "insert_date")]
        public DateTime insert_date { get; set; }
        [JsonProperty(PropertyName = "groups")]
        public List<String> groups { get; set; }
        [JsonProperty(PropertyName = "is_active")]
        public Boolean is_active { get; set; }
        [JsonProperty(PropertyName = "last_update")]
        public DateTime last_update { get; set; }
        [JsonProperty(PropertyName = "attributes")]
        public Dictionary<String, Object> attributes { get; set; }
      
    }

    public class GetUserResponse
    {
        [JsonProperty(PropertyName = "user")]
        public User user { get; set; }
    }

    public class GetUsersResponse
    {
        [JsonProperty(PropertyName = "count")]
        public int count { get; set; } 
        [JsonProperty(PropertyName = "total_count")]
        public int total_count { get; set; }
        [JsonProperty(PropertyName = "limit")]
        public int limit { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int offset { get; set; }
        [JsonProperty(PropertyName = "users")]
        public List<User> users { get; set; }
    }

    public class CreateUserRequest
    {
        [JsonProperty(PropertyName = "username")]
        public String username { get; set; }
        [JsonProperty(PropertyName = "password")]
        public String password { get; set; }
        [JsonProperty(PropertyName = "attributes")]
        public Dictionary<String, Object> attributes { get; set; }
    }

    public class PatchUserRequest
    {
        [JsonProperty(PropertyName = "attributes")]
        public Dictionary<String, Object> attributes { get; set; }
    }
}
