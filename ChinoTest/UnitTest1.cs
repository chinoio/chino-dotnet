﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chino;
using System.Collections.Generic;

namespace ChinoTest
{
    [TestClass]
    public class RepositoriesTest
    {
        String USER_SCHEMA_ID_1 = "";
        String USER_SCHEMA_ID_2 = "";
        String USER_SCHEMA_ID_3 = "";
        String SCHEMA_ID_1 = "";
        String SCHEMA_ID_2 = "";
        String SCHEMA_ID_3 = "";
        String USER_ID = "";
        String REPOSITORY_ID = "";
        String customerId = "354e3d83-5cb4-461a-b0f2-fc135c8d1a9c";
        String customerKey = "5e44d79a-dd96-448d-b3d2-78ed76cc6548";
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
            fields.Add(new UserSchemaField("test_integer", "integer"));
            fields.Add(new UserSchemaField("test_string", "string"));
            fields.Add(new UserSchemaField("test_boolean", "boolean"));
            userSchemaStructure.fields = fields;
            userSchemaRequest.structure = userSchemaStructure;
            userSchemaRequest.description = "user_schema_description_1";
            UserSchema userSchema = chino.userSchemas.create(userSchemaRequest);
            USER_SCHEMA_ID_1 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            userSchema = chino.userSchemas.create("user_schema_description_2", typeof(SchemaStructureSample));
            USER_SCHEMA_ID_2 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            userSchema = chino.userSchemas.create("user_schema_description_3", userSchemaStructure);
            USER_SCHEMA_ID_3 = userSchema.user_schema_id;
            Console.WriteLine(userSchema.ToStringExtension());
            fields = new List<UserSchemaField>();
            fields.Add(new UserSchemaField("test_integer_updated", "integer"));
            fields.Add(new UserSchemaField("test_string_updated", "string"));
            fields.Add(new UserSchemaField("test_boolean_updated", "boolean"));
            userSchemaStructure.fields = fields;
            userSchemaRequest.structure = userSchemaStructure;
            userSchemaRequest.description = "user_schema_description_updated_1";
            Console.WriteLine(chino.userSchemas.update(USER_SCHEMA_ID_1, userSchemaRequest).ToStringExtension());
            Console.WriteLine(chino.userSchemas.update(USER_SCHEMA_ID_2, "user_schema_description_updated_2", typeof(SchemaStructureSampleUpdated)).ToStringExtension());
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
            UserSchema userSchema = chino.userSchemas.create("user_schema_description_2", typeof(SchemaStructureSample));
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
        }
    }

    public class SchemaStructureSample{
        public int test_integer;
        public String test_string;
        public Boolean test_boolean;
        public DateTime test_date;
    }

    public class SchemaStructureSampleUpdated
    {
        public TimeSpan test_time;
        public int test_integer;
    }

}