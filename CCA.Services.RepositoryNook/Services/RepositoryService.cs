using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCA.Services.RepositoryNook.Models;
using CCA.Services.RepositoryNook.Config;
using CCA.Services.RepositoryNook.Exceptions;
using System.Net;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.IO;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Grpc.Core;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using CCA.Services.RepositoryNook.Models.GCP;
using Newtonsoft.Json;

namespace CCA.Services.RepositoryNook.Services
{
    public class RepositoryService : IRepositoryService
    {
        private IJsonConfiguration _config;
        private readonly string GENERIC_DB_NAME = "repository-nook-db";
        private readonly string GENERIC_COLLECTION_NAME = "repository";

        public RepositoryService(IJsonConfiguration config)     // ctor
        {
            _config = config;
        }
        //public async Task<List<string>> GetDatabases()
        //{
        //    var client = new MongoClient(_config.AtlasMongoConnection);
        //    List<string> databases = new List<string>();

        //    using (var cursor = await client.ListDatabasesAsync())
        //    {
        //        await cursor.ForEachAsync(d => databases.Add(d.ToString()));
        //    }
        //    return databases;
        //}
        //public async Task<List<string>> GetCollections(string database)
        //{
        //    var client = new MongoClient(_config.AtlasMongoConnection);
        //    List<string> collections = new List<string>();

        //    using (var cursor = await client.GetDatabase(database).ListCollectionsAsync())
        //    {
        //        await cursor.ForEachAsync(d => collections.Add( new JObject(new JProperty("name",d["name"].ToString())).ToString()));
        //    }
        //    return collections;
        //}
        public async Task<Repository> Create(string repository, string collection, Repository repoObject)
        {
            if (repoObject.validate)
            {
                ValidateInnerDataAgainstSchema(repoObject.schemaUri, repoObject.data);
            }

            GoogleCredential serviceAcct = GoogleCredential.FromJson(GetServiceAccountKey()); //hardcoded

            Channel channel = new Channel(
                FirestoreClient.DefaultEndpoint.Host, FirestoreClient.DefaultEndpoint.Port,
                serviceAcct.ToChannelCredentials());
                
            FirestoreClient client = FirestoreClient.Create(channel);

            FirestoreDb db = FirestoreDb.Create("repositorynookproject", client);
            CollectionReference c = GetCollection(db, collection);


            //convert times to UTC
            repoObject.createdDate = DateTime.SpecifyKind(repoObject.createdDate, DateTimeKind.Utc);
            repoObject.modifiedDate = DateTime.SpecifyKind(repoObject.modifiedDate, DateTimeKind.Utc);

            DocumentReference document = await c.AddAsync(repoObject);
            
            return repoObject;
        }

        //public async Task<Repository> Read(string _id, string repository, string collection)
        //{
        //    IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

        //    var filter = Builders<Repository>.Filter.Eq("_id", new ObjectId(_id));      // FIND with filter  filter("_id" = ObjectId(_id) ) - IS AN ASYNC CALL 
        //    var fluentFindInterface = repositoryCollection.Find(filter);

        //    Repository foundObject = await fluentFindInterface.SingleOrDefaultAsync().ConfigureAwait(false);

        //    if (foundObject is null)
        //    {
        //        throw new RepoSvcDocumentNotFoundException($"DocumentId: {_id}");
        //    }
        //    return foundObject;
        //}
        //public List<Repository> ReadAll(string repository, string collection)           // READ ALL repository object (mongo documents) - NOT AN ASYNC CALL
        //{
        //    IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

        //    var found = repositoryCollection.AsQueryable().ToList();

        //    if (found is null)
        //    {
        //        throw new RepoSvcDocumentNotFoundException("RepositoryService.ReadAll() error.");
        //    }
        //    return found;
        //}
        //public List<Repository> QueryByKey(string repository, string collection, string keyName, string keyValue)
        //{
        //    IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

        //    var found = repositoryCollection.Find(r => r.keyName == keyName && r.keyValue == keyValue).ToList();          // FIND keyName (req) and keyValue (req) - NOT AN ASYNC CALL

        //    if (found is null)
        //    {
        //        throw new RepoSvcDocumentNotFoundException($"keyName: {keyName}, keyValue: {keyValue}");
        //    }
        //    return found;
        //}
        //public List<Repository> QueryByTag(string repository, string collection, string tagName, string tagValue)
        //{
        //    IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

        //    var builder = Builders<Repository>.Filter.ElemMatch(t => t.tags, x => x.Name == tagName && x.Value == tagValue);  // FIND tagName (req) and tagValue (req) - NOT AN ASYNC CALL

