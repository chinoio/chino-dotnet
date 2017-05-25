using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class Collections
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Collections(RestClient client) {
            this.client = client;
        }

        public GetCollectionsResponse list(int offset)
        {
            RestRequest request = new RestRequest("/collections?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetCollectionsResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Collection read(String collectionId)
        {
            RestRequest request = new RestRequest("/collections/" + collectionId, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetCollectionResponse collectionResponse = ((JObject)o["data"]).ToObject<GetCollectionResponse>();
                return collectionResponse.collection;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Collection create(String name)
        {
            RestRequest request = new RestRequest("/collections", Method.POST);
            CreateCollectionRequest collectionRequest = new CreateCollectionRequest();
            collectionRequest.name = name;
            request.AddJsonBody(collectionRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetCollectionResponse collectionResponse = ((JObject)o["data"]).ToObject<GetCollectionResponse>();
                return collectionResponse.collection;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Collection update(String collectionId, String name)
        {
            RestRequest request = new RestRequest("/collections/" + collectionId, Method.PUT);
            CreateCollectionRequest collectionRequest = new CreateCollectionRequest();
            collectionRequest.name = name;
            request.AddJsonBody(collectionRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetCollectionResponse collectionResponse = ((JObject)o["data"]).ToObject<GetCollectionResponse>();
                return collectionResponse.collection;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String delete(string collectionId, bool force)
        {
            RestRequest request;
            if (force)
            {
                request = new RestRequest("/collections/" + collectionId + "?force=true", Method.DELETE);
            }
            else
            {
                request = new RestRequest("/collections/" + collectionId, Method.DELETE);
            }
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public GetDocumentsResponse listDocuments(String collectionId, int offset)
        {
            RestRequest request = new RestRequest("/collections/" + collectionId + "/documents?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetDocumentsResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String addDocument(String collectionId, String documentId)
        {
            RestRequest request = new RestRequest("/collections/" + collectionId + "/documents/"+documentId, Method.POST);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        public String removeDocument(String collectionId, String documentId)
        {
            RestRequest request = new RestRequest("/collections/" + collectionId + "/documents/" + documentId, Method.DELETE);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }
    }

    public class Collection
    {
        [JsonProperty(PropertyName = "insert_date")]
        public DateTime insert_date { get; set; }
        [JsonProperty(PropertyName = "is_active")]
        public Boolean is_active { get; set; }
        [JsonProperty(PropertyName = "last_update")]
        public DateTime last_update { get; set; }
        [JsonProperty(PropertyName = "name")]
        public String name { get; set; }
        [JsonProperty(PropertyName = "collection_id")]
        public String collection_id { get; set; }
    }

    public class GetCollectionResponse
    {
        [JsonProperty(PropertyName = "collection")]
        public Collection collection { get; set; }
    }

    public class GetCollectionsResponse
    {
        [JsonProperty(PropertyName = "result")]
        public String result { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int count { get; set; }
        [JsonProperty(PropertyName = "total_count")]
        public int total_count { get; set; }
        [JsonProperty(PropertyName = "limit")]
        public int limit { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int offset { get; set; }
        [JsonProperty(PropertyName = "collections")]
        public List<Collection> collections { get; set; }
    }

    public class CreateCollectionRequest
    {
        [JsonProperty(PropertyName = "name")]
        public String name { get; set; }
    }
}
