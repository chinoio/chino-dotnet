using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class Documents
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Documents(RestClient client) {
            this.client = client;
        }

        public GetDocumentsResponse list(String schemaId, int offset)
        {
            RestRequest request = new RestRequest("/schemas/" + schemaId + "/documents?offset=" + offset, Method.GET);
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

        public GetDocumentsResponse listWithFullContent(String schemaId, int offset)
        {
            RestRequest request = new RestRequest("/schemas/" + schemaId + "/documents?full_document=true&offset=" + offset, Method.GET);
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

        public Document read(String documentId)
        {
            RestRequest request = new RestRequest("/documents/" + documentId, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetDocumentResponse documentResponse = ((JObject)o["data"]).ToObject<GetDocumentResponse>();
                return documentResponse.document;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Document create(Dictionary<String, Object> content, String schemaId)
        {
            RestRequest request = new RestRequest("/schemas/" + schemaId + "/documents", Method.POST);
            CreateDocumentRequest documentRequest = new CreateDocumentRequest();
            documentRequest.content = content;
            request.AddJsonBody(documentRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetDocumentResponse documentResponse = ((JObject)o["data"]).ToObject<GetDocumentResponse>();
                return documentResponse.document;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Document update(Dictionary<String, Object> content, String documentId)
        {
            RestRequest request = new RestRequest("/documents/" + documentId, Method.PUT);
            CreateDocumentRequest documentRequest = new CreateDocumentRequest();
            documentRequest.content = content;
            request.AddJsonBody(documentRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetDocumentResponse documentResponse = ((JObject)o["data"]).ToObject<GetDocumentResponse>();
                return documentResponse.document;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String delete(string documentId, bool force)
        {
            RestRequest request;
            if (force)
            {
                request = new RestRequest("/documents/" + documentId + "?force=true", Method.DELETE);
            }
            else
            {
                request = new RestRequest("/documents/" + documentId, Method.DELETE);
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

    //Below there are the classes used to get and upload data
    public class Document
    {
        [JsonProperty(PropertyName = "repository_id")]
        public String repository_id { get; set; }
        [JsonProperty(PropertyName = "schema_id")]
        public String schema_id { get; set; }
        [JsonProperty(PropertyName = "document_id")]
        public String document_id { get; set; }
        [JsonProperty(PropertyName = "insert_date")]
        public DateTime insert_date { get; set; }
        [JsonProperty(PropertyName = "is_active")]
        public Boolean is_active { get; set; }
        [JsonProperty(PropertyName = "last_update")]
        public DateTime last_update { get; set; }
        [JsonProperty(PropertyName = "content")]
        public Dictionary<String, Object> content { get; set; }
    }

    public class GetDocumentResponse
    {
        [JsonProperty(PropertyName = "document")]
        public Document document { get; set; }
    }

    public class GetDocumentsResponse
    {
        [JsonProperty(PropertyName = "count")]
        public int count { get; set; }
        [JsonProperty(PropertyName = "total_count")]
        public int total_count { get; set; }
        [JsonProperty(PropertyName = "limit")]
        public int limit { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int offset { get; set; }
        [JsonProperty(PropertyName = "documents")]
        public List<Document> documents { get; set; }
        [JsonProperty(PropertyName = "IDs")]
        public List<String> ids { get; set; }
    }

    public class CreateDocumentRequest
    {
        [JsonProperty(PropertyName = "content")]
        public Dictionary<String, Object> content { get; set; }
    }
}
