using MongoDB.Bson.Serialization.Attributes;

namespace APIMarketplaceApp.Models
{
    public class ProduitOrder
    {
            [BsonElement("reference")]
            public string reference { get; set; }

            [BsonElement("prix")]
            public decimal prix { get; set; }

            [BsonElement("organization")]
            public string organization { get; set; }
        
    }
}