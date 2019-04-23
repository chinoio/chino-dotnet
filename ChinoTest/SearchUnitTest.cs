using System;
using System.Collections.Generic;
using System.Threading;
using Chino;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Chino.OrderEnum;

namespace ChinoTest
{
    [TestClass]
    public class SearchUnitTest
    {
        private string _userSchemaId = "";
        private string _schemaId = "";
        private string _userId = "";
        private string _documentId = "";
        private string _repositoryId = "";

        private static ChinoAPI chino;
        
        [TestInitialize]
        public void startup()
        {
            Const.init();
            
            Console.WriteLine($"HOST: {Const._hostUrl}");
            Console.WriteLine($"ID  : ********{Const._customerId.Substring(Const._customerId.Length - 5)}");
            Console.WriteLine($"KEY : ********{Const._customerKey.Substring(Const._customerKey.Length - 5)}");

            chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            Console.WriteLine("Cleanin' up test environment...");
            Const.deleteAll(chino);

            Console.WriteLine("Creating test objects...");
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
                new UserSchemaField("name", "string", true), 
                new UserSchemaField("last_name", "string", true)
            };
            UserSchemaStructure userSchemaStructure = new UserSchemaStructure {fields = userSchemaFields};
            UserSchemaRequest userSchemaRequest = new UserSchemaRequest
            {
                structure = userSchemaStructure, description = "user_schema"
            };
            UserSchema userSchema = chino.userSchemas.create(userSchemaRequest);
            _userSchemaId = userSchema.user_schema_id;

            Dictionary<string, object> attributes = new Dictionary<string, object>
            {
                {"name", "Giacomino"}, {"last_name", "Poretti"}
            };
            chino.users.create("jack@gmail.com", "password", attributes, userSchema.user_schema_id);

            attributes = new Dictionary<string, object>
            {
                {"name", "Andrea"}, {"last_name", "Arighi"}
            };
            chino.users.create("tech-support@chino.io", "somePassword", attributes, userSchema.user_schema_id);

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
            // Search documents
            var result = chino.search.documents(_schemaId)
                .setResultType(ResultTypeEnum.Only_Id)
                .addSortRule("test_string", Asc)
                .with("test_integer", FilterOperator.filter(FilterOperatorEnum.GreaterThan), 1230)
                .buildSearch().execute();

            Assert.AreEqual(
                1,
                result.ids.Count,
                "Search returned wrong ID count"
            );
            Assert.IsNull(result.documents); // should only return IDs

            // Search users
            var invalidNames = new List<string>
            {
                "Antonio",
                "Marco",
                "Lucia"
            };
            var search1 = chino.search.users(_userSchemaId)
                .setResultType(ResultTypeEnum.Full_Content)
                .addSortRule("last_name", Desc)
                .with("last_name", FilterOperator.filter(FilterOperatorEnum.Like), "*i")
                .andNot("name", FilterOperator.filter(FilterOperatorEnum.In), invalidNames)
                .buildSearch();
                
            Assert.AreNotEqual(null, search1.ToString());

            var users1 = search1.execute().users;
            
            Assert.AreEqual(
                2,
                users1.Count,
                "Search returned wrong count"
            );
            Assert.AreEqual(2, users1.Count);
            
            
            var search2 = chino.search.users(_userSchemaId)
                .setResultType(ResultTypeEnum.Full_Content)
                .addSortRule("last_name", Desc)
                .with("last_name", FilterOperator.filter(FilterOperatorEnum.Like), "*i")
                .and(
                    SearchQueryBuilder<GetUsersResponse>.not("name", FilterOperator.filter(FilterOperatorEnum.In), invalidNames)
                ).buildSearch();
            var users2 = search2.execute().users;

            for (var i = 0; i < users1.Count; i++)
            {
                Assert.AreEqual(users1[i].user_id, users2[i].user_id);
            }
            
            
            var search3 = chino.search.users(_userSchemaId)
                .setResultType(ResultTypeEnum.Full_Content)
                .addSortRule("last_name", Desc)
                .with("last_name", FilterOperator.filter(FilterOperatorEnum.Like), "*i")
                .orNot("name", FilterOperator.filter(FilterOperatorEnum.Equals), "Andrea")
                .buildSearch();
            var users3 = search3.execute().users;
            
