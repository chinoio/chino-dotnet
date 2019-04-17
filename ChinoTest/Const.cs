using System;
using System.Collections.Generic;
using Chino;

namespace ChinoTest {
    internal class Const
    {
        public const string FileName = "003.pdf";
        public const string Path = "Resources/attachments";
        public const string Destination = "Resources/attachments/temp";
        public static string _customerId;
        public static string _customerKey;
        public static string _hostUrl;
        public static bool TestAllowed = false;

        internal static bool isInitialized()
        {
            return _customerId != null && _customerKey != null && _hostUrl != null;
        }

        public static void deleteAll(ChinoAPI chino)
        {
            if (!TestAllowed)
            {
                Console.Error.WriteLine();
                Console.Error.WriteLine("WARNING: running tests will delete everything on the Chino.io account! " +
                                        "If you still want to run the tests, set in your environment variables 'automated_test=allow' and re-run the suite." +
                                        "\n");
                throw new ApplicationException(
                    "WARNING: running tests will delete everything on the Chino.io account! " +
                    "If you still want to run the tests, set in your environment variables 'automated_test=allow' and re-run the suite." +
                    "\n"
                );
            }
            // delete Documents / Schemas / Repos
            List<Repository> repos = chino.repositories.list(0).repositories;
            var repos_offset = 0;
            while (repos.Count > 0)
            {
                foreach (Repository r in repos) {
                    List<Schema> schemas = chino.schemas.list(r.repository_id, 0).schemas;
                    var schemas_offset = 0;
                    while (schemas.Count > 0)
                    {
                        foreach (Schema s in schemas)
                        {
                            List<Document> documents = chino.documents.list(s.schema_id, 0).documents;
                            var docs_offset = 0;
                            while (documents.Count > 0)
                            {
                                foreach (Document d in documents)
                                {
                                    Console.WriteLine(chino.documents.delete(d.document_id, true));
                                }
                                docs_offset += documents.Count;
                                documents = chino.documents.list(s.schema_id, docs_offset).documents;
                            }
                            Console.WriteLine(chino.schemas.delete(s.schema_id, true));
                        }
                        schemas_offset += schemas.Count;
                        schemas = chino.schemas.list(r.repository_id, schemas_offset).schemas;
                    }
                    Console.WriteLine(chino.repositories.delete(r.repository_id, true));
                }
                repos_offset += repos.Count;
                repos = chino.repositories.list(repos_offset).repositories;
            }
            // delete UserSchemas (and Users by consequence)
            List<UserSchema> userSchemas = chino.userSchemas.list(0).user_schemas;
            var userSchemas_offset = 0;
            while (userSchemas.Count > 0)
            {
                foreach (UserSchema u in userSchemas)
                {
                    Console.WriteLine(chino.userSchemas.delete(u.user_schema_id, true));
                }
                userSchemas_offset += userSchemas.Count;
                userSchemas = chino.userSchemas.list(userSchemas_offset).user_schemas;
            }
            // delete Collections
            List<Collection> collections = chino.collections.list(0).collections;
            var coll_offset = 0;
            while (collections.Count > 0)
            {
                foreach (Collection c in collections)
                {
                    Console.WriteLine(chino.collections.delete(c.collection_id, true));
                }
                coll_offset += collections.Count;
                collections = chino.collections.list(coll_offset).collections;
            }
            // delete Groups
            List<Group> groups = chino.groups.list(0).groups;
            var groups_offset = 0;
            while (groups.Count > 0)
            {
                foreach (Group g in groups)
                {
                    Console.WriteLine(chino.groups.delete(g.group_id, true));
                }
                groups_offset += groups.Count;
                groups = chino.groups.list(0).groups;
            }
        }
    }
}