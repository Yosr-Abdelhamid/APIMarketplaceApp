using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace APIMarketplaceApp.Models
{
    public class ProductVend
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string?  Id_prod{ get; set; }

        [BsonElement("Reference")]
        public string Reference { get; set; }

        [BsonElement("sous_famille_prod")]
        public string sous_famille_prod { get; set; }

        [BsonElement("Brand")]
        public string Brand { get; set; }

        [BsonElement("quantity")]
        public int quantity { get; set; }

        [BsonElement("description_prod")]
        public string description_prod { get; set; }

        [BsonElement("prix_prod")]
        public decimal prix_prod { get; set; }

        [BsonElement("image_prod")]
        public string  image_prod { get; set; }
        
        [BsonElement("Id")]
        public string Id { get; set; }
        }
    
}