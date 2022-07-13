namespace APIMarketplaceApp.Models
{
    public class RequestOrder
    {
        public string name { get; set; }
        public string lastName { get; set; }
        public string company { get; set; }
        public string country { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public int zip { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public List<ProduitOrder> produits { get; set; }
        public  double total { get; set; }  
    }
}