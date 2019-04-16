using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Chino;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChinoTest
{
    [TestClass]
    public class TrivialTest
    {
        private String _userSchemaId1 = "";
        private String _userSchemaId2 = "";
        private String _userSchemaId3 = "";
        private String _schemaId1 = "";
        private String _schemaId2 = "";
        private String _schemaId3 = "";
        private String _userId = "";
        private String _documentId = "";
        private String _collectionId = "";
        private String _repositoryId = "";
        private String _groupId = "";

        [AssemblyInitialize]
        public static void beforeAll(TestContext ctx)
        {
            Const._customerId = Environment.GetEnvironmentVariable("customer_id");
            Const._customerKey= Environment.GetEnvironmentVariable("customer_key");
            Const._hostUrl = Environment.GetEnvironmentVariable("host") ?? "https://api.test.chino.io/v1";
            
            var test = Environment.GetEnvironmentVariable("automated_test");
            if (test == null) test = "";
            if (! test.Equals("allow"))
            {
                Console.WriteLine("WARNING: running tests will delete everything on the Chino.io account!");
                Console.WriteLine("Please set in your environment variables 'automated_test=allow'");
                Environment.Exit(1);
            }
        }
        
        [TestInitialize]
        public void Startup()
        {
            Console.WriteLine($"HOST: {Const._hostUrl}");
            Console.WriteLine($"ID  : ********{Const._customerId.Substring(Const._customerId.Length - 5)}");
            Console.WriteLine($"KEY : ********{Const._customerKey.Substring(Const._customerKey.Length - 5)}");
        }


        [TestMethod]
        public void testRepositories()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            _repositoryId = repo.repository_id;
            Console.WriteLine(chino.repositories.read(_repositoryId).ToStringExtension());
            Console.WriteLine(chino.repositories.update(_repositoryId, "test_repo_description_updated").ToStringExtension());
            Console.WriteLine(chino.repositories.create("test_repo_description_2").ToStringExtension());
            Console.WriteLine(chino.repositories.list(0).ToStringExtension());
            Console.WriteLine(chino.repositories.delete(_repositoryId, true));
        }

        [TestMethod]
        public void testApplications()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            
            // password - confidential
            Application app = chino.applications.create("app_pswd_sdk_dotnet", "password");
            var appId = app.app_id;
            Console.WriteLine(chino.applications.read(appId).ToStringExtension());
            Console.WriteLine(chino.applications.update(appId, "app_pswd_sdk_dotnet_updated", "password", "").ToStringExtension());

            // password - public
            Console.WriteLine(chino.applications.create("app_pswd_sdk_dotnet_public", "password", "", client_type: "public").ToStringExtension());
            appId = app.app_id;
            Console.WriteLine(chino.applications.read(appId).ToStringExtension());
            Console.WriteLine(chino.applications.update(appId, "app_pswd_sdk_dotnet_public_updated", "password").ToStringExtension());

            // authcode - confidential
            app = chino.applications.create("app_code_sdk_dotnet", "authorization-code", "https://example.com");
            appId = app.app_id;
            Console.WriteLine(chino.applications.read(appId).ToStringExtension());
            Console.WriteLine(chino.applications.update(appId, "app_code_sdk_dotnet_updated",  "authorization-code", "https://example.com").ToStringExtension());

            // authcode - public
            app = chino.applications.create("app_code_sdk_dotnet", "authorization-code", "https://example.com", "public");
            appId = app.app_id;
            Console.WriteLine(chino.applications.read(appId).ToStringExtension());
            Console.WriteLine(chino.applications.update(appId, "app_code_sdk_dotnet_updated",  "authorization-code", client_type:"public", redirect_url:"https://example.com").ToStringExtension());


            GetApplicationsResponse apps = chino.applications.list(0);
            foreach (ApplicationsObject a in apps.applications)
            {
                Console.WriteLine(chino.applications.delete(a.app_id, true));
            }
            
            appId = null;
        }

        [TestMethod]
        public void testSchemas()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            _repositoryId = repo.repository_id;
            SchemaRequest schemaRequest = new SchemaRequest();
            SchemaStructure schemaStructure = new SchemaStructure();
            List<SchemaField> fields = new List<SchemaField>();
            fields.Add(new SchemaField("test_integer", "integer"));
            fields.Add(new SchemaField("test_string", "string"));
            fields.Add(new SchemaField("test_boolean", "boolean"));
            schemaStructure.fields = fields;
            schemaRequest.structure = schemaStructure;
            schemaRequest.description = "schema_description_1";
            Schema schema = chino.schemas.create(_repositoryId, schemaRequest);
            _schemaId1 = schema.schema_id;
            Console.WriteLine(schema.ToStringExtension());
            schema = chino.schemas.create(_repositoryId, "schema_description_2", typeof(SchemaStructureSample));
            _schemaId2 = schema.schema_id;
            Console.WriteLine(schema.ToStringExtension());
            schema = chino.schemas.create(_repositoryId, "schema_description_3", schemaStructure);
            _schemaId3 = schema.schema_id;
            Console.WriteLine(schema.ToStringExtension());
            fields = new List<SchemaField>();
            fields.Add(new SchemaField("test_integer_updated", "integer"));
            fields.Add(new SchemaField("test_string_updated", "string"));
            fields.Add(new SchemaField("test_boolean_updated", "boolean"));
            schemaStructure.fields = fields;
            schemaRequest.structure = schemaStructure;
            schemaRequest.description = "schema_description_updated_1";
            Console.WriteLine(chino.schemas.update(_schemaId1, schemaRequest).ToStringExtension());
            Console.WriteLine(chino.schemas.update(_schemaId2, "test_schema_description_updated_2", typeof(SchemaStructureSampleUpdated)).ToStringExtension());
            schemaRequest.description = "schema_description_updated_3";
            Console.WriteLine(chino.schemas.update(_schemaId3, schemaRequest).ToStringExtension());
            Console.WriteLine(chino.schemas.delete(_schemaId1, true));
            Console.WriteLine(chino.schemas.delete(_schemaId2, true));
            Console.WriteLine(chino.schemas.delete(_schemaId3, true));
        }

        [TestMethod]
        public void testUserSchemas()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            GetUserSchemasResponse userschemas = chino.userSchemas.list(0);
            foreach (UserSchema u in userschemas.user_schemas)
            {
                Console.WriteLine(chino.userSchemas.delete(u.user_schema_id, true));
            }
            UserSchemaRequest userSchemaRequest = new UserSchemaRequest();
            UserSchemaStructure userSchemaStructure = new UserSchemaStructure();
            List<UserSchemaField> fields = new List<UserSchemaField>();
            fields.Add(new UserSchemaField("test_integer", "integer", true));
            fields.Add(new UserSchemaField("test_string", "string", true));
            fields.Add(new UserSchemaField("test_boolean", "boolean"));
            userSchemaStructure.fields = fields;
            userSchemaRequest.structure = userSchemaStructure;
            userSchemaRequest.description = "user_schema_description_1";
            UserSchema userSchema = chino.userSchemas.create(userSchemaRequest);
            _userSchemaId1 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            userSchema = chino.userSchemas.create("user_schema_description_2", typeof(UserSchemaStructureSample));
            _userSchemaId2 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            userSchema = chino.userSchemas.create("user_schema_description_3", userSchemaStructure);
            _userSchemaId3 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            fields = new List<UserSchemaField>();
            fields.Add(new UserSchemaField("test_integer_updated", "integer", true));
            fields.Add(new UserSchemaField("test_string_updated", "string", true));
            fields.Add(new UserSchemaField("test_boolean_updated", "boolean", true));
            userSchemaStructure.fields = fields;
            userSchemaRequest.structure = userSchemaStructure;
            userSchemaRequest.description = "user_schema_description_updated_1";
            Console.WriteLine(chino.userSchemas.update(_userSchemaId1, userSchemaRequest).ToStringExtension());
            Console.WriteLine(chino.userSchemas.update(_userSchemaId2, "user_schema_description_updated_2", typeof(UserSchemaStructureSampleUpdated)).ToStringExtension());
            userSchemaRequest.description = "user_schema_description_updated_3";
            Console.WriteLine(chino.userSchemas.update(_userSchemaId3, userSchemaRequest).ToStringExtension());
            Console.WriteLine(chino.userSchemas.delete(_userSchemaId1, true));
            Console.WriteLine(chino.userSchemas.delete(_userSchemaId2, true));
            Console.WriteLine(chino.userSchemas.delete(_userSchemaId3, true));
        }

        [TestMethod]
        public void testUsers()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            GetUserSchemasResponse userschemas = chino.userSchemas.list(0);
            foreach (UserSchema u in userschemas.user_schemas)
            {
                Console.WriteLine(chino.userSchemas.delete(u.user_schema_id, true));
            }
            UserSchema userSchema = chino.userSchemas.create("user_schema_description_2", typeof(UserSchemaStructureSample));
            _userSchemaId1 = userSchema.user_schema_id;
            Dictionary<String, Object> attributes = new Dictionary<string, object>
            {
                {"test_integer", 123},
                {"test_string", "string_value"},
                {"test_boolean", true},
                {"test_date", "1997-12-03"}
            };
            User user = chino.users.create("Giovanni", "password", attributes, _userSchemaId1);
            Console.WriteLine(user.ToStringExtension());
            _userId = user.user_id;
            Console.WriteLine(user.ToStringExtension());
            Console.WriteLine(user.attributes["test_string"]);
            attributes = new Dictionary<string, object>();
            attributes.Add("test_integer", 12345);
            attributes.Add("test_string", "string_value_updated");
            attributes.Add("test_boolean", false);
            attributes.Add("test_date", "1967-05-04");
            Console.WriteLine(chino.users.update("Giovanni", "password", attributes, _userId).ToStringExtension());
            attributes = new Dictionary<string, object>();
            attributes.Add("test_integer", 666);
            Console.WriteLine(chino.users.updateSomeFields(_userId, attributes).ToStringExtension());
            Application app = chino.applications.create("application_sdk_dotnet", "password", "");
            LoggedUser loggedUser = chino.auth.loginUserWithPassword("Giovanni", "password", app.app_id, app.app_secret);
            Console.WriteLine(loggedUser.ToStringExtension());
            chino.initClient(Const._hostUrl, loggedUser.access_token);
            Console.WriteLine(chino.auth.checkUserStatus().ToStringExtension());
            Console.WriteLine(chino.auth.logoutUser(loggedUser.access_token, app.app_id, app.app_secret));
            chino.initClient(Const._hostUrl, Const._customerId, Const._customerKey);
            //LoggedUser loggedUser = chino.auth.loginUser("Giovanni", "password", customerId);
            //When you have logged the user and you have the token you need to init the client passing the token in the function
            //chino.initClient(hostUrl, loggedUser.access_token);
            //Console.WriteLine(chino.auth.checkUserStatus().ToStringExtension());
            //Console.WriteLine(chino.auth.logoutUser().ToStringExtension());
            //chino.initClient(hostUrl, customerId, customerKey);
            //Console.WriteLine(chino.userSchemas.read(USER_SCHEMA_ID_1).ToStringExtension());
        }

        [TestMethod]
        public void testDocuments()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            _repositoryId = repo.repository_id;
            Schema schema = chino.schemas.create(_repositoryId, "schema_description_2", typeof(SchemaStructureSample));
            _schemaId1 = schema.schema_id;
            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, _schemaId1);
            _documentId = document.document_id;
            Console.WriteLine(document.ToStringExtension());
            Console.WriteLine(chino.documents.read(_documentId).ToStringExtension());
            content = new Dictionary<string, object>();
            content.Add("test_integer", 1234);
            content.Add("test_string", "string_value_updated");
            content.Add("test_boolean", false);
            content.Add("test_date", "1993-02-04");
            document = chino.documents.update(content, _documentId);
            Console.WriteLine(document.ToStringExtension());
            GetDocumentsResponse documentsResponse = chino.documents.listWithFullContent(_schemaId1, 0);
            Console.WriteLine(documentsResponse.ToStringExtension());
            foreach (Document d in documentsResponse.documents)
            {
                Console.WriteLine(chino.documents.delete(d.document_id, true));
            }
        }

        [TestMethod]
        public void testCollections()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            GetCollectionsResponse collections = chino.collections.list(0);
            foreach (Collection c in collections.collections)
            {
                Console.WriteLine(chino.collections.delete(c.collection_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            _repositoryId = repo.repository_id;
            Schema schema = chino.schemas.create(_repositoryId, "schema_description_2", typeof(SchemaStructureSample));
            _schemaId1 = schema.schema_id;
            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, _schemaId1);
            _documentId = document.document_id;
            content = new Dictionary<string, object>();
            content.Add("test_integer", 1234);
            content.Add("test_string", "string_value_2");
            content.Add("test_boolean", false);
            content.Add("test_date", "1993-02-04");
            document = chino.documents.create(content, _schemaId1);
            String DOCUMENT_ID_2 = document.document_id;
            Collection collection = chino.collections.create("collection_name");
            Console.WriteLine(collection.ToStringExtension());
            _collectionId = collection.collection_id;
            Console.WriteLine(chino.collections.addDocument(_collectionId, _documentId));
            Console.WriteLine(chino.collections.addDocument(_collectionId, DOCUMENT_ID_2));
            Console.WriteLine(chino.collections.listDocuments(_collectionId, 0).ToStringExtension());
            chino.collections.update(_collectionId, "collection_name_updated");
            Console.WriteLine(chino.collections.read(_collectionId).ToStringExtension());
            Console.WriteLine(chino.collections.list(0).ToStringExtension());
        }

        [TestMethod]
        public void testGroups()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            GetUserSchemasResponse userschemas = chino.userSchemas.list(0);
            foreach (UserSchema u in userschemas.user_schemas)
            {
                Console.WriteLine(chino.userSchemas.delete(u.user_schema_id, true));
            }
            GetGroupsResponse groups = chino.groups.list(0);
            foreach (Group g in groups.groups)
            {
                Console.WriteLine(chino.groups.delete(g.group_id, true));
            }
            UserSchema userSchema = chino.userSchemas.create("user_schema_description_2", typeof(UserSchemaStructureSample));
            _userSchemaId1 = userSchema.user_schema_id;
            Dictionary<String, Object> attributes = new Dictionary<string, object>();
            attributes.Add("test_integer", 123);
            attributes.Add("test_string", "string_value");
            attributes.Add("test_boolean", true);
            attributes.Add("test_date", "1997-12-03");
            User user = chino.users.create("Giovanni", "password", attributes, _userSchemaId1);
            Console.WriteLine(user.ToStringExtension());
            _userId = user.user_id;
            Console.WriteLine(user.ToStringExtension());
            attributes = new Dictionary<string,object>();
            attributes.Add("test_attribute_1", "test_value");
            attributes.Add("test_attribute_2", 123);
            Group group = chino.groups.create("test_group_name", attributes);
            _groupId = group.group_id;
            Console.WriteLine(group.ToStringExtension());
            attributes = new Dictionary<string, object>();
            attributes.Add("test_attribute_1", "test_value_updated");
            Console.WriteLine(chino.groups.update(_groupId, "test_group_name_updated", attributes).ToStringExtension());
            Console.WriteLine(chino.groups.addUserToGroup(_userId, _groupId));
            Console.WriteLine(chino.groups.addUserSchemaToGroup(_userSchemaId1, _groupId));
            Console.WriteLine(chino.groups.removeUserFromGroup(_userId, _groupId));
            Console.WriteLine(chino.groups.removeUserSchemaFromGroup(_userSchemaId1, _groupId));
        }

        [TestMethod]
        public void testSearchOld()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            Const.deleteAll(chino);
            Repository repo = chino.repositories.create("test_repo_description");
            _repositoryId = repo.repository_id;
            SchemaRequest schemaRequest = new SchemaRequest();
            schemaRequest.description = "schema_dotnet_sdk";
            SchemaStructure schemaStructure = new SchemaStructure();
            List<SchemaField> fieldList = new List<SchemaField>();
            fieldList.Add(new SchemaField("test_integer", "integer", true));
            fieldList.Add(new SchemaField("test_string", "string", true));
            fieldList.Add(new SchemaField("test_boolean", "boolean", true));
            fieldList.Add(new SchemaField("test_date", "date", true));
            fieldList.Add(new SchemaField("test_file", "blob"));
            schemaStructure.fields = fieldList;
            schemaRequest.structure = schemaStructure;
            Schema schema = chino.schemas.create(_repositoryId, schemaRequest);
            _schemaId1 = schema.schema_id;

            List<UserSchemaField> userSchemaFields = new List<UserSchemaField>();
            userSchemaFields.Add(new UserSchemaField("name", "string", true));
            userSchemaFields.Add(new UserSchemaField("last_name", "string", true));
            UserSchemaStructure userSchemaStructure = new UserSchemaStructure();
            userSchemaStructure.fields = userSchemaFields;
            UserSchemaRequest userSchemaRequest = new UserSchemaRequest();
            userSchemaRequest.structure = userSchemaStructure;
            userSchemaRequest.description = "user_schema";
            UserSchema userSchema = chino.userSchemas.create(userSchemaRequest);

            Dictionary<String, Object> attributes = new Dictionary<string, object>();
            attributes.Add("name", "Giacomino");
            attributes.Add("last_name", "Poretti");

            chino.users.create("jack@gmail.com", "password", attributes, userSchema.user_schema_id);

            Thread.Sleep(8000);

            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, _schemaId1);
            _documentId = document.document_id;
            Console.WriteLine(document.ToStringExtension());
            content = new Dictionary<string, object>();
            content.Add("test_integer", 1234);
            content.Add("test_string", "string_value_2");
            content.Add("test_boolean", false);
            content.Add("test_date", "1997-12-04");
            chino.documents.create(content, _schemaId1);

            Thread.Sleep(3000);

            Console.WriteLine(document.ToStringExtension());
            SearchRequest searchRequest = new SearchRequest();
            searchRequest.result_type = "ONLY_ID";
            searchRequest.filter_type = "and";
            List<SortOption> sort = new List<SortOption>();
            sort.Add(new SortOption("test_string", "asc"));
            searchRequest.sort = sort;
            List<FilterOption> filter = new List<FilterOption>();
            filter.Add(new FilterOption("test_integer", "gt", 123));
            searchRequest.filter = filter;
            Console.WriteLine("ONLY_ID");
            Console.WriteLine(chino.search.searchDocuments(_schemaId1, searchRequest).ToStringExtension());
            
            filter.Add(new FilterOption("test_boolean", "eq", true));
            Console.WriteLine(chino.search.searchDocuments(_schemaId1, "FULL_CONTENT", true, "or", sort, filter).ToStringExtension());
            
            GetDocumentsResponse documents = chino.search.where("test_integer").gt(123).and("test_date").eq("1997-12-04").sortAscBy("test_string").searchDocuments(_schemaId1);
            
            Console.WriteLine("Test search method with functions:");
            Console.WriteLine(documents.ToStringExtension());
            GetUsersResponse users = chino.search.where("name").eq("Giacobino").sortAscBy("name").resultType("EXISTS").searchUsers(userSchema.user_schema_id);
            Console.WriteLine(users.ToStringExtension());
            
            users = chino.search.where("username").eq("jack@gmail.com").sortAscBy("name").resultType("USERNAME_EXISTS").searchUsers(userSchema.user_schema_id);
            Console.WriteLine(users.ToStringExtension());
        }

        [TestMethod]
        public void testPermissions()
        {
            // SETUP
            
            Console.WriteLine("Test setup");
            
            ChinoAPI chinoAdmin = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            Const.deleteAll(chinoAdmin);
            ChinoAPI chino = new ChinoAPI(Const._hostUrl);

            Repository repo = chinoAdmin.repositories.create("test_repo_description");
            _repositoryId = repo.repository_id;
            
            Schema schema = chinoAdmin.schemas.create(_repositoryId, "schema_description_2", typeof(SchemaStructureSample));
            _schemaId1 = schema.schema_id;
            
            UserSchema userSchema = chinoAdmin.userSchemas.create("user_schema_description_2", typeof(UserSchemaStructureSample));
            _userSchemaId1 = userSchema.user_schema_id;
            
            Application app = chinoAdmin.applications.create("app_sdk_dotnet", "password", "");
            
            Dictionary<String, Object> attributes = new Dictionary<string, object>();
            attributes.Add("test_integer", 123);
            attributes.Add("test_string", "string_value");
            attributes.Add("test_boolean", true);
            attributes.Add("test_date", "1997-12-03");
            User user = chinoAdmin.users.create("Giovanni", "password", attributes, _userSchemaId1);
            _userId = user.user_id;
            
            Console.WriteLine();
            Console.WriteLine("Test started");
            
            // TEST perms on a Schema and new Documents
            PermissionRule rule = new PermissionRule();
            rule.setAuthorize(PermissionValues.READ);
            rule.setManage(PermissionValues.READ, PermissionValues.UPDATE, PermissionValues.DELETE);
            Console.WriteLine(chinoAdmin.permissions.permissionsOnaResource(PermissionValues.GRANT, PermissionValues.REPOSITORIES, _repositoryId, PermissionValues.USERS, _userId, rule));
            
            PermissionRuleCreatedDocument permissionRuleCreatedDocument = new PermissionRuleCreatedDocument();
            permissionRuleCreatedDocument.setAuthorize("R", "C", "U");
            permissionRuleCreatedDocument.setManage("R", "C", "U", "D");
            rule = new PermissionRule();
            rule.setAuthorize("R", "U");
            rule.setManage("R", "U", "D");
            permissionRuleCreatedDocument.created_document = rule;
            Console.WriteLine(chinoAdmin.permissions.permissionsOnResourceChildren(PermissionValues.GRANT, PermissionValues.SCHEMAS, _schemaId1, PermissionValues.DOCUMENTS, PermissionValues.USERS, _userId, permissionRuleCreatedDocument));
            
            LoggedUser loggedUser = chino.auth.loginUserWithPassword("Giovanni", "password", app.app_id, app.app_secret);
            chino.initClient(Const._hostUrl, loggedUser.access_token);
            
            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chinoAdmin.documents.create(content, _schemaId1);
            _documentId = document.document_id;
            
            chino.auth.checkUserStatus();
            GetPermissionsResponse permissionsResponse = chinoAdmin.permissions.readPermissionsOfaUser(_userId, 0);

            Console.WriteLine("[Admin] Permissions of the User:\n" + "{");
            Console.WriteLine(permissionsResponse.ToStringExtension());
            Console.WriteLine("}\n");
            
            Console.WriteLine("[USER] Permissions of the User:\n" + "{");
            Console.WriteLine(chino.permissions.readPermissions(0).ToStringExtension());
            Console.WriteLine("}\n");
            
            chino.auth.logoutUser(loggedUser.access_token, app.app_id, app.app_secret);
            
            chino.initClient(Const._hostUrl, Const._customerId, Const._customerKey);
            attributes = new Dictionary<string, object>();
            attributes.Add("test_attribute_1", "test_value");
            attributes.Add("test_attribute_2", 123);
            
            Group group = chinoAdmin.groups.create("test_group_name", attributes);
            _groupId = group.group_id;
            
            rule = new PermissionRule();
            rule.setAuthorize(PermissionValues.READ, PermissionValues.UPDATE);
            rule.setManage(PermissionValues.READ, PermissionValues.UPDATE, PermissionValues.CREATE);
            chinoAdmin.permissions.permissionsOnResources(PermissionValues.GRANT, PermissionValues.REPOSITORIES, PermissionValues.GROUPS, _groupId, rule);
            chinoAdmin.permissions.permissionsOnResourceChildren(PermissionValues.GRANT, PermissionValues.REPOSITORIES, _repositoryId, PermissionValues.SCHEMAS, PermissionValues.GROUPS, _groupId, rule);
            
            Console.WriteLine("[ADMIN] Permissions of the Group:\n" + "{");
            Console.WriteLine(chino.permissions.readPermissionsOfaGroup(_groupId, 0).ToStringExtension());
            Console.WriteLine("}\n");
        }

        [TestMethod]
        public void testBlobs()
        {
            ChinoAPI chino = new ChinoAPI(Const._hostUrl, Const._customerId, Const._customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            _repositoryId = repo.repository_id;
            Schema schema = chino.schemas.create(_repositoryId, "schema_description_2", typeof(SchemaStructureSample));
            _schemaId1 = schema.schema_id;
            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, _schemaId1);
            _documentId = document.document_id;
            //The file to upload is located in ChinoTest/bin/Debug/attachments
            CommitBlobUploadResponse commitBlobUploadResponse = chino.blobs.uploadBlob(Const.Path, _documentId, "test_file", Const.FileName);
            Console.WriteLine(commitBlobUploadResponse.ToStringExtension());
            GetBlobResponse blobResponse = chino.blobs.get(commitBlobUploadResponse.blob.blob_id, Const.Destination);
            Console.WriteLine(blobResponse.ToStringExtension());
            Console.WriteLine(chino.blobs.delete(commitBlobUploadResponse.blob.blob_id, true));
        }

        [TestMethod]
        [ExpectedException(typeof(ChinoApiException),
            "Expected exception: ChinoApiException")]
        public void testException()
        {
            var chino = new ChinoAPI(Const._hostUrl, "Invalid-ID", "Invalid-Key");
            
            // should raise ChinoApiException, 401
            chino.repositories.list(0);
        }
    }

    public class SchemaStructureSample{
        public int test_integer;
        public String test_string;
        public Boolean test_boolean;
        public DateTime test_date;
        public FileStream test_file;
    }

    public class UserSchemaStructureSample
    {
        public int test_integer;
        public String test_string;
        public Boolean test_boolean;
        public DateTime test_date;
    }

    public class UserSchemaStructureSampleUpdated
    {
        public TimeSpan test_time;
        public int test_integer;
    }

    public class SchemaStructureSampleUpdated
    {
        public TimeSpan test_time;
        public int test_integer;
    }

}