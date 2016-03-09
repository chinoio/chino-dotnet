using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class UserSchemas
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public UserSchemas(RestClient client) {
            this.client = client;
        }

        public GetUserSchemasResponse list(int offset)
        {
            RestRequest request = new RestRequest("/user_schemas?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetUserSchemasResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public UserSchema read(String userSchemaId)
        {
            RestRequest request = new RestRequest("/user_schemas/" + userSchemaId, Method.GET);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetUserSchemaResponse userSchemaResponse = ((JObject)o["data"]).ToObject<GetUserSchemaResponse>();
                return userSchemaResponse.user_schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }

        }

        public UserSchema create(UserSchemaRequest userSchemaRequest)
        {
            RestRequest request = new RestRequest("/user_schemas", Method.POST);
            request.AddJsonBody(userSchemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetUserSchemaResponse userSchemaResponse = ((JObject)o["data"]).ToObject<GetUserSchemaResponse>();
                return userSchemaResponse.user_schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public UserSchema create(String description, UserSchemaStructure userSchemaStructure)
        {
            RestRequest request = new RestRequest("/user_schemas", Method.POST);
            UserSchemaRequest userSchemaRequest = new UserSchemaRequest();
            userSchemaRequest.description = description;
            userSchemaRequest.structure = userSchemaStructure;
            request.AddJsonBody(userSchemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetUserSchemaResponse userSchemaResponse = ((JObject)o["data"]).ToObject<GetUserSchemaResponse>();
                return userSchemaResponse.user_schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public UserSchema create(String description, Type myClass)
        {
            RestRequest request = new RestRequest("/user_schemas", Method.POST);
            UserSchemaRequest userSchemaRequest = new UserSchemaRequest();
            userSchemaRequest.description = description;
            List<UserSchemaField> fields = new List<UserSchemaField>();
            UserSchemaStructure userSchemaStructure = new UserSchemaStructure();
            foreach (System.Reflection.FieldInfo property in myClass.GetFields())
            {
                UserSchemaField f = new UserSchemaField(property.Name, Utils.checkType(property.FieldType));
                fields.Add(f);
            }
            userSchemaStructure.fields = fields;
            myClass.GetProperties();
            userSchemaRequest.structure = userSchemaStructure;
            request.AddJsonBody(userSchemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetUserSchemaResponse userSchemaResponse = ((JObject)o["data"]).ToObject<GetUserSchemaResponse>();
                return userSchemaResponse.user_schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public UserSchema update(String userSchemaId, UserSchemaRequest userSchemaRequest)
        {
            RestRequest request = new RestRequest("/user_schemas/"+userSchemaId, Method.PUT);
            request.AddJsonBody(userSchemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetUserSchemaResponse userSchemaResponse = ((JObject)o["data"]).ToObject<GetUserSchemaResponse>();
                return userSchemaResponse.user_schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public UserSchema update(String userSchemaId, String description, UserSchemaStructure userSchemaStructure)
        {
            RestRequest request = new RestRequest("/user_schemas/"+userSchemaId, Method.PUT);
            UserSchemaRequest userSchemaRequest = new UserSchemaRequest();
            userSchemaRequest.description = description;
            userSchemaRequest.structure = userSchemaStructure;
            request.AddJsonBody(userSchemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetUserSchemaResponse userSchemaResponse = ((JObject)o["data"]).ToObject<GetUserSchemaResponse>();
                return userSchemaResponse.user_schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public UserSchema update(String userSchemaId, String description, Type myClass)
        {
            RestRequest request = new RestRequest("/user_schemas/"+userSchemaId, Method.PUT);
            UserSchemaRequest userSchemaRequest = new UserSchemaRequest();
            userSchemaRequest.description = description;
            List<UserSchemaField> fields = new List<UserSchemaField>();
            UserSchemaStructure userSchemaStructure = new UserSchemaStructure();
            foreach (System.Reflection.FieldInfo property in myClass.GetFields())
            {
                UserSchemaField f = new UserSchemaField(property.Name, Utils.checkType(property.FieldType));
                fields.Add(f);
            }
            userSchemaStructure.fields = fields;
            myClass.GetProperties();
            userSchemaRequest.structure = userSchemaStructure;
            request.AddJsonBody(userSchemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetUserSchemaResponse userSchemaResponse = ((JObject)o["data"]).ToObject<GetUserSchemaResponse>();
                return userSchemaResponse.user_schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String delete(string userSchemaId, bool force)
        {
            RestRequest request;
            if (force)
            {
                request = new RestRequest("/user_schemas/" + userSchemaId + "?force=true", Method.DELETE);
            }
            else
            {
                request = new RestRequest("/user_schemas/" + userSchemaId, Method.DELETE);
            }
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

    }

    //Below there are the classes used to get and upload data
    public class UserSchema
    {
        [JsonProperty(PropertyName = "user_schema_id")]
        public String user_schema_id { get; set; }
        [JsonProperty(PropertyName = "description")]
        public String description { get; set; }
        [JsonProperty(PropertyName = "is_active")]
        public Boolean is_active { get; set; }
        [JsonProperty(PropertyName = "last_update")]
        public DateTime last_update { get; set; }
        [JsonProperty(PropertyName = "insert_date")]
        public DateTime insert_date { get; set; }
        [JsonProperty(PropertyName = "structure")]
        public UserSchemaStructure structure { get; set; }
    }

    public class GetUserSchemaResponse
    {
        [JsonProperty(PropertyName = "result")]
        public String result { get; set; }
        [JsonProperty(PropertyName = "user_schema")]
        public UserSchema user_schema { get; set; }
    }

    public class GetUserSchemasResponse
    {
        [JsonProperty(PropertyName = "count")]
        public int count { get; set; }
        [JsonProperty(PropertyName = "total_count")]
        public int total_count { get; set; }
        [JsonProperty(PropertyName = "limit")]
        public int limit { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int offset { get; set; }
        [JsonProperty(PropertyName = "user_schemas")]
        public List<UserSchema> user_schemas { get; set; }
    }

    public class UserSchemaRequest
    {
        [JsonProperty(PropertyName = "description")]
        public String description { get; set; }
        [JsonProperty(PropertyName = "structure")]
        public UserSchemaStructure structure { get; set; }
    }

    public class UserSchemaStructure
    {
        [JsonProperty(PropertyName = "fields")]
        public List<UserSchemaField> fields { get; set; }
    }

    public class UserSchemaField
    {
        [JsonProperty(PropertyName = "name")]
        public String name { get; set; }
        [JsonProperty(PropertyName = "type")]
        public String type { get; set; }

        public UserSchemaField() { }

        public UserSchemaField(String name, String type)
        {
            this.name = name;
            this.type = type;
        }
    }
}