            Assert.AreEqual(
                2,
                users3.Count,
                "Search returned wrong count"
            );
            Assert.AreEqual(2, users3.Count);
            
            
            var search4 = chino.search.users(_userSchemaId)
                .setResultType(ResultTypeEnum.Full_Content)
                .addSortRule("last_name", Desc)
                .with("last_name", FilterOperator.filter(FilterOperatorEnum.Like), "*i")
                .or(
                    SearchQueryBuilder<GetUsersResponse>
                        .not("name", FilterOperator.filter(FilterOperatorEnum.Equals), "Andrea")
                ).buildSearch();
            var users4 = search4.execute().users;
            
            Assert.AreEqual(
                2,
                users4.Count,
                "Search returned wrong count"
            );

            for (var i = 0; i < users3.Count; i++)
            {
                Assert.AreEqual(users3[i].user_id, users4[i].user_id);
            }
        }

        [TestMethod]
        public void testUsernameExists()
        {
            var search = chino.search.users(_userSchemaId)
                .setResultType(ResultTypeEnum.Username_Exists)
                .addSortRule("name", Desc)
                .with("username", FilterOperator.filter(FilterOperatorEnum.Equals), "tech-support@chino.io")
                .buildSearch();

            Assert.AreNotEqual(null, search.ToString());
            Assert.IsTrue(search.execute().exists);
        }

        [TestMethod]
        public void testOffsetLimit()
        {
            var testDocumentsCounter = 10;
            string[] documents = new string[testDocumentsCounter];
            // create 10 documents
            for (int i = 0; i < testDocumentsCounter; i++)
            {
                var content = new Dictionary<string, object>
                {
                    {"test_integer", i},
                    {"test_string", "test offset/limit"},
                    {"test_boolean", i % 2 == 0}, // TRUE if the integer is an even number
                    {"test_date", "2019-04-23"}
                };
                documents[i] = chino.documents.create(content, _schemaId).document_id;
            }

            // search query 1 : test_string matches the String for this test and the integer is an even number
            // filter the documents of this test case by the value of test_string
            var search1 = chino.search.documents(_schemaId)
                .setResultType(ResultTypeEnum.Full_Content).addSortRule("test_integer", Asc)
                .with("test_string", FilterOperator.filter(FilterOperatorEnum.Equals), "test offset/limit")
                .and("test_boolean", FilterOperator.filter(FilterOperatorEnum.Is), true)
                .buildSearch();

            // search query 2 : first half of the integers plus the results with an even integer value
            // filter the documents of this test case by the value of test_date
            var search2 = chino.search.documents(_schemaId)
                .setResultType(ResultTypeEnum.Full_Content).addSortRule("test_integer", Asc)
                .with(
                    SearchQueryBuilder<GetDocumentsResponse>
                        .with("test_integer", FilterOperator.filter(FilterOperatorEnum.LowerThan),
                            testDocumentsCounter / 2)
                        .or("test_boolean", FilterOperator.filter(FilterOperatorEnum.Is), true)
                )
                .and("test_date", FilterOperator.filter(FilterOperatorEnum.Equals), "2019-04-23")
                .buildSearch();

            // test search 1
            var offset = 0;
            var limit = 2;
            var docs = search1.execute(offset, limit).documents;
            while (docs.Count > 0)
            {
                Assert.IsTrue(docs.Count <= limit);
                foreach (var doc in docs)
                {
                    Console.WriteLine($"Document {doc.content["test_integer"]}");
                    doc.content.TryGetValue("test_string", out var test_string);
                    Assert.IsNotNull(test_string);
                    Assert.AreEqual("test offset/limit", (string) test_string);
                }
                offset += limit;
                docs = search1.execute(offset, limit).documents;
                Console.WriteLine("*** ++offset ***");
            }
            
            // test search 1
            offset = 0;
            limit = 2;
            docs = search2.execute(offset, limit).documents;
            while (docs.Count > 0)
            {
                Assert.IsTrue(docs.Count <= limit);
                foreach (var doc in docs)
                {
                    Console.WriteLine($"Document {doc.content["test_integer"]}");
                    int test_integer = int.Parse(doc.content["test_integer"].ToString());
                    bool test_boolean = bool.Parse(doc.content["test_boolean"].ToString());
                    Assert.IsTrue(
                        test_integer < testDocumentsCounter/2 || test_boolean,
                        $"Search condition not satisfied for integer {test_integer}"
                    );
                }
                Console.WriteLine("*** ++offset ***");
                offset += limit;
                docs = search2.execute(offset, limit).documents;
            }
        }

        [TestCleanup]
        public void cleanUp()
        {
            Console.Write("Cleanin' up test environment after tests...");
            Const.deleteAll(chino);
            Console.WriteLine(" Done.");
        }
    }
}