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
        private SearchRequest searchRequest = new SearchRequest();
        private List<SortOption> sort = new List<SortOption>();
        private List<FilterOption> filter = new List<FilterOption>();
        private FilterOption filterOption;

        //The client is passed in the constructor and is saved in the "client" variable
        public Search(RestClient client) {
            this.client = client;
        }

        public DocumentsSearch documents(string schemaId)
        {
            return new DocumentsSearch(client, schemaId);
        }

        public UsersSearch users(string userSchemaId)
        {
            return new UsersSearch(client, userSchemaId);
        }
        
        
        /* OLD SEARCH API - the following API is deprecated and may be removed in a future release */
        
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public GetDocumentsResponse searchDocuments(String schemaId, SearchRequest searchRequest)
        {
            RestRequest request = new RestRequest("/search/documents/"+schemaId, Method.POST);
            request.AddJsonBody(searchRequest);
            this.searchRequest = new SearchRequest();
            this.sort = new List<SortOption>();
            this.filter = new List<FilterOption>();
            this.filterOption = new FilterOption();
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

        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public GetDocumentsResponse searchDocuments(String schemaId, String resultType, Boolean withoutIndex, String filterType, List<SortOption> sort, List<FilterOption> filter){
            SearchRequest searchRequest = new SearchRequest();
            searchRequest.result_type = resultType;
            searchRequest.filter_type = filterType;
            searchRequest.sort = sort;
            searchRequest.filter = filter;
            return searchDocuments(schemaId, searchRequest);
        }

        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public GetUsersResponse searchUsers(String userSchemaId, SearchRequest searchRequest)
        {
            RestRequest request = new RestRequest("/search/users/" + userSchemaId, Method.POST);
            request.AddJsonBody(searchRequest);
            this.searchRequest = new SearchRequest();
            this.sort = new List<SortOption>();
            this.filter = new List<FilterOption>();
            this.filterOption = new FilterOption();
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
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

        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public GetUsersResponse searchUsers(String userSchemaId, String resultType, Boolean withoutIndex, String filterType, List<SortOption> sort, List<FilterOption> filter)
        {
            SearchRequest searchRequest = new SearchRequest();
            searchRequest.result_type = resultType;
            searchRequest.filter_type = filterType;
            searchRequest.sort = sort;
            searchRequest.filter = filter;
            return searchUsers(userSchemaId, searchRequest);
        }

        /*
         * Those functions below are used to make a search in a different way 
         */

        //This is called when you want to make a sort of a certain field in an ascending order
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search sortAscBy(String field)
        {
            //This simply adds a new SortOption to the private List "sort" of the class
            SortOption sortOption = new SortOption(field, "asc");
            sort.Add(sortOption);
            //Everytime you add a new SortOption you have to store the List in the searchRequest variable, which will be finally used to make the request
            searchRequest.sort = sort;
            return this;
        }

        //This is called when you want to make a sort of a certain field in a descending order
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search sortDescBy(String field)
        {
            SortOption sortOption = new SortOption(field, "desc");
            sort.Add(sortOption);
            searchRequest.sort = sort;
            return this;
        }

        //This is called when you want to specify a result type. If you don't call this function the default value is "FULL_CONTENT"
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search resultType(String resultType)
        {
            searchRequest.result_type = resultType;
            return this;
        }

        /* 
         * This is the first function that needs to be called and sets result_type and without_index variables at their default value.
         * It also calls the filterOperation function which creates a new FilterOption and sets its "field" value;
         */ 
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search where(String field)
        {
            searchRequest.result_type = "FULL_CONTENT";
            filterOperation(field);
            return this;
        }

        /*
         * This is the last function called which sets filter_type to "or" if there is only one FilterOption (initialized by the where(...) function)
         * It sets the schemaId and finally performs the search request, calling the function searchDocuments passing the class variable searchRequest
         */
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public GetDocumentsResponse searchDocuments(String schemaId)
        {
            if (searchRequest.filter_type == null)
                searchRequest.filter_type = "or";  
            return searchDocuments(schemaId, searchRequest);
        }

        public GetUsersResponse searchUsers(String userSchemaId)
        {
            if (searchRequest.filter_type == null)
                searchRequest.filter_type = "or";
            return searchUsers(userSchemaId, searchRequest);
        }

        //This function is called if you want to make a request with filter_type set to "and" 
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search and(String field)
        {
            //If filter_type value is set to "or" it raises an error
            if (searchRequest.filter_type == "or")
                throw new ChinoApiException("Wrong filter operations!");
            //If the value is "and" or is "null"(which is the case of the first call) it sets the value to "and"
            searchRequest.filter_type = "and";
            return filterOperation(field);
        }

        //This function is called if you want to make a request with filter_type set to "or"
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search or(String field)
        {
            if (searchRequest.filter_type == "and")
                throw new ChinoApiException("Wrong filter operations!");
            searchRequest.filter_type = "or";
            return filterOperation(field);
        }

        //This function creates a new FilterOption and adds it to the private List "filter", then sets the value of the searchRequest.filter variable to the List updated
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        private Search filterOperation(String field)
        {
            filterOption = new FilterOption();
            filterOption.field = field;
            filter.Add(filterOption);
            searchRequest.filter = filter;
            return this;
        }

        //Those functions below set the value and type of the FilterOption
        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search eq(Object value)
        {
            filterOption.value = value;
            filterOption.type = "eq";
            return this;
        }

        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search gt(Object value)
        {
            filterOption.value = value;
            filterOption.type = "gt";
            return this;
        }

        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search gte(Object value)
        {
            filterOption.value = value;
            filterOption.type = "gte";
            return this;
        }

        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search lt(Object value)
        {
            filterOption.value = value;
            filterOption.type = "lt";
            return this;
        }

        [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
        public Search lte(Object value)
        {
            filterOption.value = value;
            filterOption.type = "lte";
            return this;
        }
    }
    

    /* OLD SEARCH API - the following API is deprecated and may be removed in a future release */

    [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
    public class FilterOption
    {
        [JsonProperty(PropertyName = "field")]
        public String field { get; set; }
        [JsonProperty(PropertyName = "type")]
        public String type { get; set; }
        [JsonProperty(PropertyName = "value")]
        public Object value { get; set; }
        public FilterOption() { }
        public FilterOption(String field, String type, Object value)
        {
            this.field = field;
            this.type = type;
            this.value = value;
        }
    }

    [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
    public class SortOption
    {
        [JsonProperty(PropertyName = "field")]
        public String field { get; set; }
        [JsonProperty(PropertyName = "order")]
        public String order { get; set; }
        public SortOption() { }
        public SortOption(String field, String order)
        {
            this.field = field;
            this.order = order;
        }
    }

    [Obsolete("The old Search API are deprecated and might be removed in a future release. Please use the new Search API.")]
    public class SearchRequest
    {
        [JsonProperty(PropertyName = "result_type")]
        public String result_type { get; set; }
        [JsonProperty(PropertyName = "filter_type")]
        public String filter_type { get; set; }
        [JsonProperty(PropertyName = "sort")]
        public List<SortOption> sort { get; set; }
        [JsonProperty(PropertyName = "filter")]
        public List<FilterOption> filter { get; set; }
    }
}