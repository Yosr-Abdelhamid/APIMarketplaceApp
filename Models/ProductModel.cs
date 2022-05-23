namespace APIMarketplaceApp.Models
{
    public class ProductModel
    {
        public string Id_prod { get; set; }
        public string reference_prod { get; set; }
        public string sous_famille_prod { get; set; }
        public string Brand { get; set; }
        public int quantity { get; set; }
        public string description_prod { get; set; }
        public decimal prix_prod { get; set; }
        public string image_prod { get; set; }
    }
}