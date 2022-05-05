using MongoDB.Driver;

namespace APIMarketplaceApp.Models
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _mongoDb;
        public MongoDbContext()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _mongoDb = client.GetDatabase("MarketplaceSiteDB");
        }
        public IMongoCollection<Vendeur> Vendeur
        {
            get
            {
                return _mongoDb.GetCollection<Vendeur>("Vendeur");
            }
        }
    }
}
