using System;
using System.Collections.Generic;
using System.Threading;
using Chino;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChinoTest
{
    [TestClass]
    public class SearchUnitTest
    {
        private string _userSchemaId1 = "";
        private string _schemaId = "";
        private string _userId = "";
        private string _documentId = "";
        private string _repositoryId = "";

        private static ChinoAPI chino;

        
        [AssemblyInitialize]
        public static void beforeAll(TestContext ctx)
        {
            Const._customerId = Environment.GetEnvironmentVariable("customer_id");
            Const._customerKey= Environment.GetEnvironmentVariable("customer_key");
            Const._hostUrl = Environment.GetEnvironmentVariable("host") ?? "https://api.test.chino.io/v1";
            
            chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
        }
        
        [TestInitialize]
        public void startup()
        {
            Console.WriteLine($"HOST: {Const._hostUrl}");
            Console.WriteLine($"ID  : ********{Const._customerId.Substring(Const._customerId.Length - 5)}");
            Console.WriteLine($"KEY : ********{Const._customerKey.Substring(Const._customerKey.Length - 5)}");
            Console.WriteLine("Creating test objects...");
            
            Const.deleteAll(chino);
            
            Repository repo = chino.repositories.create("test_repo_description");
            _repositoryId = repo.repository_id;
            SchemaRequest schemaRequest = new SchemaRequest {description = "schema_dotnet_sdk"};
            SchemaStructure schemaStructure = new SchemaStructure();
            List<SchemaField> fieldList = new List<SchemaField>
            {
                new SchemaField("test_integer", "integer", true),
                new SchemaField("test_string", "string", true),
                new SchemaField("test_boolean", "boolean", true),
                new SchemaField("test_date", "date", true),
                new SchemaField("test_file", "blob")
            };
            schemaStructure.fields = fieldList;
            schemaRequest.structure = schemaStructure;
            
            Schema schema = chino.schemas.create(_repositoryId, schemaRequest);
            _schemaId = schema.schema_id;

            List<UserSchemaField> userSchemaFields = new List<UserSchemaField>
            {
                new UserSchemaField("name", "string", true), new UserSchemaField("last_name", "string", true)
            };
            UserSchemaStructure userSchemaStructure = new UserSchemaStructure {fields = userSchemaFields};
            UserSchemaRequest userSchemaRequest = new UserSchemaRequest
            {
                structure = userSchemaStructure, description = "user_schema"
            };
            UserSchema userSchema = chino.userSchemas.create(userSchemaRequest);

            Dictionary<string, object> attributes = new Dictionary<string, object>
            {
                {"name", "Giacomino"}, {"last_name", "Poretti"}
            };

            chino.users.create("jack@gmail.com", "password", attributes, userSchema.user_schema_id);

            Thread.Sleep(8000);

            Dictionary<string, object> content = new Dictionary<string, object>
            {
                {"test_integer", 123},
                {"test_string", "string_value"},
                {"test_boolean", true},
                {"test_date", "1997-12-03"}
            };
            Document document = chino.documents.create(content, _schemaId);
            
            _documentId = document.document_id;
            Console.WriteLine(document.ToStringExtension());
            content = new Dictionary<string, object>
            {
                {"test_integer", 1234},
                {"test_string", "string_value_2"},
                {"test_boolean", false},
                {"test_date", "1997-12-04"}
            };
            chino.documents.create(content, _schemaId);

            Thread.Sleep(3000);
        }

        [TestMethod]
        public void testSearch()
        {
            chino.search.
        }
    }
}