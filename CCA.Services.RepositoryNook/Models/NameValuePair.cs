using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCA.Services.RepositoryNook.Models
{
    [FirestoreData]
    public class NameValuePair
    {
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Value { get; set; }
    }
}
