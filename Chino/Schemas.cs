using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chino
{
    public class Schemas {

        RestClient client;

        //The client is passed in the constructor and is saved in the "client" variable
        public Schemas(RestClient client) {
            this.client = client;
        }

        public GetSchemasResponse list(String repositoryId, int offset)
        {
            RestRequest request = new RestRequest("/repositories/"+repositoryId+"/schemas?offset=" + offset, Method.GET);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<GetSchemasResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Schema read(String schemaId)
        {
            RestRequest request = new RestRequest("/schemas/" + schemaId, Method.GET);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetSchemaResponse schemaResponse = ((JObject)o["data"]).ToObject<GetSchemaResponse>();
                return schemaResponse.schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }

        }

        public Schema create(String repositoryId, SchemaRequest schemaRequest)
        {
            RestRequest request = new RestRequest("/repositories/"+repositoryId+"/schemas", Method.POST);
            request.AddJsonBody(schemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetSchemaResponse schemaResponse = ((JObject)o["data"]).ToObject<GetSchemaResponse>();
                return schemaResponse.schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Schema create(String repositoryId, String description, SchemaStructure schemaStructure)
        {
            RestRequest request = new RestRequest("/repositories/" + repositoryId + "/schemas", Method.POST);
            SchemaRequest schemaRequest = new SchemaRequest();
            schemaRequest.description = description;
            schemaRequest.structure = schemaStructure;
            request.AddJsonBody(schemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetSchemaResponse schemaResponse = ((JObject)o["data"]).ToObject<GetSchemaResponse>();
                return schemaResponse.schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Schema create(String repositoryId, String description, Type myClass)
        {
            RestRequest request = new RestRequest("/repositories/" + repositoryId + "/schemas", Method.POST);
            SchemaRequest schemaRequest = new SchemaRequest();
            schemaRequest.description = description;
            List<SchemaField> fields= new List<SchemaField>();
            SchemaStructure schemaStructure = new SchemaStructure();
            foreach (System.Reflection.FieldInfo property in myClass.GetFields())
            {
                SchemaField f = new SchemaField(property.Name, Utils.checkType(property.FieldType));
                fields.Add(f);
            }
            schemaStructure.fields = fields;
            myClass.GetProperties();
            schemaRequest.structure = schemaStructure;
            request.AddJsonBody(schemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetSchemaResponse schemaResponse = ((JObject)o["data"]).ToObject<GetSchemaResponse>();
                return schemaResponse.schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Schema update(String schemaId, SchemaRequest schemaRequest)
        {
            RestRequest request = new RestRequest("/schemas/" + schemaId, Method.PUT);
            request.AddJsonBody(schemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetSchemaResponse schemaResponse = ((JObject)o["data"]).ToObject<GetSchemaResponse>();
                return schemaResponse.schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Schema update(String schemaId, String description, SchemaStructure schemaStructure)
        {
            SchemaRequest schemaRequest = new SchemaRequest();
            schemaRequest.description = description;
            schemaRequest.structure = schemaStructure;
            RestRequest request = new RestRequest("/schemas/" + schemaId, Method.PUT);
            request.AddJsonBody(schemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetSchemaResponse schemaResponse = ((JObject)o["data"]).ToObject<GetSchemaResponse>();
                return schemaResponse.schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public Schema update(String schemaId, String description, Type myClass)
        {
            RestRequest request = new RestRequest("/schemas/" + schemaId, Method.PUT);
            SchemaRequest schemaRequest = new SchemaRequest();
            schemaRequest.description = description;
            List<SchemaField> fields = new List<SchemaField>();
            SchemaStructure schemaStructure = new SchemaStructure();
            foreach (System.Reflection.FieldInfo property in myClass.GetFields())
            {
                SchemaField f = new SchemaField(property.Name, Utils.checkType(property.FieldType));
                fields.Add(f);
            }
            schemaStructure.fields = fields;
            myClass.GetProperties();
            schemaRequest.structure = schemaStructure;
            request.AddJsonBody(schemaRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                GetSchemaResponse schemaResponse = ((JObject)o["data"]).ToObject<GetSchemaResponse>();
                return schemaResponse.schema;
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public String delete(string schemaId, bool force)
        {
            RestRequest request;
            if (force)
            {
                request = new RestRequest("/schemas/" + schemaId + "?force=true", Method.DELETE);
            }
            else
            {
                request = new RestRequest("/schemas/" + schemaId, Method.DELETE);
            }
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }
    }

    //Below there are the classes used to get and upload data
    public class Schema {
        [JsonProperty(PropertyName = "repository_id")]
        public String repository_id { get; set; }
        [JsonProperty(PropertyName = "schema_id")]
        public String schema_id { get; set; }
        [JsonProperty(PropertyName = "description")]
        public String description { get; set; }
        [JsonProperty(PropertyName = "is_active")]
        public Boolean is_active { get; set; }
        [JsonProperty(PropertyName = "last_update")]
        public DateTime last_update { get; set; }
        [JsonProperty(PropertyName = "insert_date")]
        public DateTime insert_date { get; set; }
        [JsonProperty(PropertyName = "structure")]
        public SchemaStructure structure { get; set; }
    }

    public class GetSchemaResponse 
    {
        [JsonProperty(PropertyName = "result")]
        public String result { get; set; }
        [JsonProperty(PropertyName = "schema")]
        public Schema schema { get; set; }
    }

    public class GetSchemasResponse
    {
        [JsonProperty(PropertyName = "count")]
        public int count { get; set; }
        [JsonProperty(PropertyName = "total_count")]
        public int total_count { get; set; }
        [JsonProperty(PropertyName = "limit")]
        public int limit { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int offset { get; set; }
        [JsonProperty(PropertyName = "schemas")]
        public List<Schema> schemas { get; set; }
    }

    public class SchemaRequest
    {
        [JsonProperty(PropertyName = "description")]
        public String description { get; set; }
        [JsonProperty(PropertyName = "structure")]
        public SchemaStructure structure { get; set; }
    }

    public class SchemaStructure
    {
        [JsonProperty(PropertyName = "fields")]
        public List<SchemaField> fields { get; set; }
    }

    public class SchemaField
    {
        [JsonProperty(PropertyName = "name")]
        public String name { get; set; }
        [JsonProperty(PropertyName = "type")]
        public String type { get; set; }
        [JsonProperty(PropertyName = "indexed")]
        public Boolean indexed { get; set; }

        public SchemaField() {}

        public SchemaField(String name, String type)
        {
            this.name = name;
            this.type = type;
            this.indexed = false;
        }

        public SchemaField(String name, String type, Boolean indexed)
        {
            this.name = name;
            this.type = type;
            this.indexed = indexed;
        }
    }
}
