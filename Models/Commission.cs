using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
namespace APIMarketplaceApp.Models
{
    public class Commission
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string? Id { get; set; }

        [BsonElement("categorie")]
        public string categorie { get; set; }  

        [BsonElement("commission")]
        public string commission { get; set; }  
    }
}