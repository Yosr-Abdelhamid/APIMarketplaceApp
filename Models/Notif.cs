namespace APIMarketplaceApp.Models;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

    public class Notif
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Message")]
        public string Message { get; set; }

        [BsonElement("Id_vendeur")]
        public string Id_vendeur { get; set; }

        [BsonElement("Date")]
        public DateTime Date { get; set; }
    }
