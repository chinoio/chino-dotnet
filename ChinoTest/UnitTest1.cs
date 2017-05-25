using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chino;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ChinoTest
{
    [TestClass]
    public class RepositoriesTest
    {
        int chunkSize=100*1024;
        String FILE_NAME = "003.pdf";
        String FILE_NAME_2 = "0008mb.pdf";
        String PATH = "attachments";
        String DESTINATION = "attachments/temp";
        String APP_ID = "";
        String USER_SCHEMA_ID_1 = "";
        String USER_SCHEMA_ID_2 = "";
        String USER_SCHEMA_ID_3 = "";
        String SCHEMA_ID_1 = "";
        String SCHEMA_ID_2 = "";
        String SCHEMA_ID_3 = "";
        String USER_ID = "";
        String DOCUMENT_ID = "";
        String COLLECTION_ID = "";
        String REPOSITORY_ID = "";
        String GROUP_ID = "";
        String customerId = "<your-customer-id>";
        String customerKey = "<your-customer-key>";
        String hostUrl = "https://api.test.chino.io/v1";

        [TestMethod]
        public void TestRepositories()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            REPOSITORY_ID = repo.repository_id;
            Console.WriteLine(chino.repositories.read(REPOSITORY_ID).ToStringExtension());
            Console.WriteLine(chino.repositories.update(REPOSITORY_ID, "test_repo_description_updated").ToStringExtension());
            Console.WriteLine(chino.repositories.create("test_repo_description_2").ToStringExtension());
            Console.WriteLine(chino.repositories.list(0).ToStringExtension());
            Console.WriteLine(chino.repositories.delete(REPOSITORY_ID, true));
        }

        [TestMethod]
        public void TestApplications()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
            Application app = chino.applications.create("application_sdk_dotnet", "password", "");
            APP_ID = app.app_id;
            Console.WriteLine(chino.applications.read(APP_ID).ToStringExtension());
            Console.WriteLine(chino.applications.update(APP_ID, "application_sdk_dotnet_updated", "password", "").ToStringExtension());
            Console.WriteLine(chino.applications.create("application_sdk_dotnet_2", "password", "").ToStringExtension());
            GetApplicationsResponse apps = chino.applications.list(0);
            foreach (ApplicationsObject a in apps.applications)
            {
                Console.WriteLine(chino.applications.delete(a.app_id, true));
            }
        }

        [TestMethod]
        public void TestSchemas()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            REPOSITORY_ID = repo.repository_id;
            SchemaRequest schemaRequest = new SchemaRequest();
            SchemaStructure schemaStructure = new SchemaStructure();
            List<SchemaField> fields = new List<SchemaField>();
            fields.Add(new SchemaField("test_integer", "integer"));
            fields.Add(new SchemaField("test_string", "string"));
            fields.Add(new SchemaField("test_boolean", "boolean"));
            schemaStructure.fields = fields;
            schemaRequest.structure = schemaStructure;
            schemaRequest.description = "schema_description_1";
            Schema schema = chino.schemas.create(REPOSITORY_ID, schemaRequest);
            SCHEMA_ID_1 = schema.schema_id;
            Console.WriteLine(schema.ToStringExtension());
            schema = chino.schemas.create(REPOSITORY_ID, "schema_description_2", typeof(SchemaStructureSample));
            SCHEMA_ID_2 = schema.schema_id;
            Console.WriteLine(schema.ToStringExtension());
            schema = chino.schemas.create(REPOSITORY_ID, "schema_description_3", schemaStructure);
            SCHEMA_ID_3 = schema.schema_id;
            Console.WriteLine(schema.ToStringExtension());
            fields = new List<SchemaField>();
            fields.Add(new SchemaField("test_integer_updated", "integer"));
            fields.Add(new SchemaField("test_string_updated", "string"));
            fields.Add(new SchemaField("test_boolean_updated", "boolean"));
            schemaStructure.fields = fields;
            schemaRequest.structure = schemaStructure;
            schemaRequest.description = "schema_description_updated_1";
            Console.WriteLine(chino.schemas.update(SCHEMA_ID_1, schemaRequest).ToStringExtension());
            Console.WriteLine(chino.schemas.update(SCHEMA_ID_2, "test_schema_description_updated_2", typeof(SchemaStructureSampleUpdated)).ToStringExtension());
            schemaRequest.description = "schema_description_updated_3";
            Console.WriteLine(chino.schemas.update(SCHEMA_ID_3, schemaRequest).ToStringExtension());
            Console.WriteLine(chino.schemas.delete(SCHEMA_ID_1, true));
            Console.WriteLine(chino.schemas.delete(SCHEMA_ID_2, true));
            Console.WriteLine(chino.schemas.delete(SCHEMA_ID_3, true));
        }

        [TestMethod]
        public void TestUserSchemas()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
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
            USER_SCHEMA_ID_1 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            userSchema = chino.userSchemas.create("user_schema_description_2", typeof(UserSchemaStructureSample));
            USER_SCHEMA_ID_2 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            userSchema = chino.userSchemas.create("user_schema_description_3", userSchemaStructure);
            USER_SCHEMA_ID_3 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            fields = new List<UserSchemaField>();
            fields.Add(new UserSchemaField("test_integer_updated", "integer", true));
            fields.Add(new UserSchemaField("test_string_updated", "string", true));
            fields.Add(new UserSchemaField("test_boolean_updated", "boolean", true));
            userSchemaStructure.fields = fields;
            userSchemaRequest.structure = userSchemaStructure;
            userSchemaRequest.description = "user_schema_description_updated_1";
            Console.WriteLine(chino.userSchemas.update(USER_SCHEMA_ID_1, userSchemaRequest).ToStringExtension());
            Console.WriteLine(chino.userSchemas.update(USER_SCHEMA_ID_2, "user_schema_description_updated_2", typeof(UserSchemaStructureSampleUpdated)).ToStringExtension());
            userSchemaRequest.description = "user_schema_description_updated_3";
            Console.WriteLine(chino.userSchemas.update(USER_SCHEMA_ID_3, userSchemaRequest).ToStringExtension());
            Console.WriteLine(chino.userSchemas.delete(USER_SCHEMA_ID_1, true));
            Console.WriteLine(chino.userSchemas.delete(USER_SCHEMA_ID_2, true));
            Console.WriteLine(chino.userSchemas.delete(USER_SCHEMA_ID_3, true));
        }

        [TestMethod]
        public void TestUsers()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
            GetUserSchemasResponse userschemas = chino.userSchemas.list(0);
            foreach (UserSchema u in userschemas.user_schemas)
            {
                Console.WriteLine(chino.userSchemas.delete(u.user_schema_id, true));
            }
            UserSchema userSchema = chino.userSchemas.create("user_schema_description_2", typeof(UserSchemaStructureSample));
            USER_SCHEMA_ID_1 = userSchema.user_schema_id;
            Dictionary<String, Object> attributes = new Dictionary<string,object>();
            attributes.Add("test_integer", 123);
            attributes.Add("test_string", "string_value");
            attributes.Add("test_boolean", true);
            attributes.Add("test_date", "1997-12-03");
            User user = chino.users.create("Giovanni", "password", attributes, USER_SCHEMA_ID_1);
            Console.WriteLine(user.ToStringExtension());
            USER_ID = user.user_id;
            Console.WriteLine(user.ToStringExtension());
            Console.WriteLine(user.attributes["test_string"]);
            attributes = new Dictionary<string, object>();
            attributes.Add("test_integer", 12345);
            attributes.Add("test_string", "string_value_updated");
            attributes.Add("test_boolean", false);
            attributes.Add("test_date", "1967-05-04");
            Console.WriteLine(chino.users.update("Giovanni", "password", attributes, USER_ID).ToStringExtension());
            attributes = new Dictionary<string, object>();
            attributes.Add("test_integer", 666);
            Console.WriteLine(chino.users.updateSomeFields(USER_ID, attributes).ToStringExtension());
            Application app = chino.applications.create("application_sdk_dotnet", "password", "");
            LoggedUser loggedUser = chino.auth.loginUserWithPassword("Giovanni", "password", app.app_id, app.app_secret);
            Console.WriteLine(loggedUser.ToStringExtension());
            chino.initClient(hostUrl, loggedUser.access_token);
            Console.WriteLine(chino.auth.checkUserStatus().ToStringExtension());
            Console.WriteLine(chino.auth.logoutUser(loggedUser.access_token, app.app_id, app.app_secret));
            chino.initClient(hostUrl, customerId, customerKey);
            //LoggedUser loggedUser = chino.auth.loginUser("Giovanni", "password", customerId);
            //When you have logged the user and you have the token you need to init the client passing the token in the function
            //chino.initClient(hostUrl, loggedUser.access_token);
            //Console.WriteLine(chino.auth.checkUserStatus().ToStringExtension());
            //Console.WriteLine(chino.auth.logoutUser().ToStringExtension());
            //chino.initClient(hostUrl, customerId, customerKey);
            //Console.WriteLine(chino.userSchemas.read(USER_SCHEMA_ID_1).ToStringExtension());
        }

        [TestMethod]
        public void TestDocuments()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            REPOSITORY_ID = repo.repository_id;
            Schema schema = chino.schemas.create(REPOSITORY_ID, "schema_description_2", typeof(SchemaStructureSample));
            SCHEMA_ID_1 = schema.schema_id;
            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, SCHEMA_ID_1);
            DOCUMENT_ID = document.document_id;
            Console.WriteLine(document.ToStringExtension());
            Console.WriteLine(chino.documents.read(DOCUMENT_ID).ToStringExtension());
            content = new Dictionary<string, object>();
            content.Add("test_integer", 1234);
            content.Add("test_string", "string_value_updated");
            content.Add("test_boolean", false);
            content.Add("test_date", "1993-02-04");
            document = chino.documents.update(content, DOCUMENT_ID);
            Console.WriteLine(document.ToStringExtension());
            GetDocumentsResponse documentsResponse = chino.documents.listWithFullContent(SCHEMA_ID_1, 0);
            Console.WriteLine(documentsResponse.ToStringExtension());
            foreach (Document d in documentsResponse.documents)
            {
                Console.WriteLine(chino.documents.delete(d.document_id, true));
            }
        }

        [TestMethod]
        public void TestCollections()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
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
            REPOSITORY_ID = repo.repository_id;
            Schema schema = chino.schemas.create(REPOSITORY_ID, "schema_description_2", typeof(SchemaStructureSample));
            SCHEMA_ID_1 = schema.schema_id;
            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, SCHEMA_ID_1);
            DOCUMENT_ID = document.document_id;
            content = new Dictionary<string, object>();
            content.Add("test_integer", 1234);
            content.Add("test_string", "string_value_2");
            content.Add("test_boolean", false);
            content.Add("test_date", "1993-02-04");
            document = chino.documents.create(content, SCHEMA_ID_1);
            String DOCUMENT_ID_2 = document.document_id;
            Collection collection = chino.collections.create("collection_name");
            Console.WriteLine(collection.ToStringExtension());
            COLLECTION_ID = collection.collection_id;
            Console.WriteLine(chino.collections.addDocument(COLLECTION_ID, DOCUMENT_ID));
            Console.WriteLine(chino.collections.addDocument(COLLECTION_ID, DOCUMENT_ID_2));
            Console.WriteLine(chino.collections.listDocuments(COLLECTION_ID, 0).ToStringExtension());
            chino.collections.update(COLLECTION_ID, "collection_name_updated");
            Console.WriteLine(chino.collections.read(COLLECTION_ID).ToStringExtension());
            Console.WriteLine(chino.collections.list(0).ToStringExtension());
        }

        [TestMethod]
        public void TestGroups()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
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
            USER_SCHEMA_ID_1 = userSchema.user_schema_id;
            Dictionary<String, Object> attributes = new Dictionary<string, object>();
            attributes.Add("test_integer", 123);
            attributes.Add("test_string", "string_value");
            attributes.Add("test_boolean", true);
            attributes.Add("test_date", "1997-12-03");
            User user = chino.users.create("Giovanni", "password", attributes, USER_SCHEMA_ID_1);
            Console.WriteLine(user.ToStringExtension());
            USER_ID = user.user_id;
            Console.WriteLine(user.ToStringExtension());
            attributes = new Dictionary<string,object>();
            attributes.Add("test_attribute_1", "test_value");
            attributes.Add("test_attribute_2", 123);
            Group group = chino.groups.create("test_group_name", attributes);
            GROUP_ID = group.group_id;
            Console.WriteLine(group.ToStringExtension());
            attributes = new Dictionary<string, object>();
            attributes.Add("test_attribute_1", "test_value_updated");
            Console.WriteLine(chino.groups.update(GROUP_ID, "test_group_name_updated", attributes).ToStringExtension());
            Console.WriteLine(chino.groups.addUserToGroup(USER_ID, GROUP_ID));
            Console.WriteLine(chino.groups.addUserSchemaToGroup(USER_SCHEMA_ID_1, GROUP_ID));
            Console.WriteLine(chino.groups.removeUserFromGroup(USER_ID, GROUP_ID));
            Console.WriteLine(chino.groups.removeUserSchemaFromGroup(USER_SCHEMA_ID_1, GROUP_ID));
        }

        [TestMethod]
        public void TestSearch()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
            deleteAll(chino);
            Repository repo = chino.repositories.create("test_repo_description");
            REPOSITORY_ID = repo.repository_id;
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
            Schema schema = chino.schemas.create(REPOSITORY_ID, schemaRequest);
            SCHEMA_ID_1 = schema.schema_id;

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
            attributes.Add("name", "Giacobino");
            attributes.Add("last_name", "Poretti");

            chino.users.create("jack@gmail.com", "password", attributes, userSchema.user_schema_id);

            Thread.Sleep(8000);

            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, SCHEMA_ID_1);
            DOCUMENT_ID = document.document_id;
            Console.WriteLine(document.ToStringExtension());
            content = new Dictionary<string, object>();
            content.Add("test_integer", 1234);
            content.Add("test_string", "string_value_2");
            content.Add("test_boolean", false);
            content.Add("test_date", "1997-12-04");
            chino.documents.create(content, SCHEMA_ID_1);

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
            Console.WriteLine(chino.search.searchDocuments(SCHEMA_ID_1, searchRequest).ToStringExtension());
            filter.Add(new FilterOption("test_boolean", "eq", true));
            Console.WriteLine(chino.search.searchDocuments(SCHEMA_ID_1, "FULL_CONTENT", true, "or", sort, filter).ToStringExtension());
            GetDocumentsResponse documents = chino.search.where("test_integer").gt(123).and("test_date").eq("1997-12-04").sortAscBy("test_string").searchDocuments(SCHEMA_ID_1);
            Console.WriteLine("Test search method with functions:");
            Console.WriteLine(documents.ToStringExtension());
            GetUsersResponse users = chino.search.where("name").eq("Giacobino").sortAscBy("name").resultType("EXISTS").searchUsers(userSchema.user_schema_id);
            Console.WriteLine(users.ToStringExtension());
            users = chino.search.where("username").eq("jack@gmail.com").sortAscBy("name").resultType("USERNAME_EXISTS").searchUsers(userSchema.user_schema_id);
            Console.WriteLine(users.ToStringExtension());
        }

        [TestMethod]
        public void TestPermissions()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
            deleteAll(chino);
            Repository repo = chino.repositories.create("test_repo_description");
            REPOSITORY_ID = repo.repository_id;
            Schema schema = chino.schemas.create(REPOSITORY_ID, "schema_description_2", typeof(SchemaStructureSample));
            SCHEMA_ID_1 = schema.schema_id;
            UserSchema userSchema = chino.userSchemas.create("user_schema_description_2", typeof(UserSchemaStructureSample));
            USER_SCHEMA_ID_1 = userSchema.user_schema_id;
            Dictionary<String, Object> attributes = new Dictionary<string, object>();
            attributes.Add("test_integer", 123);
            attributes.Add("test_string", "string_value");
            attributes.Add("test_boolean", true);
            attributes.Add("test_date", "1997-12-03");
            User user = chino.users.create("Giovanni", "password", attributes, USER_SCHEMA_ID_1);
            USER_ID = user.user_id;
            PermissionRule rule = new PermissionRule();
            rule.setAuthorize(PermissionValues.READ);
            rule.setManage(PermissionValues.READ, PermissionValues.UPDATE, PermissionValues.DELETE);
            Console.WriteLine(chino.permissions.permissionsOnaResource(PermissionValues.GRANT, PermissionValues.REPOSITORIES, REPOSITORY_ID, PermissionValues.USERS, USER_ID, rule));
            PermissionRuleCreatedDocument permissionRuleCreatedDocument = new PermissionRuleCreatedDocument();
            permissionRuleCreatedDocument.setAuthorize("R", "C", "U");
            permissionRuleCreatedDocument.setManage("R", "C", "U", "D");
            rule = new PermissionRule();
            rule.setAuthorize("R", "U");
            rule.setManage("R", "U", "D");
            permissionRuleCreatedDocument.created_document = rule;
            Console.WriteLine(chino.permissions.permissionsOnResourceChildren(PermissionValues.GRANT, PermissionValues.SCHEMAS, SCHEMA_ID_1, PermissionValues.DOCUMENTS, PermissionValues.USERS, USER_ID, permissionRuleCreatedDocument));
            Application app = chino.applications.create("app_sdk_dotnet", "password", "");
            LoggedUser loggedUser = chino.auth.loginUserWithPassword("Giovanni", "password", app.app_id, app.app_secret);
            chino.initClient(hostUrl, loggedUser.access_token);
            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, SCHEMA_ID_1);
            DOCUMENT_ID = document.document_id;
            chino.auth.checkUserStatus();
            Console.WriteLine("Permissions of the User:");
            GetPermissionsResponse permissionsResponse = chino.permissions.readPermissionsOfaUser(USER_ID, 0);
            Console.WriteLine(permissionsResponse.ToStringExtension());
            //This is the way to access the permissions
            foreach (Permission p in permissionsResponse.permissions)
            {
                List<String> authorize = p.getAuthorize();
                List<String> manage = p.getManage();
                List<String> authorizeCreatedDocument = p.getAuthorizeCreatedDocument();
                List<String> manageCreatedDocument = p.getManageCreatedDocument();
                Console.WriteLine("Authorize: " + string.Join(",", authorize.ToArray()));
                Console.WriteLine("Manage: " + string.Join(",", manage.ToArray()));
                //Console.WriteLine("Authorize Created Document: " + string.Join(",", authorizeCreatedDocument.ToArray()));
                //Console.WriteLine("Manage Created Document: " + string.Join(",", manageCreatedDocument.ToArray()));
                Console.WriteLine("");
            }
            Console.WriteLine("Permissions on the Document:");
            permissionsResponse = chino.permissions.readPermissionsOnaDocument(DOCUMENT_ID, 0);
            Console.WriteLine(permissionsResponse.ToStringExtension());
            Console.WriteLine("All Permissions:");
            Console.WriteLine(chino.permissions.readPermissions(0).ToStringExtension());
            chino.auth.logoutUser(loggedUser.access_token, app.app_id, app.app_secret);
            chino.initClient(hostUrl, customerId, customerKey);
            attributes = new Dictionary<string, object>();
            attributes.Add("test_attribute_1", "test_value");
            attributes.Add("test_attribute_2", 123);
            Group group = chino.groups.create("test_group_name", attributes);
            GROUP_ID = group.group_id;
            rule = new PermissionRule();
            rule.setAuthorize(PermissionValues.READ, PermissionValues.UPDATE);
            rule.setManage(PermissionValues.READ, PermissionValues.UPDATE, PermissionValues.CREATE);
            chino.permissions.permissionsOnResources(PermissionValues.GRANT, PermissionValues.REPOSITORIES, PermissionValues.GROUPS, GROUP_ID, rule);
            chino.permissions.permissionsOnResourceChildren(PermissionValues.GRANT, PermissionValues.REPOSITORIES, REPOSITORY_ID, PermissionValues.SCHEMAS, PermissionValues.GROUPS, GROUP_ID, rule);
            Console.WriteLine("Permissions of the Group:");
            Console.WriteLine(chino.permissions.readPermissionsOfaGroup(GROUP_ID, 0).ToStringExtension());
        }

        [TestMethod]
        public void TestBlobs()
        {
            ChinoAPI chino = new ChinoAPI(hostUrl, customerId, customerKey);
            GetRepositoriesResponse repos = chino.repositories.list(0);
            foreach (Repository r in repos.repositories)
            {
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            Repository repo = chino.repositories.create("test_repo_description");
            REPOSITORY_ID = repo.repository_id;
            Schema schema = chino.schemas.create(REPOSITORY_ID, "schema_description_2", typeof(SchemaStructureSample));
            SCHEMA_ID_1 = schema.schema_id;
            Dictionary<String, Object> content = new Dictionary<string, object>();
            content.Add("test_integer", 123);
            content.Add("test_string", "string_value");
            content.Add("test_boolean", true);
            content.Add("test_date", "1997-12-03");
            Document document = chino.documents.create(content, SCHEMA_ID_1);
            DOCUMENT_ID = document.document_id;
            //The file to upload is located in ChinoTest/bin/Debug/attachments
            CommitBlobUploadResponse commitBlobUploadResponse = chino.blobs.uploadBlob(PATH, DOCUMENT_ID, "test_file", FILE_NAME);
            Console.WriteLine(commitBlobUploadResponse.ToStringExtension());
            GetBlobResponse blobResponse = chino.blobs.get(commitBlobUploadResponse.blob.blob_id, DESTINATION);
            Console.WriteLine(blobResponse.ToStringExtension());
            Console.WriteLine(chino.blobs.delete(commitBlobUploadResponse.blob.blob_id, true));
        }

        public void deleteAll(ChinoAPI chino)
        {
            List<Repository> repos = chino.repositories.list(0).repositories;
            foreach (Repository r in repos) {
                List<Schema> schemas = chino.schemas.list(r.repository_id, 0).schemas;
                foreach (Schema s in schemas)
                {
                    List<Document> documents = chino.documents.list(s.schema_id, 0).documents;
                    foreach (Document d in documents)
                    {
                        Console.WriteLine(chino.documents.delete(d.document_id, true));
                    }
                    Console.WriteLine(chino.schemas.delete(s.schema_id, true));
                }
                Console.WriteLine(chino.repositories.delete(r.repository_id, true));
            }
            List<UserSchema> userSchemas = chino.userSchemas.list(0).user_schemas;
            foreach (UserSchema u in userSchemas)
            {
                List<User> users = chino.users.list(u.user_schema_id, 0).users;
                foreach (User us in users)
                {
                    Console.WriteLine(chino.users.delete(us.user_id, true));
                }
                Console.WriteLine(chino.userSchemas.delete(u.user_schema_id, true));
            }
            List<Collection> collections = chino.collections.list(0).collections;
            foreach (Collection c in collections)
            {
                Console.WriteLine(chino.collections.delete(c.collection_id, true));
            }
            List<Group> groups = chino.groups.list(0).groups;
            foreach (Group g in groups)
            {
                Console.WriteLine(chino.groups.delete(g.group_id, true));
            }
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