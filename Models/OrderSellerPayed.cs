using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace APIMarketplaceApp.Models
{
    public class OrderBySellerPayed
    {
         [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string Id { get; set; }

        [BsonElement("Organization")]
        public string Organization { get; set; }

        [BsonElement("id_vendeur")]
        public string id_vendeur { get; set; }

        [BsonElement("id_order")]
        public string id_order { get; set; }

        [BsonElement("payed")]
        public string payed { get; set; }

        [BsonElement("datepayed")]
        public string datepayed { get; set; }

    }
}