        //    var found = repositoryCollection.Find(builder).ToList();

        //    if (found is null)
        //    {
        //        throw new RepoSvcDocumentNotFoundException($"tagName: {tagName}, tagValue: {tagValue}");
        //    }
        //    return found;
        //}
        //public async Task Update(string _id, string repository, string collection, Repository repoObject)
        //{

        //    if (repoObject.validate)
        //    {
        //        ValidateInnerDataAgainstSchema(repoObject.schemaUri, repoObject.data);
        //    }

        //    IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

        //    var filter = Builders<Repository>.Filter.Eq("_id", new ObjectId(_id));
        //    repoObject._id = new ObjectId(_id);   // object-ize the GUID string

        //    if (repoObject.modifiedDate is null)
        //    {
        //        repoObject.modifiedDate = DateTime.Now;
        //    }
        //    var replaceOneResult = await repositoryCollection.ReplaceOneAsync(filter, repoObject, new UpdateOptions { IsUpsert = true });

        //    if (replaceOneResult.ModifiedCount == 0)
        //    {
        //        throw new RepoSvcDocumentNotFoundException($"DocumentId: {_id}");
        //    }

        //}

        //public async Task Delete(string _id, string repository, string collection)
        //{
        //    IMongoCollection<Repository> repositoryCollection = ConnectToCollection(repository, collection);

        //    var filter = Builders<Repository>.Filter.Eq("_id", ObjectId.Parse(_id));
        //    var result = await repositoryCollection.DeleteOneAsync(filter);

        //    if (result.DeletedCount != 1)
        //    {
        //        throw new RepoSvcDocumentNotFoundException($"DocumentId: {_id}");
        //    }
        //}

        //
        // private routines
        //
        private string GetServiceAccountKey()
        {
            ServiceAccount serviceAccount = new ServiceAccount();

            serviceAccount.type = "service_account";            // hardcoding for now, as opposed to reading from generated JSON file (see SolutionItems\GCP\ServiceAccountKey file)
            serviceAccount.project_id = "repositorynookproject";
            serviceAccount.private_key_id = "c8da459c44bf860f1f17d66a9fc337ce090c765b";
            serviceAccount.private_key = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCFsd8kElkdwrSD\nzDdY5kYdGJtk3pKHJjCoa+kqf1Fdq+Qhkk3StR6x7arNUxZatQRLiZvrXQRVwoOY\nWx9WT27hb1vJV4XDKHQ6bj9Kn4+VoiINBMTIv1MvyghLq0gYmR8j0DoKpjOsO6D6\ncaPeR98KX47txx5l6/oM7gZiHXSNFMJ8EDQE/S6rjVKoHBB1OMnC4HdFZ1qsqXu0\nomj4fWY70putMTxCtfTQdev2fPIhxh0jLJl5s0eosTfc4IGXopkbCryoh35eiru2\nWuiDwQlcGhxxuuH18j7ER6VZYSgH+t5myHfBbQMOiPK0rKePzDJnMDESaBiNSsnk\ne3A7GOPfAgMBAAECggEABrOMwM0epGw/CiAxYgZg21KBb4PsilOyuq52CIhLNsvH\n6prfxcEIYdjM5+/TPga2yWaDZz/JWNcgK4Hdx0DCCRDA5A28CbU8ZjVifMYUt2Zi\nGI71oi5c6BdTZR+XxIvgULY+QoxkKKzwe704forRtj5l9qs1bdxvMzi3HzHkxrGC\nJSYm+N9P2ZDq3nw6lgXEkByudRk4a21kc2Q/oa3AGHPBvrClPx/bx/xLwN3hRvwA\ngc+QRRDc0s5EykkKYL8AqSSHxAaqu/CqPYtr25o03uA4C02pZ9/z56Ie0EHcMUy1\nf2SeiHigj6vGkREfvzjepj4QEwKQ8bDaVZvCm6QmGQKBgQC89Zl7p0khrlCGA2nA\nLRDUb6tN3gM0mfKUxv2c4MzFAknyAwyLnyBWLJzfgpfVs0RQInTx361+NFFhJQPD\nNKzHcRSIbEG/feA/K94rus8WXIkRwMqsifUCzgdrGhxNBz8mxSy32F0z0ZSl7U2S\nPs/e2wMj9EzkBhL5TkW+WfgkWwKBgQC1INHjiY5Gvq2HuMt6u+62Sf4tPVWJElbm\n3gDUq6Y0ay/OpYPY8l0MxGqkZ0yDT1Ay8ybus+KSOmMkrtXpf5vbKPkEQ+A4tnmI\nSsRS4FGwNLU1H09iRIdX5KMCB7r2WAXEP3xFjaMsL3I3XD3mTC8RnCyPGWFsdj6/\no9cSK2oFzQKBgDJjO55hqXq1xPs3hcedPNOpQ4DsJuar9qf9uDtRJsmSJq3Gal4A\n/Np94wcnB94Qg7LqvUySXO0+fkTtXed78GbunI8UbyPlKRsvU3tNwVMxMcvuIR2J\nXDB3SDsjJ1DTEeAAzD/qDlB3HrBwazMIVN4UgO3hg296vyyD1s0/qI/pAoGBAII6\nZq5CaJlU30+F/kbweGF4Mdg1ERrMpM65L7+46ncl2emp93I0T9KuJj6uRsTicbcw\nO/3EOFMKx93IFuUbauPYQbJfWwdrq1Xi6+Tqg9E9FExthpYQz37SVKFRDYuxHfRL\n8P3RKFDMjEJhf9/lRrJSp3b7uKBTJCqFkkzuCfzBAoGAMp0U/nDcTlUw9FbOEm/U\ngWK3M5Ao/rCpc9lPil8Vv/k51RA4OZtnNER6dJZNkY7i0OxDSGEZwACOOGPgGDoy\nPro7xXFhXxffb9qGyppFchDrNimWrC3K6UNMYziXof8x6KhmLWXeh7gYAnbC6J5V\nHGhkBJb2cjI/B/O9Uzxbakg=\n-----END PRIVATE KEY-----\n";
            serviceAccount.client_email = "repositorynook@repositorynookproject.iam.gserviceaccount.com";
            serviceAccount.client_id = "101061036925064764809";
            serviceAccount.auth_uri = "https://accounts.google.com/o/oauth2/auth";
            serviceAccount.token_uri = "https://oauth2.googleapis.com/token";
            serviceAccount.auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs";
            serviceAccount.client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/repositorynook%40repositorynookproject.iam.gserviceaccount.com";


            return JsonConvert.SerializeObject(serviceAccount);
        }

