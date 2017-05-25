using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class Groups
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Groups(RestClient client) {
            this.client = client;
        }

        public GetGroupsResponse list(int offset)
        {
            RestRequest request = new RestRequest("/groups?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetGroupsResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Group read(String groupId)
        {
            RestRequest request = new RestRequest("/groups/" + groupId, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetGroupResponse groupResponse = ((JObject)o["data"]).ToObject<GetGroupResponse>();
                return groupResponse.group;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Group create(String groupName, Dictionary<String, Object> attributes)
        {
            RestRequest request = new RestRequest("/groups", Method.POST);
            CreateGroupRequest groupRequest = new CreateGroupRequest();
            groupRequest.group_name = groupName;
            groupRequest.attributes = attributes;
            request.AddJsonBody(groupRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetGroupResponse groupResponse = ((JObject)o["data"]).ToObject<GetGroupResponse>();
                return groupResponse.group;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Group update(String groupId, String groupName, Dictionary<String, Object> attributes)
        {
            RestRequest request = new RestRequest("/groups/"+groupId, Method.PUT);
            CreateGroupRequest groupRequest = new CreateGroupRequest();
            groupRequest.group_name = groupName;
            groupRequest.attributes = attributes;
            request.AddJsonBody(groupRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetGroupResponse groupResponse = ((JObject)o["data"]).ToObject<GetGroupResponse>();
                return groupResponse.group;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String addUserToGroup(String userId, String groupId)
        {
            RestRequest request = new RestRequest("/groups/" + groupId + "/users/" + userId, Method.POST);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public String removeUserFromGroup(String userId, String groupId)
        {
            RestRequest request = new RestRequest("/groups/" + groupId + "/users/" + userId, Method.DELETE);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public String addUserSchemaToGroup(String userSchemaId, String groupId)
        {
            RestRequest request = new RestRequest("/groups/" + groupId + "/user_schemas/" + userSchemaId, Method.POST);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public String removeUserSchemaFromGroup(String userSchemaId, String groupId)
        {
            RestRequest request = new RestRequest("/groups/" + groupId + "/user_schemas/" + userSchemaId, Method.DELETE);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public String delete(string groupId, bool force)
        {
            RestRequest request;
            if (force)
            {
                request = new RestRequest("/groups/" + groupId + "?force=true", Method.DELETE);
            }
            else
            {
                request = new RestRequest("/groups/" + groupId, Method.DELETE);
            }
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }
    }

    public class Group
    {
        [JsonProperty(PropertyName = "insert_date")]
        public DateTime insert_date { get; set; }
        [JsonProperty(PropertyName = "is_active")]
        public Boolean is_active { get; set; }
        [JsonProperty(PropertyName = "last_update")]
        public DateTime last_update { get; set; }
        [JsonProperty(PropertyName = "group_name")]
        public String name { get; set; }
        [JsonProperty(PropertyName = "group_id")]
        public String group_id { get; set; }
        [JsonProperty(PropertyName = "attributes")]
        public Dictionary<String, Object> attributes { get; set; }
    }

    public class GetGroupResponse
    {
        public Group group { get; set; }
    }

    public class GetGroupsResponse
    {
        [JsonProperty(PropertyName = "count")]
        public int count { get; set; }
        [JsonProperty(PropertyName = "total_count")]
        public int total_count { get; set; }
        [JsonProperty(PropertyName = "limit")]
        public int limit { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int offset { get; set; }
        [JsonProperty(PropertyName = "groups")]
        public List<Group> groups { get; set; }
    }

    public class CreateGroupRequest
    {
        [JsonProperty(PropertyName = "group_name")]
        public String group_name { get; set; }
        [JsonProperty(PropertyName = "attributes")]
        public Dictionary<String, Object> attributes { get; set; }
    }
}
