using Google.Cloud.Firestore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CCA.Services.RepositoryNook.Models
{
    [FirestoreData]
    public class Repository
    {
        [FirestoreProperty]
        public object _id { get; set; }
        [FirestoreProperty]
        public string keyName { get; set; }
        [FirestoreProperty]
        public string keyValue { get; set; }
        [FirestoreProperty]
        public IEnumerable<NameValuePair> tags { get; set; }
        [FirestoreProperty]
        public DateTime createdDate { get; set; }
        [FirestoreProperty]
        public string createdBy { get; set; }
        [FirestoreProperty]
        public DateTime modifiedDate { get; set; }
        [FirestoreProperty]
        public string modifiedBy { get; set; }
        [FirestoreProperty]
        public string app { get; set; }
        [FirestoreProperty]
        public string repository { get; set; }
        [FirestoreProperty]
        public string collection { get; set; }
        [FirestoreProperty]
        public bool validate { get; set; }
        [FirestoreProperty]
        public string schemaUri { get; set; }
        [FirestoreProperty]
        public string data { get; set; }
    }
}
