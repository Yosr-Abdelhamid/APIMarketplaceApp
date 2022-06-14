using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace APIMarketplaceApp.Models
{
    public class Contact
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Subject")]
        public string Subject { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }
    }
}