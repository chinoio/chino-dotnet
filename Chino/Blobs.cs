using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Chino
{
    public class Blobs
    {
        RestClient client;
        static int chunkSize = 1024 * 1024;

        //The client is passed in the constructor and is saved in the "client" variable
        public Blobs(RestClient client) {
            this.client = client;
        }

        public CommitBlobUploadResponse uploadBlob(String path, String documentId, String field, String fileName)
        {
            CreateBlobUploadResponse blobResponse = initUpload(documentId, field, fileName);
            FileStream file = new FileStream(path + Path.DirectorySeparatorChar + fileName, FileMode.Open);
            Byte[] bytes;
            int currentFilePosition = 0;
            file.Seek(currentFilePosition, SeekOrigin.Begin);
            while (currentFilePosition < file.Length)
            {
                int distanceFromEnd = (int)file.Length - currentFilePosition;
                if (distanceFromEnd > chunkSize)
                {
                    bytes = new Byte[chunkSize];
                    file.Read(bytes, currentFilePosition, chunkSize);
                }
                else
                {
                    bytes = new Byte[distanceFromEnd];
                    file.Read(bytes, currentFilePosition, distanceFromEnd);
                }
                uploadChunk(blobResponse.blob.upload_id, bytes, currentFilePosition, bytes.Length);
                currentFilePosition = currentFilePosition + bytes.Length;
                file.Seek(currentFilePosition, SeekOrigin.Begin);
            }
            file.Close();
            return commitUpload(blobResponse.blob.upload_id);
        }

        public CommitBlobUploadResponse commitUpload(String uploadId)
        {
            RestRequest request = new RestRequest("/blobs/commit", Method.POST);
            CommitBlobUploadRequest commitBlobUploadRequest = new CommitBlobUploadRequest();
            commitBlobUploadRequest.upload_id = uploadId;
            request.AddJsonBody(commitBlobUploadRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<CommitBlobUploadResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public CreateBlobUploadResponse uploadChunk(String upload_id, Byte[] chuckData, int offset, int length)
        {
            RestRequest request = new RestRequest("/blobs/"+upload_id, Method.PUT);

            request.AddParameter("text/json", chuckData, ParameterType.RequestBody);
            request.AddHeader("offset", offset.ToString());
            request.AddHeader("length", length.ToString());
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<CreateBlobUploadResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public CreateBlobUploadResponse initUpload(String documentId, String field, String fileName)
        {
            RestRequest request = new RestRequest("/blobs", Method.POST);
            CreateBlobUploadRequest createBlobUploadRequest = new CreateBlobUploadRequest();
            createBlobUploadRequest.document_id = documentId;
            createBlobUploadRequest.field = field;
            createBlobUploadRequest.file_name = fileName;
            request.AddJsonBody(createBlobUploadRequest);
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            if ((int)o["result_code"] == 200)
            {
                return ((JObject)o["data"]).ToObject<CreateBlobUploadResponse>();
            }
            else
            {
                throw new ChinoApiException((String)o["message"]);
            }
        }

        public GetBlobResponse get(String blobId, String destination)
        {
            GetBlobResponse blobResponse = new GetBlobResponse();
            RestRequest request = new RestRequest("/blobs/"+blobId, Method.GET);
            IRestResponse response = client.Execute(request);
            foreach (Parameter p in response.Headers)
            {
                if (p.Name == "Content-Disposition")
                {
                    blobResponse.file_name = ((String)p.Value).Substring(((String)p.Value).IndexOf("=")+1);
                }
            }
            blobResponse.path = destination + Path.DirectorySeparatorChar + blobResponse.file_name;
            Byte[] bytes = client.DownloadData(request);
            Directory.CreateDirectory(Path.GetDirectoryName(blobResponse.path));
            FileStream file = new FileStream(blobResponse.path, FileMode.Create);
            file.Write(bytes, 0, bytes.Length);
            blobResponse.size = file.Length;
            file.Close();
            String md5 = HashOf<MD5CryptoServiceProvider>(bytes, Encoding.Default).ToLower();
            String sha1 = HashOf<SHA1CryptoServiceProvider>(bytes, Encoding.Default).ToLower();
            blobResponse.sha1 = sha1;
            blobResponse.md5 = md5;
            return blobResponse;
        }

        public String delete(string blobId, bool force)
        {
            RestRequest request;
            if (force)
            {
                request = new RestRequest("/blobs/" + blobId + "?force=true", Method.DELETE);
            }
            else
            {
                request = new RestRequest("/users/" + blobId, Method.DELETE);
            }
            IRestResponse response = client.Execute(request);
            JObject o = JObject.Parse(response.Content.ToString());
            return (String)o["result"];
        }

        protected static string HashOf<TP>(byte[] bytes, Encoding enc)
        where TP : HashAlgorithm, new()
        {
            var provider = new TP();
            return BitConverter.ToString(provider.ComputeHash(bytes)).Replace("-", "");
        }
    }

    public class GetBlobResponse
    {
        [JsonProperty(PropertyName = "file_name")]
        public String file_name { get; set; }
        [JsonProperty(PropertyName = "size")]
        public long size { get; set; }
        [JsonProperty(PropertyName = "sha1")]
        public String sha1 { get; set; }
        [JsonProperty(PropertyName = "md5")]
        public String md5 { get; set; }
        [JsonProperty(PropertyName = "path")]
        public String path { get; set; }
    }

    public class CommitBlobUploadRequest
    {
        [JsonProperty(PropertyName = "upload_id")]
        public String upload_id { get; set; }
    }

    public class CommitBlobUploadResponseContent
    {
        [JsonProperty(PropertyName = "bytes")]
        public int bytes { get; set; }
        [JsonProperty(PropertyName = "blob_id")]
        public String blob_id { get; set; }
        [JsonProperty(PropertyName = "document_id")]
        public String document_id { get; set; }
        [JsonProperty(PropertyName = "sha1")]
        public String sha1 { get; set; }
        [JsonProperty(PropertyName = "md5")]
        public String md5 { get; set; }
    }

    public class CommitBlobUploadResponse 
    {
        [JsonProperty(PropertyName = "blob")]
        public CommitBlobUploadResponseContent blob { get; set; }
    }

    public class CreateBlobUploadRequest
    {
        [JsonProperty(PropertyName = "document_id")]
        public String document_id { get; set; }
        [JsonProperty(PropertyName = "field")]
        public String field { get; set; }
        [JsonProperty(PropertyName = "file_name")]
        public String file_name { get; set; }
    }

    public class CreateBlobUploadResponseContent
    {
        [JsonProperty(PropertyName = "expire_date")]
        public DateTime expire_date { get; set; }
        [JsonProperty(PropertyName = "upload_id")]
        public String upload_id { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int offset { get; set; }
    }

    public class CreateBlobUploadResponse
    {
        [JsonProperty(PropertyName = "blob")]
        public CreateBlobUploadResponseContent blob { get; set; }
    }
}
