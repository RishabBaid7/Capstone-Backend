using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConstructionManagement_Backend.Models
{
    public class Vendor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id {get; set;}

        public string Name { get; set; }

        public string ContactDetails { get; set; }

        public string MaterialSupplied { get; set; }

        public string ContractTerms { get; set; }

        public string DeliveryStatus { get; set; }
    }
}
