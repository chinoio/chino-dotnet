using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class Search
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Search(RestClient client) {
            this.client = client;
        }

        public GetDocumentsResponse searchDocuments(SearchRequest searchRequest)
        {
            RestRequest request = new RestRequest("/search/", Method.POST);
            request.AddJsonBody(searchRequest);
            IRestResponse response = client.Execute(request);
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

        public GetDocumentsResponse searchDocuments(String schemaId, String resultType, Boolean withoutIndex, String filterType, List<SortOption> sort, List<FilterOption> filter){
            SearchRequest searchRequest = new SearchRequest();
            searchRequest.schema_id = schemaId;
            searchRequest.result_type = resultType;
            searchRequest.without_index = withoutIndex;
            searchRequest.filter_type = filterType;
            searchRequest.sort = sort;
            searchRequest.filter = filter;
            return searchDocuments(searchRequest);
        }
    }

    public class FilterOption
    {
        [JsonProperty(PropertyName = "field")]
        public String field { get; set; }
        [JsonProperty(PropertyName = "type")]
        public String type { get; set; }
        [JsonProperty(PropertyName = "value")]
        public Object value { get; set; }
        [JsonProperty(PropertyName = "case_sensitive")]
        public Boolean case_sensitive { get; set; }
        public FilterOption(String field, String type, Object value, Boolean case_sensitive)
        {
            this.field = field;
            this.type = type;
            this.value = value;
            this.case_sensitive = case_sensitive;
        }
    }

    public class SortOption
    {
        [JsonProperty(PropertyName = "field")]
        public String field { get; set; }
        [JsonProperty(PropertyName = "order")]
        public String order { get; set; }
        public SortOption(String field, String order)
        {
            this.field = field;
            this.order = order;
        }
    }

    public class SearchRequest
    {
        [JsonProperty(PropertyName = "schema_id")]
        public String schema_id { get; set; }
        [JsonProperty(PropertyName = "result_type")]
        public String result_type { get; set; }
        [JsonProperty(PropertyName = "without_index")]
        public Boolean without_index { get; set; }
        [JsonProperty(PropertyName = "filter_type")]
        public String filter_type { get; set; }
        [JsonProperty(PropertyName = "sort")]
        public List<SortOption> sort { get; set; }
        [JsonProperty(PropertyName = "filter")]
        public List<FilterOption> filter { get; set; }
    }
}
