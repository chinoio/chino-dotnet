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

        internal static bool isInitialized()
        {
            return _customerId != null && _customerKey != null && _hostUrl != null;
        }

        public static void deleteAll(ChinoAPI chino)
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
}