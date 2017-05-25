using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chino
{
    public class Applications
    {
        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Applications(RestClient client)
        {
            this.client = client;
        }

        public GetApplicationsResponse list(int offset)
        {
            RestRequest request = new RestRequest("/auth/applications?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetApplicationsResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Application read(string applicationId)
        {
            RestRequest request = new RestRequest("/auth/applications/" + applicationId, Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            if ((int)o["result_code"] == 200)
            {
                GetApplicationResponse appResponse = ((JObject)o["data"]).ToObject<GetApplicationResponse>();
                return appResponse.application;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }

        }

        public Application create(string name, string grant_type, string redirect_url)
        {
            RestRequest request = new RestRequest("/auth/applications", Method.POST);
            CreateApplicationRequest applicationRequest = new CreateApplicationRequest();
            applicationRequest.name = name;
            applicationRequest.grant_type = grant_type;
            applicationRequest.redirect_url = redirect_url;
            request.AddJsonBody(applicationRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            if ((int)o["result_code"] == 200)
            {
                GetApplicationResponse appResponse = ((JObject)o["data"]).ToObject<GetApplicationResponse>();
                return appResponse.application;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Application update(string applicationId, string name, string grant_type, string redirect_url)
        {
            RestRequest request = new RestRequest("/auth/applications/" + applicationId, Method.PUT);
            CreateApplicationRequest applicationRequest = new CreateApplicationRequest();
            applicationRequest.name = name;
            applicationRequest.grant_type = grant_type;
            applicationRequest.redirect_url = redirect_url;
            request.AddJsonBody(applicationRequest);
            IRestResponse response = client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ChinoApiException(response.ErrorMessage);
            }
            String content = response.Content.ToString();
            JObject o = JObject.Parse(content);
            if ((int)o["result_code"] == 200)
            {
                GetApplicationResponse appResponse = ((JObject)o["data"]).ToObject<GetApplicationResponse>();
                return appResponse.application;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String delete(string applicationId, bool force)
        {
            RestRequest request;
            if (force)
            {
                request = new RestRequest("/auth/applications/" + applicationId + "?force=true", Method.DELETE);
            }
            else
            {
                request = new RestRequest("/auth/applications/" + applicationId, Method.DELETE);
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
}

public class CreateApplicationRequest
{
    [JsonProperty(PropertyName = "name")]
    public String name { get; set; }
    [JsonProperty(PropertyName = "grant_type")]
    public String grant_type { get; set; }
    [JsonProperty(PropertyName = "redirect_url")]
    public String redirect_url { get; set; }
}

public class Application
{
    [JsonProperty(PropertyName = "app_secret")]
    public String app_secret { get; set; }
    [JsonProperty(PropertyName = "grant_type")]
    public String grant_type { get; set; }
    [JsonProperty(PropertyName = "app_name")]
    public String app_name { get; set; }
    [JsonProperty(PropertyName = "redirect_url")]
    public String redirect_url { get; set; }
    [JsonProperty(PropertyName = "app_id")]
    public String app_id { get; set; }
}

public class ApplicationsObject
{
    [JsonProperty(PropertyName = "app_id")]
    public String app_id { get; set; }
    [JsonProperty(PropertyName = "app_name")]
    public String app_name { get; set; }
}

public class GetApplicationResponse
{
    [JsonProperty(PropertyName = "application")]
    public Application application { get; set; }
}

public class GetApplicationsResponse
{
    [JsonProperty(PropertyName = "applications")]
    public List<ApplicationsObject> applications { get; set; }
}