        private void ValidateInnerDataAgainstSchema(string schemaUri, string data)
        {
            JObject jobject = null;
            JSchema schema = null;
            try
            {
                schema = JSchema.Parse(ReadInStringFromWebUri(schemaUri));
                jobject = JObject.Parse(data);
            }
            catch (Exception exc)
            {
                throw new RepoSvcValidationError("Error parsing schema or data JSON, please check schema URI and file, and data for valid JSON, and retry.");
            }
            if (!jobject.IsValid(schema))
            {
                throw new RepoSvcValidationError("Invalid Error; validating data against schema failed. Check data and schema and retry.");
            };
        }

        private CollectionReference GetCollection(FirestoreDb db, string collectionName)
        {
            CollectionReference collection = db.Collection(collectionName);

            return collection;
        }


        //private void CreateRepositoryTextIndices(IMongoCollection<Repository> collection)   // indempotent; a no-op if index already exists.
        //{
        //    // text search field     .Text()   is the keyValue
        //    var textKey = Builders<Repository>.IndexKeys.Text(t => t.keyValue);             // the key value, is collections text search field, and is highly queryable
        //    var options = new CreateIndexOptions() { Name = "IX_keyValue}" };
        //    collection.Indexes.CreateOne(textKey, options);

        //    // another indexed field   is the  keyName
        //    var indexKey = Builders<Repository>.IndexKeys.Ascending(i => i.keyName);        // the key name, is text and is indexed for speedier queries
        //    var ix_options = new CreateIndexOptions() { Name = "IX_keyName}" };
        //    collection.Indexes.CreateOne(indexKey, ix_options);

        //    // finally the tags are madesearchable
        //    var tagsKey = Builders<Repository>.IndexKeys.Ascending(t => t.tags);            // the tags array
        //    var tags_ix_options = new CreateIndexOptions() { Name = "IX_tags}" };
        //    collection.Indexes.CreateOne(tagsKey, tags_ix_options);
        //}         

        private string ReadInStringFromWebUri(string schemaUri)
        {
            try
            {
                WebRequest request = WebRequest.Create(schemaUri);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                string schemaBody = String.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    schemaBody = sr.ReadToEnd();
                }
                return schemaBody;
            }
            catch (Exception exc)
            {
                throw new RepoSvcValidationError("error: reading in string from Uri. Check URI string and/or file existence, and retry.");
            }
        }
    }
}