using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using System.Collections;

namespace Chino
{
    public class ChinoAPI {

        public Documents documents;
        public Users users;
        public UserSchemas userSchemas;
        public Repositories repositories;
        public Schemas schemas;
        public RestClient client;

        //Constructor called when a customer needs to be authenticated
        public ChinoAPI(string hostUrl, string customerId, string customerKey)
        {
            initClient(hostUrl, customerId, customerKey);
            initObject();
        }

        //Constructor called when a user needs to be authenticated
        public ChinoAPI(string hostUrl)
        {
            initClient(hostUrl);
            initObject();
        }

        private void initClient(string hostUrl, string customerId, string customerKey)
        {
            ChinoClient chinoClient = new ChinoClient(hostUrl);
            chinoClient.setAuth(customerId, customerKey);
            client = chinoClient.getClient();
        }

        private void initClient(string hostUrl)
        {
            ChinoClient chinoClient = new ChinoClient(hostUrl);
            client = chinoClient.getClient();
        }

        private void initObject(){
            documents = new Documents(client);
            users = new Users(client);
            userSchemas = new UserSchemas(client);
            repositories = new Repositories(client);
            schemas = new Schemas(client);
        }
    }

    public class ChinoClient
    {
        private RestClient client;

        //When a new ChinoClient is created the RestClient is initialized
        public ChinoClient(string hostUrl)
        {
            client = new RestClient(hostUrl);
        }

        public RestClient getClient()
        {
            return client;
        }

        //Using this method you can set the default auth as a customer
        public void setAuth(string customerId, string customerKey)
        {
            //Here the encoded string for the authentication is created
            var tot = customerId + ":" + customerKey;
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(tot);
            string encodedText = Convert.ToBase64String(bytesToEncode);
            client.RemoveDefaultParameter("Authorization");
            //In this way you set a default header that exists in every call you do with this client
            client.AddDefaultHeader("Authorization", "Basic " + encodedText);
        }

        //Using this method you can set the default auth as a user
        public void setAuth(string token)
        {
            //Here the encoded string for the authentication is created
            var tot = "ACCES_TOKEN:" + token;
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(tot);
            string encodedText = Convert.ToBase64String(bytesToEncode);
            client.RemoveDefaultParameter("Authorization");
            //In this way you set a default header that exists in every call you do with this client
            client.AddDefaultHeader("Authorization", "Basic " + encodedText);
        }

    }

    //This is an extension to print all variables in a Class
    public static class Utils
    {
        public static string ToStringExtension(this object obj)
        {
            StringBuilder sb = new StringBuilder();
            foreach (System.Reflection.PropertyInfo property in obj.GetType().GetProperties())
            {
                sb.Append(property.Name);
                sb.Append(": ");
                if (property.GetIndexParameters().Length > 0)
                {
                    sb.Append("Indexed Property cannot be used");
                }
                else
                {
                    var value = property.GetValue(obj, null);
                    if (value == null)
                    {
                        sb.Append("null");
                    }
                    else
                    {
                        if (value is IList)
                        {
                            foreach (var x in (IList)value)
                            {
                                sb.Append(System.Environment.NewLine);
                                sb.Append(x.ToStringExtension());
                            }
                        }
                        else if (value.GetType().Namespace == "Chino")
                        {
                            sb.Append(System.Environment.NewLine);
                            sb.Append(value.ToStringExtension());
                        }
                        else if (value is IDictionary)
                        {
                            foreach (var x in (IDictionary)value)
                            {
                                sb.Append(System.Environment.NewLine);
                                sb.Append(x.ToStringExtension());
                            }
                        }
                        else
                        {
                            sb.Append(value);
                        }
                    }
                    
                }
                sb.Append(System.Environment.NewLine);
            }
            return sb.ToString();
        }

        public static String checkType(Type t){
            if (t == typeof(String) || t == typeof(string))
            {
                return "string";
            }
            else if (t == typeof(int) || t == typeof(Int32) || t == typeof(Int16) || t == typeof(Int64))
            {
                return "integer";
            }
            else if (t == typeof(bool) || t == typeof(Boolean))
            {
                return "boolean";
            }
            else if(t == typeof(float)){
                return "float";
            }
            else if(t == typeof(DateTime)){
                return "date";
            }
            else if(t == typeof(TimeSpan)){
                return "time";
            }
            else if(t == typeof(FileParameter)){
                return "blob";
            } else {
                throw new ChinoApiException("error, invalid type: "+t+".");
            }
        }

    }
}
