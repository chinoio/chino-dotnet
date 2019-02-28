using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class Permissions
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Permissions(RestClient client) {
            this.client = client;
        }

        public GetPermissionsResponse readPermissions(int offset)
        {
            RestRequest request = new RestRequest("/perms?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetPermissionsResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public GetPermissionsResponse readPermissionsOnaDocument(String documentId, int offset)
        {
            RestRequest request = new RestRequest("/perms/documents/"+documentId+"?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetPermissionsResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public GetPermissionsResponse readPermissionsOfaUser(String userId, int offset)
        {
            RestRequest request = new RestRequest("/perms/users/" + userId + "?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetPermissionsResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public GetPermissionsResponse readPermissionsOfaGroup(String groupId, int offset)
        {
            RestRequest request = new RestRequest("/perms/groups/" + groupId + "?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetPermissionsResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String permissionsOnResources(String action, String resourceType, String subjectType, String subjectId, PermissionRule permissionRule)
        {
            RestRequest request = new RestRequest("/perms/" + action + "/" + resourceType + "/" + subjectType + "/" + subjectId, Method.POST);
            request.AddJsonBody(permissionRule);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public String permissionsOnaResource(String action, String resourceType, String resourceId, String subjectType, String subjectId, PermissionRule permissionRule)
        {
            RestRequest request = new RestRequest("/perms/" + action + "/" + resourceType + "/" + resourceId + "/" + subjectType + "/" + subjectId, Method.POST);
            request.AddJsonBody(permissionRule);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public String permissionsOnResourceChildren(String action, String resourceType, String resourceId, String resourceChildren, String subjectType, String subjectId, PermissionRule permissionRule)
        {
            RestRequest request = new RestRequest("/perms/" + action + "/" + resourceType + "/" + resourceId + "/" + resourceChildren + "/" + subjectType + "/" + subjectId, Method.POST);
            request.AddJsonBody(permissionRule);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public String permissionsOnResourceChildren(String action, String resourceType, String resourceId, String resourceChildren, String subjectType, String subjectId, PermissionRuleCreatedDocument permissionRule)
        {
            RestRequest request = new RestRequest("/perms/" + action + "/" + resourceType + "/" + resourceId + "/" + resourceChildren + "/" + subjectType + "/" + subjectId, Method.POST);
            request.AddJsonBody(permissionRule);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }
    }

    public class Permission
    {
        [JsonProperty(PropertyName = "access")]
        public String access { get; set; }
        [JsonProperty(PropertyName = "parent_id")]
        public String parent_id { get; set; }
        [JsonProperty(PropertyName = "resource_id")]
        public String resource_id { get; set; }
        [JsonProperty(PropertyName = "resource_type")]
        public String resource_type { get; set; }
        [JsonProperty(PropertyName = "permission")]
        public Dictionary<String, Object> permission { get; set; }
        public List<String> getManage(){
            try
            {
                return ((Newtonsoft.Json.Linq.JArray)permission["Manage"]).ToObject<List<String>>();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<String> getAuthorize()
        {
            try
            {
                return ((Newtonsoft.Json.Linq.JArray)permission["Authorize"]).ToObject<List<String>>();
            } catch(Exception){
                return null;
            }
        }
        public List<String> getAuthorizeCreatedDocument()
        {
            try
            {
                return ((Newtonsoft.Json.Linq.JObject)permission["created_document"])["authorize"].ToObject<List<String>>();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<String> getManageCreatedDocument()
        {
            try
            {
                return ((Newtonsoft.Json.Linq.JObject)permission["created_document"])["manage"].ToObject<List<String>>();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class PermissionRule
    {
        [JsonProperty(PropertyName = "Manage")]
        public List<String> manage { get; set; }
        [JsonProperty(PropertyName = "Authorize")]
        public List<String> authorize { get; set; }

        public void setManage(params String[] strings){
            manage = new List<string>();
            foreach(String s in strings){
                manage.Add(s);
            }
        }

        public void setAuthorize(params String[] strings)
        {
            authorize = new List<string>();
            foreach (String s in strings)
            {
                authorize.Add(s);
            }
        }
    }

    public class PermissionRuleCreatedDocument : PermissionRule
    {
        [JsonProperty(PropertyName = "Created_document")]
        public PermissionRule created_document { get; set; }
    }

    public class GetPermissionsResponse
    {
        [JsonProperty(PropertyName = "permissions")]
        public List<Permission> permissions { get; set; }
    }

    public static class PermissionValues
    {
        public static String GRANT = "grant";
        public static String REVOKE = "revoke";
        public static String USERS = "users";
        public static String USER = "user";
        public static String GROUP = "group";
        public static String DOCUMENTS = "documents";
        public static String GROUPS = "groups";
        public static String USER_SCHEMAS = "user_schemas";
        public static String REPOSITORIES = "repositories";
        public static String SCHEMAS = "schemas";
        public static String CREATE = "C";
        public static String READ = "R";
        public static String UPDATE = "U";
        public static String DELETE = "D";
        public static String LIST = "L";
        public static String ADMINISTER = "A";
        public static String SEARCH = "S";
    }
}
