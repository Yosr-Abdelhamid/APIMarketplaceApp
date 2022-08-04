using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
namespace APIMarketplaceApp.Models
{
    public class PortfeuilleVendeur
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string? Id_portf { get; set; }

         [BsonElement("CardName")]
        public string CardName { get; set; }

        [BsonElement("CardNumber")]
        public string CardNumber { get; set; }

        [BsonElement("ExpireDate")]
        public string ExpireDate { get; set; }

        [BsonElement("CVV")]
        public string CVV { get; set; }

        [BsonElement("Sold")]
        public string Sold { get; set; }

        [BsonElement("Id")]
        public string Id { get; set; }
        

        
    }
}