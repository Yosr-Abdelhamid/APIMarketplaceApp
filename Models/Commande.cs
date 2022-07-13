using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace APIMarketplaceApp.Models
{
    public class Commande
    {
    
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string name { get; set; }

        [BsonElement("lastName")]
        public string lastName { get; set; }

        [BsonElement("country")]
        public string country { get; set; }

        [BsonElement("street")]
        public string street { get; set; }

        [BsonElement("city")]
        public string city { get; set; }

        [BsonElement("zip")]
        public int zip { get; set; }
        
        [BsonElement("phone")]
        public string phone { get; set; }

        [BsonElement("email")]
        public string email { get; set; }

        [BsonElement("produits")]
        public virtual List<ProduitOrder>? produits { get; set; }

        [BsonElement("total")]
        public  double total { get; set; }

         [BsonElement("payment")]
        public  string payment { get; set; }

    }
        
}