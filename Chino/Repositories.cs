using System;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chino {

    public class Repositories {

        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Repositories(RestClient client) {
            this.client = client;
        }

        public GetRepositoriesResponse list(int offset)
        {
            RestRequest request = new RestRequest("/repositories?offset="+offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetRepositoriesResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }
        
        public Repository read(string repositoryId) {
            RestRequest request = new RestRequest("/repositories/" + repositoryId, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            if ((int)o["result_code"] == 200)
            {
                GetRepositoryResponse repoResponse = ((JObject)o["data"]).ToObject<GetRepositoryResponse>();
                return repoResponse.repository;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
            
        }

        public Repository create(string description) {
            RestRequest request = new RestRequest("/repositories", Method.POST);
            RepositoryRequest repositoryRequest = new RepositoryRequest();
            repositoryRequest.description = description;
            request.AddJsonBody(repositoryRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            if ((int)o["result_code"] == 200)
            {
                GetRepositoryResponse repoResponse = ((JObject)o["data"]).ToObject<GetRepositoryResponse>();
                return repoResponse.repository;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Repository update(string repositoryId, string description) {
            RestRequest request = new RestRequest("/repositories/" + repositoryId, Method.PUT);
            RepositoryRequest repositoryRequest = new RepositoryRequest();
            repositoryRequest.description = description;
            request.AddJsonBody(repositoryRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            if ((int)o["result_code"] == 200) {
                GetRepositoryResponse repoResponse = ((JObject)o["data"]).ToObject<GetRepositoryResponse>();
                return repoResponse.repository;
            } else {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String delete(string repositoryId, bool force) {
            RestRequest request;
            if (force) {
              request = new RestRequest("/repositories/" + repositoryId + "?force=true", Method.DELETE);
            } else {
              request = new RestRequest("/repositories/" + repositoryId, Method.DELETE);
            }
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            return (String)o["result"];
        }
    }

    //Below there are the classes used to get and upload data
    public class Repository
    {
        [JsonProperty(PropertyName = "repository_id")]
        public String repository_id { get; set; }
        [JsonProperty(PropertyName = "description")]
        public String description { get; set; }
        [JsonProperty(PropertyName = "is_active")]
        public Boolean is_active { get; set; }
        [JsonProperty(PropertyName = "last_update")]
        public DateTime last_update { get; set; }
        [JsonProperty(PropertyName = "insert_date")]
        public DateTime insert_date { get; set; }
        
    }

    public class GetRepositoryResponse
    {
        [JsonProperty(PropertyName = "repository")]
        public Repository repository { get; set; }
    }

    public class RepositoryRequest
    {
        [JsonProperty(PropertyName = "description")]
        public String description { get; set; }
    }

    public class GetRepositoriesResponse
    {
        [JsonProperty(PropertyName = "count")]
        public int count { get; set; }
        [JsonProperty(PropertyName = "total_count")]
        public int total_count { get; set; }
        [JsonProperty(PropertyName = "limit")]
        public int limit { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int offset { get; set; }
        [JsonProperty(PropertyName = "repositories")]
        public List<Repository> repositories { get; set; }
    }

}
