namespace APIMarketplaceApp.Models
{
        public class VendeurLookup  {
        
        public string Id { get; set; }
        public string Reference { get; set; }
        public string sous_famille_prod { get; set; }
        public string Brand { get; set; }
        public int quantity { get; set; }
        public string description_prod { get; set; }
        public string prix_prod { get; set; }
        public string  image_prod { get; set; }

        public List<Vendeur> Vendeurs { get; set; }
        //public List<ProductVend> Produits { get ; set; }

    }
        
}